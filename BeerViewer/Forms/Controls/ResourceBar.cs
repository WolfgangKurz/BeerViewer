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

		private int DrawBitmapNumber(Graphics g, string Text, Point pos, bool OnCenter, int ColorIndex = 0)
		{
			int p = 4, h = 7;
			int p2 = p + 2, hh = h / 2;
			var table = "0123456789[]:";
			int x = -p2 + pos.X - (OnCenter ? Text.Length * p2 / 2 : 0);
			int y = pos.Y - (OnCenter ? hh : 0);

			foreach (var c in Text)
			{
				x += (c == ' ' ? 2 : p2);

				int offset = table.IndexOf(c);
				if (offset < 0) continue;

				g.DrawImage(
					Constants.BitmapNumber,
					new Rectangle(x, y, p, h),
					new Rectangle(offset * p, h * ColorIndex, p, h),
					GraphicsUnit.Pixel
				);
			}
			return x - pos.X;
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

				x += this.DrawBitmapNumber(
					g,
					value.ToString(),
					new Point(x, this.ClientBound.Height / 2 - 3),
					false,
					((i < 4) && (value < admiral?.ResourceLimit)) ? 2 : 1
				) + 12;
			}
			this.Width = Math.Max(1, x);
		}
	}
}
