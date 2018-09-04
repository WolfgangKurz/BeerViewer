using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
using CefSharp.Enums;
using CefSharp.WinForms;

namespace BeerViewer.Framework
{
	// This file is not for usercontrol
	public class FrameBrowserPlaceholder { }

	public class FrameworkBrowser : ChromiumWebBrowser
	{
		private class ChromeWidgetMessageInterceptor : NativeWindow
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
		private static class ChromeWidgetHandleFinder
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
		private class NoMenuHandler : IContextMenuHandler
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

		[DllImport("User32.dll")]
		internal static extern bool ReleaseCapture();
		[DllImport("User32.dll")]
		internal static extern bool SetCapture(IntPtr hWnd);
		[DllImport("User32.dll")]
		internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[DllImport("User32.dll")]
		internal static extern int PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[DllImport("user32.dll")]
		private static extern long GetWindowLong(IntPtr hWnd, int nIndex);

		private const int GWL_STYLE = -16;
		private const long WS_MAXIMIZE = 0x01000000L;

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

#if !DEBUG
			cefSettings.LogSeverity = LogSeverity.Disable;
#endif
			// cefSettings.DisableGpuAcceleration();

			CefSharpSettings.Proxy = new ProxyOptions("localhost", Network.Proxy.Instance.ListeningPort.ToString());
			CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
			CefSharpSettings.ShutdownOnExit = true;
			Cef.EnableHighDPISupport();
			Cef.Initialize(
				cefSettings,
				false,
				null
			);
		}

		public FrameworkBrowser(string address, IRequestContext requestContext = null) : base(address, requestContext)
		{
			ChromeWidgetMessageInterceptor messageInterceptor;
			var browserHandle = this.Handle;

			this.MenuHandler = new NoMenuHandler();

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
							Thread.Sleep(10);
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
			if (message.Msg == (int)WindowMessages.WM_LBUTTONDOWN) // WM_LBUTTONDOWN
			{
				IntPtr handle = IntPtr.Zero;
				this.Parent.Invoke((Action)(() => handle = this.Parent.Handle));
				if (handle == IntPtr.Zero) return false;

				var pt = new Point(message.LParam.ToInt32());
				this.Invoke((Action)(() => pt = this.PointToScreen(pt)));

				var lParam = (pt.X & 0xffff) | ((pt.Y & 0xffff) << 16);
				var HitTest = SendMessage(handle, (int)WindowMessages.WM_NCHITTEST, 0, (int)lParam);
				if (HitTest == (int)HitTestValue.HTCLIENT) return false; // HTCLIENT

				PostMessage(handle, (int)WindowMessages.WM_NCLBUTTONDOWN, HitTest, (int)lParam);

				return true;
			}
			else if (message.Msg == (int)WindowMessages.WM_LBUTTONDBLCLK)
			{
				IntPtr handle = IntPtr.Zero;
				this.Parent.Invoke((Action)(() => handle = this.Parent.Handle));
				if (handle == IntPtr.Zero) return false;

				var pt = new Point(message.LParam.ToInt32());
				this.Invoke((Action)(() => pt = this.PointToScreen(pt)));

				var lParam = (pt.X & 0xffff) | ((pt.Y & 0xffff) << 16);
				var HitTest = SendMessage(handle, (int)WindowMessages.WM_NCHITTEST, 0, (int)lParam);
				if (HitTest == (int)HitTestValue.HTCLIENT) return false; // HTCLIENT

				if (HitTest == (int)HitTestValue.HTCAPTION)
				{
					if ((GetWindowLong(handle, GWL_STYLE) & WS_MAXIMIZE) != 0)
						PostMessage(handle, (int)WindowMessages.WM_SYSCOMMAND, (int)SystemCommand.SC_RESTORE, 0);
					else
						PostMessage(handle, (int)WindowMessages.WM_SYSCOMMAND, (int)SystemCommand.SC_MAXIMIZE, 0);
				}
				return true;
			}

			return false;
		}
	}
	public static class FrameworkBrowserExtension
	{
		public static void LoadUrl(this IFrame frame, string url)
			=> frame?.LoadUrl(url);

		public static Task<JavascriptResponse> EvaluateScriptAsync(this IFrame frame, string script, string scriptUrl = "about:blank", int startLine = 1, TimeSpan? timeout = null)
			=> frame?.EvaluateScriptAsync(script, scriptUrl, startLine, timeout);

		public static void ZoomAsPercentage(this IFrame frame, double zoomFactor)
		{
			try
			{
				frame?.Browser.SetZoomLevel(0);
				frame?.Browser.SetZoomLevel(Math.Log(zoomFactor / 100.0) / Math.Log(1.2));
			}
			catch (Exception ex)
			{
				Logger.Log(ex.ToString());
			}
		}
	}
}
