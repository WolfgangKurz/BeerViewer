using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using BeerViewer.Framework;
using BeerViewer.Forms.Controls;
using BeerViewer.Models;

namespace BeerViewer.Forms.Controls.Overview
{
	internal class OverviewView : FrameworkContainer
	{
		public SimpleFleetView[] Fleets { get; protected set; }
		public DockView Docks { get; protected set; }
		public QuestsView Quests { get; protected set; }

		private TabHost FleetTabHost { get; set; }

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
			this.Fleets = new SimpleFleetView[4]
				.Select(_ => new SimpleFleetView(0, 24, 400, 2) { Visible = false })
				.ToArray();

			Fleets.ForEach(f =>
			{
				f.Invalidated += (s, e) => this.AdjustSize();
				f.Resize += (s, e) => this.AdjustSize();
				this.AddControl(f);
			});
			Fleets.First().Visible = true;
			#endregion

			#region Fleet TabHost
			FleetTabHost = new TabHost(0, 0, 1, 24);
			FleetTabHost.AddTab(new TabHost.TabItem("I", Fleets[0]));
			FleetTabHost.AddTab(new TabHost.TabItem("II", Fleets[1]));
			FleetTabHost.AddTab(new TabHost.TabItem("III", Fleets[2]));
			FleetTabHost.AddTab(new TabHost.TabItem("IV", Fleets[3]));
			this.AddControl(FleetTabHost);

			this.FleetTabHost.TabIndexChanged += (s, e) =>
			{
				var fv = this.Fleets.FirstOrDefault(x => x.Visible);
				if (fv == null) return;

				this.AdjustSize();
			};
			#endregion

			#region DockView
			this.Docks = new DockView(0, 0, 1, 1);
			this.Docks.Invalidated += (s, e) => this.AdjustSize();
			this.AddControl(this.Docks);
			#endregion

			#region QuestsView
			this.Quests = new QuestsView(0, 0, 1, 1);
			this.Quests.Invalidated += (s, e) => this.AdjustSize();
			this.AddControl(this.Quests);
			#endregion

			this.Resize += (s, e) => this.AdjustSize();
			this.Paint += this.OnPaint;

			this.FleetTabHost.OnTabIndexChanged();
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

		private void AdjustSize()
		{
			if (this.FleetTabHost != null)
				this.FleetTabHost.Width = this.ClientWidth;

			if (this.Fleets != null)
				this.Fleets.ForEach(f => f.Width = this.ClientWidth);

			if (this.Docks != null)
			{
				var fv = this.Fleets?.FirstOrDefault(x => x.Visible);

				this.Docks.Width = this.ClientWidth;
				this.Docks.Y = (fv?.Y + fv?.Height) ?? 0;
			}

			if (this.Quests != null)
			{
				this.Quests.Width = this.ClientWidth;
				this.Quests.Y = (this.Docks?.Y + this.Docks?.Height) ?? 0;
			}

			this.Height = 24 // Tab height
				+ (this.Fleets?.FirstOrDefault(x => x.Visible)?.Height ?? 0) // FleetView height
				+ (this.Docks?.Height ?? 0) // Docks height
				+ (this.Quests?.Height ?? 0); // Quests height
		}
	}
}
