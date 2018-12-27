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

			var mainWindow = frmMain.Instance;
			FrameworkManager.Instance.Initialize();

			Application.Run(mainWindow);

			Cef.Shutdown();
		}
	}
}
