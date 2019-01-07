using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using BeerViewer.Framework;
using BeerViewer.Forms;
using System.IO;
using System.Reflection;

using CefSharp;
using CefSharp.WinForms;
using BeerViewer.Libraries;
using MetroTrilithon.Desktop;
using System.Diagnostics;

namespace BeerViewer
{
	static class Program
	{
		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// Unhandled Exception
			AppDomain.CurrentDomain.UnhandledException += (sender, args) => ReportError("Application", sender, args.ExceptionObject as Exception);
			Application.ThreadException += (sender, args) => ReportError("Application", sender, args.Exception);

			// Compile SCSS to CSS
			Sass.Instance.CompileRecursive(
				Path.Combine(
					Constants.EntryDir,
					"WindowFrame"
				)
			);

			// When compile SCSS to CSS only mode
			if (Environment.GetCommandLineArgs().Any(x => x.ToLower() == "-css-only"))
				return;

			var mainWindow = frmMain.Instance;
			FrameworkManager.Instance.Initialize();

			Application.Run(mainWindow);

			Cef.Shutdown();
		}

		private static void ReportError(string Caller, object Sender, Exception ExceptionObject)
		{
			try
			{
				var now = DateTimeOffset.Now;
				var path = Path.Combine(
					Constants.EntryDir,
					"ErrorReports",
					$"ErrorReport-{now:yyyyMMdd-HHmmss}-{now.Millisecond:000}.log"
				);

#if DEBUG
				var IsDebug = true;
#else
				var IsDebug = false;
#endif

				var assembly = Assembly.GetEntryAssembly();
				var product = ((AssemblyProductAttribute)assembly.GetCustomAttribute(typeof(AssemblyProductAttribute))).Product;
				var _ver = assembly.GetName().Version;
				var version = $"{_ver.ToString(3)}{(IsDebug ? " dev" : "")}{(_ver.Revision == 0 ? "" : " r" + _ver.Revision)}";

				var message = $@"# Error Report from {Caller}
### {product} v{version}
### Report time: {now:yyyy-MM-dd HH:mm:ss}.{now.Millisecond:000}

{new SystemEnvironment()}

## Error information
Sender:    {(Sender is Type t ? t : Sender?.GetType())?.FullName}
Exception: {ExceptionObject?.GetType().FullName}

{ExceptionObject}
";

				Directory.CreateDirectory(Path.GetDirectoryName(path));
				File.AppendAllText(path, message);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			Application.Exit();
		}
	}
}
