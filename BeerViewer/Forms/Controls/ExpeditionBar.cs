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

		#region Initializers
		public ExpeditionBar() : base()
			=> this.Initialize();

		public ExpeditionBar(FrameworkRenderer Renderer) : base(Renderer)
			=> this.Initialize();

		public ExpeditionBar(int X, int Y) : base(X, Y)
			=> this.Initialize();

		public ExpeditionBar(FrameworkRenderer Renderer, int X, int Y) : base(Renderer, X, Y)
			=> this.Initialize();

		public ExpeditionBar(int X, int Y, int Width, int Height) : base(X, Y, Width, Height)
			=> this.Initialize();

		public ExpeditionBar(FrameworkRenderer Renderer, int X, int Y, int Width, int Height) : base(Renderer, X, Y, Width, Height)
			=> this.Initialize();
		#endregion

		private void Initialize()
		{
			this.Paint += this.OnPaint;
		}

		private int DrawText(Graphics g, string Text, Point pos, bool OnCenter = false, Brush ColorBrush = null)
		{
			var font = Constants.fontDefault;
			var ms = g.MeasureString(Text, font);
			g.DrawString(
				Text,
				font,
				ColorBrush ?? Brushes.White,
				OnCenter ? new Point(pos.X - (int)(ms.Width / 2), pos.Y - (int)(font.Size / 2)) : pos
			);
			return (int)ms.Width;
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
					this.DrawText(
						g,
						string.Format("[{0}] {1}", expedition.Id, expedition.RemainingText),
						new Point(this.ClientBound.Width / 2, this.ClientBound.Height / 2),
						true,
						Brushes.White
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
