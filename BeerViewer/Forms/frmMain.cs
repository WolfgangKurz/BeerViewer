// #define USE_GC

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
using BeerViewer.Modules.Communication;

using System.Runtime.InteropServices;

using IFrameworkBrowserFrame = CefSharp.IFrame;

namespace BeerViewer.Forms
{
	public partial class frmMain : BorderlessWindow
	{
		[DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		private static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

		public static frmMain Instance { get; }

		public FrameworkBrowser WindowBrowser { get; }
		public IFrameworkBrowserFrame GameBrowser { get; private set; }
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
			};

			/// Load <see cref="WindowInfo" /> settings
			{
				var info = Settings.WindowInformation.Value;
				this.Size = new Size(info.Width, info.Height);
				if (info.Left.HasValue) this.Left = info.Left.Value;
				if (info.Top.HasValue) this.Top = info.Top.Value;
			}
			this.MinimumSize = new Size(
				800 + 2,
				480 + 28 + 2
			);
			this.ResizeEnd += (s, e) => Settings.WindowInformation.Value = this.GetWindowInformation();
			this.Move += (s, e) => Settings.WindowInformation.Value = this.GetWindowInformation();


			Logger.Register("MainLogger");

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
			this.Communicator = new WindowBrowserCommicator(
				this,
				this.WindowBrowser,
				async () => // After communicator initialized
				{
					{
						// Register to Logger
						Logger.Logged += e => this.Communicator.CallbackScript("Logged", e);

						// Logged before initialized
						string prevLog;
						while ((prevLog = Logger.Fetch("MainLogger")) != null)
							await this.Communicator.CallbackScript("Logged", prevLog);

						Logger.Unregister("MainLogger");
					}

					this.ClientSizeChanged += (s, e) => this.Communicator?.CallbackScript("WindowState", ((int)this.WindowState).ToString());

					this.GameBrowser = this.WindowBrowser.GetBrowser().GetFrame("MAIN_FRAME");
					// await this.Communicator.CallScript("window.INTERNAL.zoomMainFrame", "66.6666");
					await this.Communicator.CallScript("window.INTERNAL.loadMainFrame", Constants.GameURL);
				}
			);
			this.Communicator.RegisterObserveObject(nameof(Master), Master.Instance);
			this.Communicator.RegisterObserveObject(nameof(Homeport), Homeport.Instance);

			this.WindowBrowser.JavascriptObjectRepository.Register("API", this.Communicator, true, new CefSharp.BindingOptions { CamelCaseJavascriptNames = false });
			this.WindowBrowser.FrameLoadEnd += async (s, e) =>
			{
				var rootUri = Extensions.UriOrBlank(e.Browser.MainFrame?.Url);
				var frameUri = Extensions.UriOrBlank(e.Url);

				// Cookie patch
				if (frameUri.AbsoluteUri.Contains("/foreign/"))
				{
					Logger.Log("Foreign page detected, applying DMM cookie");
					await this.GameBrowser?.EvaluateScriptAsync(Constants.DMMCookie);

					this.GameBrowser?.LoadUrl(Constants.GameURL);
				}

				// CSS patch
				if (this.GameBrowser?.IsValid ?? false)
				{
					if (this.GameBrowser?.Url == Constants.GameURL)
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
							true;
						}else false;"
						).ToEvaluatableString();

						var ret = await this.GameBrowser?.EvaluateScriptAsync(script);
						if (ret.Success && (bool)ret.Result == true)
							Logger.Log("Game CSS applied");
					}
				}
			};
			this.WindowBrowser.LoadError += (s, e) =>
			{
				switch (e.ErrorCode) {
					case CefSharp.CefErrorCode.FileNotFound:
					case CefSharp.CefErrorCode.ConnectionFailed:
					case CefSharp.CefErrorCode.ConnectionRefused:
					case CefSharp.CefErrorCode.ConnectionTimedOut:
					case CefSharp.CefErrorCode.DisallowedUrlScheme:
					case CefSharp.CefErrorCode.InvalidResponse:
					case CefSharp.CefErrorCode.InvalidUrl:
					case CefSharp.CefErrorCode.NetworkAccessDenied:
					case CefSharp.CefErrorCode.OutOfMemory:
					case CefSharp.CefErrorCode.TooManyRedirects:
						e.Frame.LoadUrl("file:///" + Constants.EntryDir.Replace("\\", "/") + "/WindowFrame/internal/error.html");
						break;
				}
			};

			this.Resize += (s, e) => this.WindowBrowser.Size = this.ClientSize;
			this.WindowBrowser.Load("file:///" + Constants.EntryDir.Replace("\\", "/") + "/WindowFrame/Application.html");
			this.Controls.Add(this.WindowBrowser);

			this.WindowBrowser.IsBrowserInitializedChanged += (s, e) =>
			{
				if (e.IsBrowserInitialized)
				{
#if DEBUG
					this.WindowBrowser.GetBrowser().GetHost().ShowDevTools();
#endif
				}
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
