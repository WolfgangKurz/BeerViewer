using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using BeerViewer.Framework;

namespace BeerViewer.Forms
{
	public partial class frmMain : BorderlessWindow
	{
		private ChromiumWebBrowser browser;

		protected Rectangle MenuButtonRectangle => new Rectangle(0, 0, 120, 28);

		public frmMain() : base()
		{
			InitializeComponent();

			this.BackColor = FrameworkExtension.FromRgb(0x222225);

			#region Menu Button rendering
			var RenderInvalidate = new Action(() =>
			{
				var w = this.ClientSize.Width;
				this.Invalidate(new Rectangle(0, 0, 120, 28));
			});
			var ProcButton = new Action<Point, Point>((x, y) =>
			{
				if (MenuButtonRectangle.Contains(x) && MenuButtonRectangle.Contains(y))
					; // TODO: Open Menu
			});
			Point ptMouseDown = Point.Empty;

			this.MouseMove += (s, e) => RenderInvalidate();
			this.MouseLeave += (s, e) => RenderInvalidate();

			this.MouseDown += (s, e) =>
			{
				ptMouseDown = e.Location;
				RenderInvalidate();
			};
			this.MouseUp += (s, e) =>
			{
				RenderInvalidate();
				ProcButton(ptMouseDown, e.Location);
			};
			#endregion

			this.Paint += frmMain_Paint;
		}

		private void frmMain_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;
			RenderMenuButton(g);
		}

		private void RenderMenuButton(Graphics g)
		{
			bool focus = false;
			bool active = false;

			var pt = this.PointToClient(Cursor.Position);
			if (pt.Y >= 0 && pt.Y < 28)
			{
				focus = MenuButtonRectangle.Contains(pt);
				active = (MouseButtons == MouseButtons.Left);
			}

			var hoverBrush = new SolidBrush(FrameworkExtension.FromRgb(0x313131));
			var activeBrush = new SolidBrush(FrameworkExtension.FromRgb(0x575757));

			if (focus)
				g.FillRectangle(active ? activeBrush : hoverBrush, new Rectangle(0, 0, 120, 28));

			// Icon
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
		}

		public class GameLoaderMessageHandler : IRenderProcessMessageHandler
		{
			void IRenderProcessMessageHandler.OnContextCreated(IWebBrowser browserControl, IBrowser browser, IFrame frame)
			{
				if (frame.Url == Constants.StartupPage)
					frame.ExecuteJavaScriptAsync(Constants.GameURI);
			}

			public void OnContextReleased(IWebBrowser browserControl, IBrowser browser, IFrame frame) { }
			public void OnFocusedNodeChanged(IWebBrowser browserControl, IBrowser browser, IFrame frame, IDomNode node) { }
		}


		private void frmMain_Load(object sender, EventArgs e)
		{
			browser = new ChromiumWebBrowser("")
			{
				Dock = DockStyle.None,
				Location = new Point(1, 28),
				Size = new Size(800, 480)
			};
			browser.RenderProcessMessageHandler = new GameLoaderMessageHandler();
			browser.MenuHandler = new EmptyMenuHandler();
			browser.Load(Constants.StartupPage);
			this.Controls.Add(browser);
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
