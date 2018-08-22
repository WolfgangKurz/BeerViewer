using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BeerViewer.Modules;
using CefSharp;
using CefSharp.WinForms;

namespace BeerViewer.Framework
{
	// This file is not for usercontrol

	public class NoMenuHandler : IContextMenuHandler
	{
		public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
			=> model.Clear();

		public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
			=> false;

		public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
		{
		}

		public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
			=> false;
	}


	internal class ChromeWidgetMessageInterceptor : NativeWindow
	{
		public delegate bool ChromeMessage(ref Message msg, BaseWndProc WndProc);
		public delegate void BaseWndProc(ref Message msg);

		private ChromeMessage forwardAction;

		internal ChromeWidgetMessageInterceptor(Control browser, IntPtr chromeWidgetHostHandle, ChromeMessage forwardAction)
		{
			AssignHandle(chromeWidgetHostHandle);

			browser.HandleDestroyed += BrowserHandleDestroyed;

			this.forwardAction = forwardAction;
		}

		private void BrowserHandleDestroyed(object sender, EventArgs e)
		{
			ReleaseHandle();

			var browser = (Control)sender;

			browser.HandleDestroyed -= BrowserHandleDestroyed;
			forwardAction = null;
		}

		protected override void WndProc(ref Message m)
		{
			var handled = forwardAction?.Invoke(ref m, base.WndProc) ?? false;
			if (!handled) base.WndProc(ref m);
		}
	}

	internal static class ChromeWidgetHandleFinder
	{
		private delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

		[DllImport("user32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		private class ClassDetails
		{
			public IntPtr DescendantFound { get; set; }
		}

		private static bool EnumWindow(IntPtr hWnd, IntPtr lParam)
		{
			const string chromeWidgetHostClassName = "Chrome_RenderWidgetHostHWND";

			var buffer = new StringBuilder(128);
			GetClassName(hWnd, buffer, buffer.Capacity);

			if (buffer.ToString() == chromeWidgetHostClassName)
			{
				var gcHandle = GCHandle.FromIntPtr(lParam);

				var classDetails = (ClassDetails)gcHandle.Target;

				classDetails.DescendantFound = hWnd;
				return false;
			}

			return true;
		}

		/// <summary>
		/// Chrome's message-loop Window isn't created synchronously, so this may not find it.
		/// If so, you need to wait and try again later.
		/// </summary>
		public static bool TryFindHandle(IntPtr browserHandle, out IntPtr chromeWidgetHostHandle)
		{
			var classDetails = new ClassDetails();
			var gcHandle = GCHandle.Alloc(classDetails);

			var childProc = new EnumWindowProc(EnumWindow);
			EnumChildWindows(browserHandle, childProc, GCHandle.ToIntPtr(gcHandle));

			chromeWidgetHostHandle = classDetails.DescendantFound;

			gcHandle.Free();

			return classDetails.DescendantFound != IntPtr.Zero;
		}
	}

	public class FrameworkBrowser : ChromiumWebBrowser
	{
		static FrameworkBrowser()
		{
			var cefSettings = new CefSettings()
			{
				CachePath = Path.Combine(
					Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
					"CacheData"
				),
				BrowserSubprocessPath = Path.Combine(
					Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
					"Libs",
					"CefSharp.BrowserSubprocess.exe"
				),
			};
			cefSettings.CefCommandLineArgs.Add("disable-extensions", "1");
			// cefSettings.DisableGpuAcceleration();

			CefSharpSettings.Proxy = new ProxyOptions("localhost", "49217");
			CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
			Cef.EnableHighDPISupport();
			Cef.Initialize(cefSettings, performDependencyCheck: false, browserProcessHandler: null);
		}

		public FrameworkBrowser(string address, IRequestContext requestContext = null) : base(address, requestContext)
		{
			ChromeWidgetMessageInterceptor messageInterceptor;
			var browserHandle = this.Handle;

			Task.Run(() =>
			{
				try
				{
					while (true)
					{
						IntPtr chromeWidgetHostHandle;
						if (ChromeWidgetHandleFinder.TryFindHandle(browserHandle, out chromeWidgetHostHandle))
						{
							messageInterceptor = new ChromeWidgetMessageInterceptor(this, chromeWidgetHostHandle, handle);

							break;
						}
						else
						{
							// Chrome hasn't yet set up its message-loop window.
							Thread.Sleep(10);
						}
					}
				}
				catch
				{
					// Errors are likely to occur if browser is disposed, and no good way to check from another thread
				}
			});
		}

		private bool handle(ref Message message, ChromeWidgetMessageInterceptor.BaseWndProc WndProc)
		{
			if (message.Msg == 0x84)
			{
				WndProc(ref message);
				if(message.Result == (IntPtr)1)
					message.Result = (IntPtr)(-1);
				return true;
			}
			//Forward mouse button down message to browser control
			//else if(message.Msg == WM_LBUTTONDOWN)
			//{
			//    PostMessage(browserHandle, WM_LBUTTONDOWN, message.WParam, message.LParam);
			//}

			// The ChromiumWebBrowserControl does not fire MouseEnter/Move/Leave events, because Chromium handles these.
			// However we can hook into Chromium's messaging window to receive the events.
			//
			//const int WM_MOUSEMOVE = 0x0200;
			//const int WM_MOUSELEAVE = 0x02A3;
			//
			//switch (message.Msg) {
			//    case WM_MOUSEMOVE:
			//        Console.WriteLine("WM_MOUSEMOVE");
			//        break;
			//    case WM_MOUSELEAVE:
			//        Console.WriteLine("WM_MOUSELEAVE");
			//        break;
			//}
			return false;
		}

		public void Zoom(int zoomFactor)
		{
			try
			{
				this.SetZoomLevel(0);
				this.SetZoomLevel(Math.Log(zoomFactor / 100.0) / Math.Log(1.2));
			}
			catch (Exception ex)
			{
				Logger.Log(ex.ToString());
			}
		}
	}
}
