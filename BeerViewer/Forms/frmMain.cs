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
using BeerViewer.Modules;
using BeerViewer.Modules.Communication;

using System.Runtime.InteropServices;

using Gecko;
using IFrameworkBrowserFrame = Gecko.DOM.GeckoIFrameElement;

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
				1200 + 2,
				720 + 28 + 2
			);
			this.ResizeEnd += (s, e) => Settings.WindowInformation.Value = this.GetWindowInformation();
			this.Move += (s, e) => Settings.WindowInformation.Value = this.GetWindowInformation();

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
				Location = Point.Empty,
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
						if (this.GameBrowser == null)
						{
							if (this.WindowBrowser.IsDisposed) return;

							this.GameBrowser = this.WindowBrowser.Document.GetElementById("MAIN_FRAME") as IFrameworkBrowserFrame;
							await this.Communicator.CallbackScript("Game.Load", Constants.GameURL);

							// Register to Logger
							Logger.Logged += (f, a) => this.Communicator.CallbackScript("Logged", f, a);

							// Logged before initialized
							LogData prevLog;
							while ((prevLog = Logger.Fetch("MainLogger")) != null)
								await this.Communicator.CallbackScript("Logged", prevLog.Format, prevLog.Arguments);

							Logger.Unregister("MainLogger");
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

			this.WindowBrowser.RegisterMessageListener(this.Communicator, "API");
			// this.WindowBrowser.JavascriptObjectRepository.Register("API", this.Communicator, true, new CefSharp.BindingOptions { CamelCaseJavascriptNames = false });
			this.WindowBrowser.Navigated += async (s, e) =>
			{
				if (this.WindowBrowser.IsDisposed) return;

				if (this.GameBrowser == null)
				{
					this.GameBrowser = this.WindowBrowser.Document.GetElementById("MAIN_FRAME") as IFrameworkBrowserFrame;
					if (this.GameBrowser == null)
					{
						Logger.Log("Failed to control game frame...");
						return;
					}
				}

				var frameUri = e.Uri;

				// Cookie patch
				if (frameUri.AbsoluteUri.Contains("/foreign/"))
				{
					Logger.Log("Foreign page detected, applying DMM cookie");
					this.GameBrowser?.EvaluateScriptAsync(Constants.DMMCookie);
					this.GameBrowser.Src = Constants.GameURL;
				}

				// Login welcome patch
				if (frameUri.AbsoluteUri.Contains("/login/"))
				{
					Logger.Log("Login page detected, applying Welcome patch");
					this.GameBrowser?.EvaluateScriptAsync(Constants.WelcomePatch);
				}

				// CSS patch
				if (this.GameBrowser != null)
				{
					if (this.GameBrowser?.ContentDocument.Location.Href == Constants.GameURL)
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

						var ret = this.GameBrowser?.EvaluateScriptAsync(script);
						if (ret.Success && (bool)ret.Result == true)
							Logger.Log("Game CSS applied");
					}
				}
			};
			this.WindowBrowser.NavigationError += (s, e) =>
			{
				e.DomWindow.Document.Location.Href = "file:///" + Constants.EntryDir.Replace("\\", "/") + "/WindowFrame/internal/error.html";
			};

			this.WindowBrowser.LoadUrl("file:///" + Constants.EntryDir.Replace("\\", "/") + "/WindowFrame/Application.html");
			// this.WindowBrowser.Load("chrome://version/");

			this.Resize += (s, e) => this.WindowBrowser.Size = this.ClientSize;
			this.Controls.Add(this.WindowBrowser);
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
