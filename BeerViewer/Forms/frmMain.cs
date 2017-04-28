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
					this.Font,
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
