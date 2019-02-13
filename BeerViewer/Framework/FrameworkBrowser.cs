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

using Gecko;
using Gecko.DOM;

namespace BeerViewer.Framework
{
	// This file is not for usercontrol
	public class FrameBrowserPlaceholder { }

	public class JavascriptResponse
	{
		public bool Success { get; }
		public dynamic Result { get; }

		public JavascriptResponse(bool Success, JsVal Value)
		{
			this.Success = Success;
			this.Result =
				Value.IsBoolean ? Value.ToBoolean() :
				Value.IsDouble ? Value.ToDouble() :
				Value.IsInt ? Value.ToInteger() :
				Value.IsNull ? null :
				Value.IsString ? Value.ToString() :
				Value.IsObject ? Value.ToObject() :
				null;
		}
	}

	public class FrameworkBrowser : GeckoWebBrowser
	{
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
			var baseDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

			if (!Settings.HardwareAccelerationEnabled)
			{
				GeckoPreferences.Default["gfx.direct2d.disabled"] = true;
				GeckoPreferences.Default["layers.acceleration.disabled"] = true;
			}
			GeckoPreferences.Default["browser.xul.error_pages.enabled"] = true;
			GeckoPreferences.Default["devtools.debugger.remote-enabled"] = true;

			Xpcom.ProfileDirectory = Path.Combine(baseDir, "CacheData");

			GeckoPreferences.Default["network.proxy.type"] = 1;
			GeckoPreferences.Default["network.proxy.http"] = "localhost";
			GeckoPreferences.Default["network.proxy.http_port"] = Network.Proxy.Instance.ListeningPort;
			GeckoPreferences.Default["network.proxy.ssl"] = "localhost";
			GeckoPreferences.Default["network.proxy.ssl_port"] = Network.Proxy.Instance.ListeningPort;

			Xpcom.Initialize(Path.Combine(baseDir, "Gecko"));
		}

		public FrameworkBrowser(string address) : base()
		{
			var browserHandle = this.Handle;

		}
		/*
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
		*/
	}
	public static class FrameworkBrowserExtension
	{
		public static void LoadUrl(this FrameworkBrowser browser, string url)
			=> browser?.LoadUrl(url);

		public static JavascriptResponse EvaluateScriptAsync(this FrameworkBrowser browser, string script, string scriptUrl = "about:blank", int startLine = 1, TimeSpan? timeout = null)
		{
			using (var js = new AutoJSContext(browser.Window))
			{
				var ret = js.EvaluateScript(script, (nsISupports)browser.Window.DomWindow);
				return new JavascriptResponse(!ret.IsUndefined, ret);
			}
		}
		public static JavascriptResponse EvaluateScriptAsync(this GeckoIFrameElement browser, string script, string scriptUrl = "about:blank", int startLine = 1, TimeSpan? timeout = null)
		{
			using (var js = new AutoJSContext(browser.Window))
			{
				var ret = js.EvaluateScript(script, (nsISupports)browser.ContentWindow.DomWindow);
				return new JavascriptResponse(!ret.IsUndefined, ret);
			}
		}

		public static void ZoomAsPercentage(this FrameworkBrowser browser, double zoomFactor)
		{
			try
			{
				browser.GetDocShellAttribute()
					.GetContentViewerAttribute()
					.SetFullZoomAttribute((float)zoomFactor);
			}
			catch (Exception ex)
			{
				Logger.Log(ex.ToString());
			}
		}
	}
}
