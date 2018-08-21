using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
