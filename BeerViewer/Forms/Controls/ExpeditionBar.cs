using System.Drawing;

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
			this.Paint += (s, e) =>
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
						g.DrawString(
							string.Format("[{0}] {1}", expedition.Id, expedition.RemainingText),
							Constants.fontSmall,
							Brushes.White,
							new Point(this.ClientBound.Width / 2, -1),
							new StringFormat { Alignment = StringAlignment.Center }
						);
					}
					else
					{
						g.FillRectangle(Constants.brushHoverFace, this.ClientBound);
					}
				}
			};
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
