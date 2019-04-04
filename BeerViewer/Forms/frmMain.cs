#define USE_GC
// #define USE_STARTUP_DEVTOOLS

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using BeerViewer.Framework;
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
		private bool OnShutdown { get; set; } = false;

		static frmMain()
		{
			frmMain.Instance = new frmMain();
		}
		private frmMain() : base()
		{
			InitializeComponent();

#if DEBUG
			this.Text = "βeerViewer 2.0";
#else
			this.Text = "BeerViewer 2.0";
#endif

			Logger.Register("MainLogger");

			// Master.Instance.Ready();
			// Homeport.Instance.Ready();

			this.FormClosed += (s, e) =>
			{
				OnShutdown = true;

				// Hide application first
				this.Hide();
				Application.DoEvents();

				this.Controls.Remove(this.WindowBrowser);

				Network.Proxy.Instance.Dispose();
			};

			/// Load <see cref="WindowInfo" /> settings
			{
				var info = (WindowInfo)Settings.WindowInformation;
				this.Size = new Size(info.Width, info.Height);
				if (info.Left.HasValue) this.Left = info.Left.Value;
				if (info.Top.HasValue) this.Top = info.Top.Value;
			}
			this.MinimumSize = new Size(
				(int)(1200 * 0.25) + 2,
				(int)(720 * 0.25) + 28 + 2
			);
			this.ResizeEnd += (s, e) =>
			{
				if (!OnShutdown) Settings.WindowInformation.Value = this.GetWindowInformation();
			};
			this.Move += (s, e) =>
			{
				if (!OnShutdown) Settings.WindowInformation.Value = this.GetWindowInformation();
			};

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
			// Components (Plugins)
			ComponentService.Instance.Initialize();
			#endregion

			#region WindowBrowser
			this.WindowBrowser = new FrameworkBrowser("")
			{
				Dock = DockStyle.None,
				AllowDrop = false,
				Location = Point.Empty
			};

			this.WindowBrowser.KeyDown += (s, e) =>
			{
				this.Communicator?.CallbackScript("GlobalKeyInput", e.KeyValue, (e.Control ? 1 : 0) | (e.Shift ? 2 : 0) | (e.Alt ? 4 : 0));
			};

			this.Communicator = new WindowBrowserCommicator(
				this,
				this.WindowBrowser,
				() => // After communicator initialized
				{
					Settings.LanguageCode.ValueChanged += async (s, e) => await this.Communicator.CallbackScript("i18n", Settings.LanguageCode.Value);

					this.ClientSizeChanged += (s, e) => this.Communicator?.CallbackScript("WindowState", (int)this.WindowState);
					this.Activated += (s, e) => this.Communicator?.CallbackScript("FocusState", true);
					this.Deactivate += (s, e) => this.Communicator?.CallbackScript("FocusState", false);

					var timer = new System.Timers.Timer(1000);
					timer.Elapsed += async (s, e) =>
					{
						if (this.GameBrowser == null || !this.GameBrowser.IsValid)
						{
							if (this.WindowBrowser.IsDisposed) return;
							if (this.WindowBrowser.GetBrowser().IsDisposed) return;

							this.GameBrowser = this.WindowBrowser.GetBrowser().GetFrame("MAIN_FRAME");
							await this.Communicator.CallbackScript("Game.Load", Constants.GameURL);

							// Register to Logger
							Logger.Logged += (f, a) => this.Communicator.CallbackScript("Logged", f, a);

							// Logged before initialized
							LogData prevLog;
							while ((prevLog = Logger.Fetch("MainLogger")) != null)
								await this.Communicator.CallbackScript("Logged", prevLog.Format, prevLog.Arguments);

							Logger.Unregister("MainLogger");

							timer.Enabled = false;
							timer = null;
						}
					};
					timer.Start();
				}
			);
			this.Communicator.MainFrameResized += (s, e) =>
				this.Invoke(() =>
					this.MinimumSize = new Size(
						e.Width + 2,
						e.Height + 28 + 2
					)
				);

			this.WindowBrowser.Cursor = Cursors.Cross;

			this.WindowBrowser.JavascriptObjectRepository.Register("API", this.Communicator, true, new CefSharp.BindingOptions { CamelCaseJavascriptNames = false });
			this.WindowBrowser.FrameLoadEnd += async (s, e) =>
			{
				if (this.WindowBrowser.IsDisposed) return;

				var rootUri = Extensions.UriOrBlank(e.Browser.MainFrame?.Url);
				var frameUri = Extensions.UriOrBlank(e.Url);

				if (this.GameBrowser != null && !this.GameBrowser.IsValid)
				{
					this.GameBrowser = this.WindowBrowser.GetBrowser().GetFrame("MAIN_FRAME");
					if (this.GameBrowser != null && !this.GameBrowser.IsValid)
					{
						Logger.Log("Failed to control game frame...");
						return;
					}
				}

				// Cookie patch
				if (frameUri.AbsoluteUri.Contains("/foreign/"))
				{
					Logger.Log("Foreign page detected, applying DMM cookie");
					await this.GameBrowser?.EvaluateScriptAsync(Constants.DMMCookie);

					this.GameBrowser?.LoadUrl(Constants.GameURL);
				}

				// Main page redirection (logout)
				if (new Regex(@"^https?://www\.dmm\.com/$").IsMatch(frameUri.AbsoluteUri))
				{
					Logger.Log("Main page detected, redirecting");
					await this.Communicator.CallbackScript("Game.Load", Constants.GameURL);

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
				switch (e.ErrorCode)
				{
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

			this.WindowBrowser.Load("file:///" + Constants.EntryDir.Replace("\\", "/") + "/WindowFrame/Application.html");
			// this.WindowBrowser.Load("chrome://version/");

			this.Resize += (s, e) => this.WindowBrowser.Size = this.ClientSize;
			this.Controls.Add(this.WindowBrowser);

#if DEBUG && USE_STARTUP_DEVTOOLS
			this.WindowBrowser.IsBrowserInitializedChanged += (s, e) =>
			{
				if (e.IsBrowserInitialized)
					this.WindowBrowser.GetBrowser().GetHost().ShowDevTools();
			};
#endif
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
