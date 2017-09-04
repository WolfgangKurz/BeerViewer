using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using BeerViewer.Framework;
using BeerViewer.Forms.Controls;
using BeerViewer.Models;

namespace BeerViewer.Forms.Controls.Fleets
{
	internal class FleetView : FrameworkContainer
	{
		public Fleet Fleet { get; protected set; }

		#region Initializers
		public FleetView() : base()
			=> this.Initialize();

		public FleetView(FrameworkRenderer Renderer) : base(Renderer)
			=> this.Initialize();

		public FleetView(int X, int Y) : base(X, Y)
			=> this.Initialize();

		public FleetView(FrameworkRenderer Renderer, int X, int Y) : base(Renderer, X, Y)
			=> this.Initialize();

		public FleetView(int X, int Y, int Width, int Height) : base(X, Y, Width, Height)
			=> this.Initialize();

		public FleetView(FrameworkRenderer Renderer, int X, int Y, int Width, int Height) : base(Renderer, X, Y, Width, Height)
			=> this.Initialize();
		#endregion

		public new void Initialize()
		{
			base.Initialize();
			this.Paint += this.OnPaint;
		}

		private void OnPaint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;

			this.Height = 30;

			if (this.Fleet == null)
			{

			}
			else
			{
				g.DrawString(
					string.Format("Lv: {0}", this.Fleet?.Ships?.Sum(s => s.Level).ToString() ?? "-"),
					Constants.fontDefault,
					Brushes.White,
					new Point(4, 4)
				);
			}
		}

		public void SetFleet(Fleet Fleet)
		{
			this.Fleet = Fleet;
			this.Invalidate();
		}
	}
}
