using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using CefSharp;

namespace BeerViewer.Framework
{
	internal class FrameworkManager
	{
		public static FrameworkManager Instance { get; } = new FrameworkManager();

		public void Initialize()
		{
			Cef.EnableHighDPISupport();
			FrameworkManager.InitCEF(false, true, null);
		}

		private static void InitCEF(bool osr, bool multiThreadedMessageLoop, IBrowserProcessHandler browserProcessHandler)
		{
			var settings = new CefSettings();
			settings.RemoteDebuggingPort = 8088;
			settings.CachePath = "cache";

			settings.CefCommandLineArgs.Add("disable-plugins-discovery", "1"); //Disable discovering third-party plugins. Effectively loading only ones shipped with the browser plus third-party ones as specified by --extra-plugin-dir and --load-plugin switches
			settings.CefCommandLineArgs.Add("disable-pdf-extension", "1"); //The PDF extension specifically can be disabled
			// settings.CefCommandLineArgs.Add("disable-extensions", "1"); //Extension support can be disabled
			settings.CefCommandLineArgs.Add("proxy-auto-detect", "1");

			settings.MultiThreadedMessageLoop = multiThreadedMessageLoop;
			settings.ExternalMessagePump = !multiThreadedMessageLoop;

			settings.FocusedNodeChangedEnabled = true;

			var architecture = Environment.Is64BitProcess ? "x64" : "x86";
			var path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			path = Path.Combine(path, architecture, "CefSharp.BrowserSubprocess.exe");
			settings.BrowserSubprocessPath = path;

			if (!Cef.Initialize(settings, false, browserProcessHandler))
				throw new Exception("Unable to Initialize Cef");
		}
	}
}
