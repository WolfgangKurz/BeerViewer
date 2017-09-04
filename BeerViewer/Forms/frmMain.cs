using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BeerViewer.Framework;
using BeerViewer.Forms.Controls;
using BeerViewer.Forms.Controls.Fleets;
using BeerViewer.Models;

namespace BeerViewer.Forms
{
	public partial class frmMain : BorderlessWindow
	{
		public static frmMain Instance { get; }

		public WebBrowser Browser { get; private set; }

		static frmMain()
		{
			frmMain.Instance = new frmMain();
		}
		private frmMain() : base()
		{
			InitializeComponent();
			Master.Instance.Ready();
			Homeport.Instance.Ready();

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
			this.Browser = new WebBrowser()
			{
				Location = new Point(1, 29),
				Size = new Size(800, 480),

				ScriptErrorsSuppressed = true,
				AllowWebBrowserDrop = false,
				IsWebBrowserContextMenuEnabled = false,

				Url = new Uri(Constants.GameURL)
			};
			this.Browser.Navigated += (s, e) =>
			{
				// Cookie patch
				if (e.Url.Host == "www.dmm.com")
					this.Browser.Document.InvokeScript("eval", new object[] { Constants.DMMCookie });

				// CSS patch
				if (e.Url.AbsoluteUri == Constants.GameURL)
				{
					var script = string.Format(
						"document.addEventListener('DOMContentLoaded', function(){{ var x=document.createElement('style');x.type='text/css';x.innerHTML='{0}';document.body.appendChild(x); }});",
						Constants.UserStyleSheet
					);
					this.Browser.Document.InvokeScript("eval", new object[] { script });
				}
			};

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

			#region Menu Name Rendering
			this.Paint += (s, e) =>
			{
				var g = e.Graphics;
				g.DrawString(
					"Overview",
					Constants.fontBig,
					Brushes.White,
					new Point(801 + 12, 28 + 2)
				);
				g.DrawLine(
					Constants.penActiveFace,
					new Point(801, 29 + 27),
					new Point(this.ClientSize.Width - 2, 29 + 27)
				);
			};
			#endregion

			#region Overview
			var Overview = new OverviewView(801, 29 + 28, 400, 24);
			Homeport.Instance.Organization.PropertyEvent(nameof(Homeport.Instance.Organization.Fleets), () =>
			{
				var fleets = Homeport.Instance.Organization.Fleets;
				Overview.SetFleet(fleets.Select(_ => _.Value).ToArray());
			});
			this.Renderer.AddControl(Overview);
			#endregion
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
