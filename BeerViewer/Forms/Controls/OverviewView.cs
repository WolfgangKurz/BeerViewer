using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using BeerViewer.Framework;
using BeerViewer.Forms.Controls.Fleets;
using BeerViewer.Models;

namespace BeerViewer.Forms.Controls
{
	internal class OverviewView : FrameworkContainer
	{
		public FleetView[] Fleets { get; protected set; }

		#region Initializers
		public OverviewView() : base()
			=> this.Initialize();

		public OverviewView(FrameworkRenderer Renderer) : base(Renderer)
			=> this.Initialize();

		public OverviewView(int X, int Y) : base(X, Y)
			=> this.Initialize();

		public OverviewView(FrameworkRenderer Renderer, int X, int Y) : base(Renderer, X, Y)
			=> this.Initialize();

		public OverviewView(int X, int Y, int Width, int Height) : base(X, Y, Width, Height)
			=> this.Initialize();

		public OverviewView(FrameworkRenderer Renderer, int X, int Y, int Width, int Height) : base(Renderer, X, Y, Width, Height)
			=> this.Initialize();
		#endregion

		public new void Initialize()
		{
			base.Initialize();

			#region Fleets
			this.Fleets = new FleetView[4]
				.Select(_ => new FleetView(0, 24, 400, 2) { Visible = false })
				.ToArray();

			Fleets.First().Visible = true;
			#endregion

			#region Fleet TabHost
			var FleetTabHost = new TabHost(0, 0, 400, 24);
			FleetTabHost.AddTab(new TabHost.TabItem("I", Fleets[0]));
			FleetTabHost.AddTab(new TabHost.TabItem("II", Fleets[1]));
			FleetTabHost.AddTab(new TabHost.TabItem("III", Fleets[2]));
			FleetTabHost.AddTab(new TabHost.TabItem("IV", Fleets[3]));
			this.AddControl(FleetTabHost);

			Fleets.ForEach(f => f.Invalidated += (s, e) =>
			{
				var fv = s as FleetView;
				if (!fv.Visible) return;

				this.Height = 24 + fv.Height;
			});

			this.Resize += (s, e) =>
			{
				FleetTabHost.Width = this.Width;
				Fleets.ForEach(f => f.Width = this.Width);
			};
			#endregion

			this.Paint += this.OnPaint;
		}

		private void OnPaint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;
		}

		public void SetFleet(int Index, Fleet Fleet)
		{
			this.Fleets[Index].SetFleet(Fleet);
			this.Invalidate();
		}
		public void SetFleet(Fleet[] Fleets)
		{
			int i = 0;
			foreach (var fleet in Fleets)
				this.SetFleet(i++, fleet);
		}
	}
}
