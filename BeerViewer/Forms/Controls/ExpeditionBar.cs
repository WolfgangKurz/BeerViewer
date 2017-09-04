using System;
using System.Drawing;
using System.Windows.Forms;

using BeerViewer.Framework;
using BeerViewer.Models;
using BeerViewer.Models.Raw;

namespace BeerViewer.Forms.Controls
{
	internal class ExpeditionBar : FrameworkControl
	{
		public Fleet Fleet { get; private set; }

		public ExpeditionBar() : base()
		{
			this.Initialize();
		}
		public ExpeditionBar(int X, int Y) : base(X, Y)
		{
			this.Initialize();
		}
		public ExpeditionBar(int X, int Y, int Width, int Height) : base(X, Y, Width, Height)
		{
			this.Initialize();
		}

		private void Initialize()
		{
			this.Paint += this.OnPaint;
		}

		private void DrawBitmapNumber(Graphics g, string Text, Point pos, bool OnCenter, bool UseWhite)
		{
			int p = 4, h = 7;
			int p2 = p + 1, hh = h / 2;
			var table = "0123456789[]:";
			int x = -p2 + pos.X - (OnCenter ? Text.Length * p2 / 2 : 0);
			int y = pos.Y - (OnCenter ? hh : 0);

			foreach(var c in Text)
			{
				x += (c==' ' ? 2 : p2);

				int offset = table.IndexOf(c);
				if (offset< 0) continue;

				g.DrawImage(
					Constants.BitmapNumber,
					new Rectangle(x, y, p, h),
					new Rectangle(offset * p, UseWhite ? h : 0, p, h),
					GraphicsUnit.Pixel
				);
			}
		}
		private void OnPaint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;

			if (this.Fleet != null)
			{
				var expedition = this.Fleet.Expedition;
				var progress = expedition.Progress;

				if (expedition.IsInExecution && progress.Maximum > 0)
				{
					g.FillRectangle(Constants.brushActiveFace, this.ClientBound);

					g.FillRectangle(
						Constants.brushBlueAccent,
						new Rectangle(0, 0, this.ClientBound.Width * progress.Current / progress.Maximum, this.ClientBound.Height)
					);
					this.DrawBitmapNumber(
						g,
						string.Format("[{0}] {1}", expedition.Id, expedition.RemainingText),
						new Point(this.ClientBound.Width / 2, this.ClientBound.Height / 2),
						true, true
					);
				}
				else
				{
					g.FillRectangle(Constants.brushHoverFace, this.ClientBound);
				}
			}
		}

		public void SetFleet(Fleet Fleet)
		{
			this.Fleet = Fleet;
			if (this.Fleet != null)
				this.Fleet.Expedition.PropertyEvent(nameof(this.Fleet.Expedition.Progress), () => this.Invalidate());

			this.Visible = this.Fleet != null;
			this.Invalidate();
		}
	}
}
