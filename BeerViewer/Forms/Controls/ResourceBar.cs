using System;
using System.Drawing;
using System.Windows.Forms;

using BeerViewer.Framework;
using BeerViewer.Models;
using BeerViewer.Models.Raw;

namespace BeerViewer.Forms.Controls
{
	internal class ResourceBar : FrameworkControl
	{
		#region Initializers
		public ResourceBar() : base()
			=> this.Initialize();

		public ResourceBar(FrameworkRenderer Renderer) : base(Renderer)
			=> this.Initialize();

		public ResourceBar(int X, int Y) : base(X, Y)
			=> this.Initialize();

		public ResourceBar(FrameworkRenderer Renderer, int X, int Y) : base(Renderer, X, Y)
			=> this.Initialize();

		public ResourceBar(int X, int Y, int Width, int Height) : base(X, Y, Width, Height)
			=> this.Initialize();

		public ResourceBar(FrameworkRenderer Renderer, int X, int Y, int Width, int Height) : base(Renderer, X, Y, Width, Height)
			=> this.Initialize();
		#endregion

		private void Initialize()
		{
			var materials = Homeport.Instance.Materials;
			materials.PropertyEvent(nameof(materials.Fuel), () => this.Invalidate());
			materials.PropertyEvent(nameof(materials.Ammo), () => this.Invalidate());
			materials.PropertyEvent(nameof(materials.Steel), () => this.Invalidate());
			materials.PropertyEvent(nameof(materials.Bauxite), () => this.Invalidate());
			materials.PropertyEvent(nameof(materials.ImproveMaterials), () => this.Invalidate());

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
			var admiral = Homeport.Instance.Admiral;
			var materials = Homeport.Instance.Materials;
			var g = e.Graphics;

			var imgs = new Image[]
			{
				BeerViewer.Properties.Resources.icon_fuel,
				BeerViewer.Properties.Resources.icon_ammo,
				BeerViewer.Properties.Resources.icon_steel,
				BeerViewer.Properties.Resources.icon_bauxite,
				BeerViewer.Properties.Resources.icon_bucket,
				BeerViewer.Properties.Resources.icon_screw
			};
			var values = new int[]
			{
				materials.Fuel,
				materials.Ammo,
				materials.Steel,
				materials.Bauxite,
				materials.RepairBuckets,
				materials.ImproveMaterials
			};
			var len = Math.Min(values.Length, imgs.Length);

			var x = 0;
			for (var i = 0; i < len; i++)
			{
				var img = imgs[i];
				var value = values[i];

				g.DrawImage(
					img,
					new Rectangle(x, this.Height / 2 - img.Height / 2, img.Width, img.Height),
					new Rectangle(0, 0, img.Width, img.Height),
					GraphicsUnit.Pixel
				);
				x += img.Width + 4;

				x += this.DrawText(
					g,
					value.ToString(),
					new Point(x, this.ClientBound.Height / 2 - 8),
					false,
					((i < 4) && (value < admiral?.ResourceLimit))
						? Constants.brushYellowAccent
						: Brushes.White
				) + 12;
			}
			this.Width = Math.Max(1, x);
		}
	}
}
