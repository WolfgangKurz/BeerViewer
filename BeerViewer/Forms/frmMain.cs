using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using BeerViewer.Framework;
using BeerViewer.Models;
using BeerViewer.Modules;

using System.Runtime.InteropServices;

namespace BeerViewer.Forms
{
	public partial class frmMain : BorderlessWindow
	{
		[DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		private static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

		public static frmMain Instance { get; }

		public FrameworkBrowser WindowBrowser { get; }
		public IFrameworkBrowser GameBrowser { get; private set; }
		public WindowBrowserCommicator Communicator { get; }

		static frmMain()
		{
			frmMain.Instance = new frmMain();
		}
		private frmMain() : base()
		{
			InitializeComponent();
			Master.Instance.Ready();
			Homeport.Instance.Ready();

			this.FormClosed += (s, e) =>
			{
				// Hide application first
				this.Hide();
				Application.DoEvents();

				this.Controls.Remove(this.WindowBrowser);

				Network.Proxy.Instance.Dispose();
				Application.Exit();
			};

			/// Load <see cref="WindowInfo" /> settings
			{
				var info = Settings.WindowInformation.Value;
				this.Size = new Size(info.Width, info.Height);
				if (info.Left.HasValue) this.Left = info.Left.Value;
				if (info.Top.HasValue) this.Top = info.Top.Value;
			}
			this.MinimumSize = new Size(
				1200 + 2,
				720 + 28 + 2
			);
			this.ResizeEnd += (s, e) => Settings.WindowInformation.Value = this.GetWindowInformation();
			this.Move += (s, e) => Settings.WindowInformation.Value = this.GetWindowInformation();
			this.ClientSizeChanged += (s, e) => this.Communicator?.CallScript("WindowState", ((int)this.WindowState).ToString());


			#region GC timer
#if USE_GC
			{
				var timer = new System.Timers.Timer(5000);
				timer.Elapsed += (s, e) =>
				{
					GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
					GC.WaitForPendingFinalizers();

					if (Environment.OSVersion.Platform == PlatformID.Win32NT)
						SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
				};
				timer.Start();
			}
#endif
			#endregion

			#region ComponentService
			ComponentService.Instance.Initialize();
			#endregion

			#region WindowBrowser
			this.WindowBrowser = new FrameworkBrowser("")
			{
				Dock = DockStyle.None,
				AllowDrop = false,
				Location = new Point(1, 1),
			};
			this.WindowBrowser.JavascriptObjectRepository.Register(
				"API",
				this.Communicator = new WindowBrowserCommicator(
					this,
					this.WindowBrowser,
					() =>
					{
						// After communicator initialized
						this.GameBrowser = this.WindowBrowser.GetBrowser().GetFrame("MAIN_FRAME")?.Browser as IFrameworkBrowser;
						// this.GameBrowser.Load(Constants.GameURL);
					}
				),
				true
			);
			this.WindowBrowser.FrameLoadEnd += async (s, e) =>
			{
				var rootUri = Extensions.UriOrBlank(e.Browser.MainFrame?.Url);
				var frameUri = Extensions.UriOrBlank(e.Url);

				// Cookie patch
				if (rootUri.Host == "www.dmm.com")
					await this.GameBrowser?.EvaluateScriptAsync(Constants.DMMCookie);

				// CSS patch
				if (rootUri.AbsoluteUri == Constants.GameURL)
				{
					var script =
					(
						@"var x = document.querySelector('#game_style');
						if(!x) {
							x = document.createElement('style');
							x.id='game_style';
							x.type='text/css';
							x.innerHTML='" + Constants.UserStyleSheet + @"';
							document.body.appendChild(x);
						}"
					).ToEvaluatableString();
					await this.GameBrowser?.EvaluateScriptAsync(script);
				}
			};
			this.Resize += (s, e) => this.WindowBrowser.Size = this.ClientSize;
			this.WindowBrowser.Load("file:///" + Constants.EntryDir.Replace("\\", "/") + "/WindowFrame/Application.html");
			this.Controls.Add(this.WindowBrowser);

			this.WindowBrowser.IsBrowserInitializedChanged += (s, e) =>
			{
				if (e.IsBrowserInitialized)
					this.WindowBrowser.GetBrowser().GetHost().ShowDevTools();
			};
			#endregion

			this.OnResize(EventArgs.Empty);
		}

		protected override Rectangle CaptionSize(int Width, int Height)
		{
			var size = base.CaptionSize(Width, Height);
			size.X += 120;
			size.Width -= 120;
			return size;
		}
	}
}
