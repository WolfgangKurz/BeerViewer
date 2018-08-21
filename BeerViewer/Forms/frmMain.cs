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
using BeerViewer.Forms.Controls;
using BeerViewer.Forms.Controls.Overview;
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

		public FrameworkBrowser Browser { get; }
		public TextBox LogView { get; }

		static frmMain()
		{
			frmMain.Instance = new frmMain();
		}
		private frmMain() : base()
		{
			InitializeComponent();
			Master.Instance.Ready();
			Homeport.Instance.Ready();

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

			#region LogView
			this.LogView = new TextBox()
			{
				Location = new Point(1, 720 + 29),
				Size = new Size(1200, 80),

				Multiline = true,
				ScrollBars = ScrollBars.Vertical,
				ReadOnly = true
			};
			Logger.Logged += x =>
			{
				Action<string> f = y =>
				{
					this.LogView.Text = y + Environment.NewLine + this.LogView.Text;
					this.LogView.Update();
				};

				if (this.LogView.InvokeRequired)
					this.LogView.Invoke((Action)(() => f(x)));
				else
					f(x);
			};
			this.Resize += (s, e) =>
			{
				this.LogView.Size = new Size(1200, this.ClientSize.Height - 720 - 29);
			};
			this.Controls.Add(this.LogView);
			#endregion


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


			#region Menu Button rendering
			var MenuButton = new FrameworkControl(1, 1, 120, 28);
			MenuButton.Paint += (s, e) =>
			{
				var c = s as FrameworkControl;
				var g = e.Graphics;

				if (c.IsHover) g.FillRectangle(c.IsActive ? Constants.brushActiveFace : Constants.brushHoverFace, c.ClientBound);
				g.DrawImage(
					Properties.Resources.Menu_Button,
					new Rectangle(0, 0, 28, 28),
					new Rectangle(0, 0, 28, 28),
					GraphicsUnit.Pixel
				);
				g.DrawString(
					"BeerViewer 2.0",
					Constants.fontDefault,
					Brushes.White,
					new Point(28, 5)
				);
			};
			MenuButton.Click += (s, e) =>
			{
				// TODO: Open Menu
			};
			this.Renderer.AddControl(MenuButton);
			#endregion

			#region Browser
			this.Browser = new FrameworkBrowser("")
			{
				Location = new Point(1, 29),
				Size = new Size(1200, 720),
				Dock = DockStyle.None,
				AllowDrop = false,

				// To remove context menu
				MenuHandler = new NoMenuHandler()
			};
			this.Browser.FrameLoadEnd += async (s, e) =>
			{
				var rootUri = Extensions.UriOrBlank(e.Browser.MainFrame?.Url);
				var frameUri = Extensions.UriOrBlank(e.Url);

				// Cookie patch
				if (rootUri.Host == "www.dmm.com")
					await this.Browser.GetBrowser().MainFrame.EvaluateScriptAsync(Constants.DMMCookie);

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
					await this.Browser.GetBrowser().MainFrame.EvaluateScriptAsync(script);
				}
			};
			this.Browser.Load(Constants.GameURL);
			this.Controls.Add(this.Browser);
			#endregion

			#region Expedition bar
			var ExpeditionBars = new ExpeditionBar[3]
			{
				new ExpeditionBar(131, 8, 80, 14),
				new ExpeditionBar(221, 8, 80, 14),
				new ExpeditionBar(311, 8, 80, 14)
			};
			Homeport.Instance.Organization.PropertyEvent(nameof(Homeport.Instance.Organization.Fleets), () =>
			{
				var fleets = Homeport.Instance.Organization.Fleets;

				if (fleets.Count >= 2) ExpeditionBars[0].SetFleet(fleets[2]);
				if (fleets.Count >= 3) ExpeditionBars[1].SetFleet(fleets[3]);
				if (fleets.Count >= 4) ExpeditionBars[2].SetFleet(fleets[4]);
			});
			ExpeditionBars.ForEach(x => this.Renderer.AddControl(x));
			#endregion

			#region Resource bar
			{
				var bar = new ResourceBar(0, 5, 1, 18);
				bar.Resize += (s, e) => bar.X = this.ClientSize.Width - 96 - 7 - bar.Width;
				this.Resize += (s, e) => bar.X = this.MinimizeButton.X - 7 - bar.Width;
				this.Renderer.AddControl(bar);
			}
			#endregion

			#region Menu Name Rendering
			this.Paint += (s, e) =>
			{
				var g = e.Graphics;
				g.DrawString(
					"Overview",
					Constants.fontBig,
					Brushes.White,
					new Point(1200 + 8, 28 + 2)
				);
				g.DrawLine(
					Constants.penActiveFace,
					new Point(1200, 29 + 27),
					new Point(this.ClientSize.Width - 1, 29 + 27)
				);
			};
			#endregion

			#region Overview
			var Overview = new OverviewView(1200, 29 + 28, 1, 24);
			Homeport.Instance.Organization.PropertyEvent(nameof(Homeport.Instance.Organization.Fleets), () =>
			{
				var fleets = Homeport.Instance.Organization.Fleets;
				Overview.SetFleet(fleets.Select(_ => _.Value).ToArray());
			});
			this.Renderer.AddControl(Overview);

			this.Resize += (s, e) =>
			{
				Overview.Width = this.ClientSize.Width - 1200;
				Overview.MaximumHeight = this.ClientSize.Height - (29 + 28) + 1;
				// Overview.Height = this.ClientSize.Height - (29 + 28);
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
