using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using BeerViewer.Framework;
using BeerViewer.Forms.Controls.Fleets;
using BeerViewer.Models;
using BeerViewer.Models.Enums;

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

			Fleets.ForEach(f =>
			{
				f.Invalidated += (s, e) =>
				{
					var fv = s as FleetView;
					if (!fv.Visible) return;

					this.Height = 24 // Tab height
						+ fv.Height // FleetView height
						+ 72 * 2; // Dock info height
				};
				this.AddControl(f);
			});

			this.Resize += (s, e) =>
			{
				FleetTabHost.Width = this.Width;
				Fleets.ForEach(f => f.Width = this.Width);
			};
			#endregion

			#region PropertyEventListener
			{
				var dockyard = Homeport.Instance.Dockyard;
				var repairyard = Homeport.Instance.Repairyard;

				dockyard.PropertyEvent(nameof(dockyard.Docks), () =>
				{
					dockyard.Docks?.ForEach(x =>
					{
						var dock = x.Value;
						if (dock == null) return;

						dock.PropertyEvent(nameof(dock.Remaining), () => this.Invalidate());
						dock.PropertyEvent(nameof(dock.State), () => this.Invalidate());
						dock.PropertyEvent(nameof(dock.Ship), () => this.Invalidate());
					});
					this.Invalidate();
				}, true);
				repairyard.PropertyEvent(nameof(repairyard.Docks), () =>
				{
					repairyard.Docks?.ForEach(x =>
					{
						var dock = x.Value;
						if (dock == null) return;

						dock.PropertyEvent(nameof(dock.Remaining), () => this.Invalidate());
						dock.PropertyEvent(nameof(dock.State), () => this.Invalidate());
						dock.PropertyEvent(nameof(dock.Ship), () => this.Invalidate());
					});
					this.Invalidate();
				}, true);
			}
			#endregion

			this.Paint += this.OnPaint;
		}

		private void OnPaint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;

			var bY = 24
				+ this.Fleets.FirstOrDefault(x => x.Visible).Height;

			#region Repair & Construct dock
			Brush textColor;

			var dockWidth = this.Width - 12;
			if (dockWidth < 200) dockWidth = (dockWidth - 0) / 1;
			else if (dockWidth < 400) dockWidth = (dockWidth - 4) / 2;
			else dockWidth = (dockWidth - 12) / 4;
			dockWidth = Math.Max(dockWidth, 1);

			var perLine = Math.Max(1, (this.Width - 12) / dockWidth);
			var lines = (int)Math.Ceiling(4.0 / perLine);

			for (var i = 0; i < 4; i++)
			{
				var line = i / perLine;
				var dock = Homeport.Instance.Repairyard.Docks[i + 1];
				textColor = Brushes.White;

				var _s = g.Save();
				g.SetClip(new Rectangle(
					6 + (dockWidth + 4) * (i % perLine),
					bY + 6 + line * 32,
					dockWidth, 28
				));

				if (dock?.State == RepairingDockState.Repairing)
				{
					var name = i18n.Current[dock.Ship.Info.Name];
					g.FillRectangle(
						Constants.brushBrownAccent,
						6 + (dockWidth + 4) * (i % perLine),
						bY + 6 + line * 32,
						dockWidth, 28
					);

					var txt = i18n.Current.fleet_done;
					if (dock.Remaining.HasValue)
						txt = $"{(int)dock.Remaining.Value.TotalHours:D2}:{dock.Remaining.Value.ToString(@"mm\:ss")}";

					var slWidth = (int)g.MeasureString($"{name}  {txt}", Constants.fontDefault).Width;
					if (slWidth >= dockWidth) // Double Line
					{
						g.DrawString(
							name,
							Constants.fontDefault,
							textColor,
							new Point(
								6 + (dockWidth + 4) * (i % perLine) + (dockWidth / 2),
								bY + 6 + line * 32 - 1
							),
							new StringFormat { Alignment = StringAlignment.Center }
						);
						g.DrawString(
							txt,
							Constants.fontDefault,
							textColor,
							new Point(
								6 + (dockWidth + 4) * (i % perLine) + (dockWidth / 2),
								bY + 6 + line * 32 - 1 + 14
							),
							new StringFormat { Alignment = StringAlignment.Center }
						);
					}
					else // Single Line
					{
						g.DrawString(
							$"{name}  {txt}",
							Constants.fontDefault,
							textColor,
							new Point(
								6 + (dockWidth + 4) * (i % perLine) + (dockWidth / 2),
								bY + 6 + line * 32 + 14 - 8
							),
							new StringFormat { Alignment = StringAlignment.Center }
						);
					}
				}
				else
				{
					Brush face;
					if ((dock?.State ?? RepairingDockState.Locked) == RepairingDockState.Locked)
					{
						face = Constants.brushHoverFace;
						textColor = Brushes.Gray;
					}
					else face = Constants.brushActiveFace;

					g.FillRectangle(
						face,
						6 + (dockWidth + 4) * (i % perLine),
						bY + 6 + line * 32,
						dockWidth, 28
					);

					var txt = (
						(dock?.State ?? RepairingDockState.Locked) == RepairingDockState.Locked
							? i18n.Current.fleet_locked
							: i18n.Current.fleet_repair_empty
					) as string;

					g.DrawString(
						txt,
						Constants.fontDefault,
						textColor,
						new Point(
							6 + (dockWidth + 4) * (i % perLine) + (dockWidth / 2),
							bY + 6 + line * 32 + 14 - 8
						),
						new StringFormat { Alignment = StringAlignment.Center }
					);
				}

				g.Restore(_s);
			}
			bY += 6 + 6 + lines * 32 - 4;

			for (var i = 0; i < 4; i++)
			{
				var line = i / perLine;
				var dock = Homeport.Instance.Dockyard.Docks[i + 1];
				textColor = Brushes.White;

				var _s = g.Save();
				g.SetClip(new Rectangle(
					6 + (dockWidth + 4) * (i % perLine),
					bY + 6 + line * 32,
					dockWidth, 28
				));

				if (dock?.State == BuildingDockState.Building || dock?.State == BuildingDockState.Completed)
				{
					var name = i18n.Current[dock.Ship.Name];
					g.FillRectangle(
						dock.State == BuildingDockState.Completed
							? Constants.brushGreenAccent
							: Constants.brushBlueAccent,
						6 + (dockWidth + 4) * (i % perLine),
						bY + 6 + line * 32,
						dockWidth, 28
					);

					var txt = i18n.Current.fleet_done;
					if (dock.Remaining.HasValue)
						txt = $"{(int)dock.Remaining.Value.TotalHours:D2}:{dock.Remaining.Value.ToString(@"mm\:ss")}";

					var slWidth = (int)g.MeasureString($"{name}  {txt}", Constants.fontDefault).Width;
					if (slWidth >= dockWidth) // Double Line
					{
						g.DrawString(
							name,
							Constants.fontDefault,
							textColor,
							new Point(
								6 + (dockWidth + 4) * (i % perLine) + (dockWidth / 2),
								bY + 6 + line * 32 - 1
							),
							new StringFormat { Alignment = StringAlignment.Center }
						);
						g.DrawString(
							txt,
							Constants.fontDefault,
							textColor,
							new Point(
								6 + (dockWidth + 4) * (i % perLine) + (dockWidth / 2),
								bY + 6 + line * 32 - 1 + 14
							),
							new StringFormat { Alignment = StringAlignment.Center }
						);
					}
					else // Single Line
					{
						g.DrawString(
							$"{name}  {txt}",
							Constants.fontDefault,
							textColor,
							new Point(
								6 + (dockWidth + 4) * (i % perLine) + (dockWidth / 2),
								bY + 6 + line * 32 + 14 - 8
							),
							new StringFormat { Alignment = StringAlignment.Center }
						);
					}
				}
				else
				{
					Brush face;
					if ((dock?.State ?? BuildingDockState.Locked) == BuildingDockState.Locked)
					{
						face = Constants.brushHoverFace;
						textColor = Brushes.Gray;
					}
					else face = Constants.brushActiveFace;

					g.FillRectangle(
						face,
						6 + (dockWidth + 4) * (i % perLine),
						bY + 6 + line * 32,
						dockWidth, 28
					);

					var txt = (
						(dock?.State ?? BuildingDockState.Locked) == BuildingDockState.Locked
							? i18n.Current.fleet_locked
							: i18n.Current.fleet_dock_empty
					) as string;

					g.DrawString(
						txt,
						Constants.fontDefault,
						textColor,
						new Point(
							6 + (dockWidth + 4) * (i % perLine) + (dockWidth / 2),
							bY + 6 + line * 32 + 14 - 8
						),
						new StringFormat { Alignment = StringAlignment.Center }
					);
				}

				g.Restore(_s);
			}
			bY += 6 + 6 + lines * 32 - 4;
			#endregion
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
