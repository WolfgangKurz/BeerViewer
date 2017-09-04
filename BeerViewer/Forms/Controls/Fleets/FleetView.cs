using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using BeerViewer.Framework;
using BeerViewer.Forms.Controls;
using BeerViewer.Models;
using BeerViewer.Models.Enums;

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
			var textColor = (this.Fleet == null ? Brushes.Gray : Brushes.White);

			var pX = (this.Width - 6 * 2) / 2;
			var pY = (60 - 2 * 2) / 3;

			g.Clear(Constants.colorNormalFace);
			#region Fleet Top Info
			{
				string[] texts;

				if ((this.Fleet?.Ships?.Length ?? 0) == 0)
				{
					texts = new string[]
					{
						$"{i18n.Current.fleet_level}: -",
						"-",
						$"{i18n.Current.fleet_los}: -",
						$"{i18n.Current.fleet_aa}: -",
						"-",
						"" // Empty for condition timer
					};
				}
				else
				{
					var levels = this.Fleet.Ships.Sum(x => x.Level);
					var speed = this.Fleet.Speed.ToLanguageString();
					var los = Math.Round(
						LOSCalcLogic.Get(Settings.LOSCalcType.Value)
							.Calc(new Fleet[] { this.Fleet }),
						2
					);
					var aa = string.Format(
						"{0}~{1}",
						this.Fleet.Ships.Sum(x => x.GetAirSuperiorityPotential(AirSuperiorityCalculationOptions.Minimum)),
						this.Fleet.Ships.Sum(x => x.GetAirSuperiorityPotential(AirSuperiorityCalculationOptions.Maximum))
					);

					texts = new string[]
					{
						$"{i18n.Current.fleet_level}: {levels}",
						$"{i18n.Current["fleet_speed_"+speed]}",
						$"{i18n.Current.fleet_los}: {los}",
						$"{i18n.Current.fleet_aa}: {aa}",
						"-", // Remaining time (condition or expedition)
						""
					};
				}

				for (var i = 0; i < 2; i++)
				{
					for (var j = 0; j < 2; j++)
					{
						var s = g.Save();

						g.SetClip(new Rectangle(6 + pX * i, 2 + pY * j, pX, pY));
						g.DrawString(
							texts[i + j * 2],
							Constants.fontDefault,
							textColor,
							new Point(6 + pX * i, 2 + pY * j)
						);

						g.Restore(s);
					}
				}
				g.DrawLine(
					Constants.penActiveFace,
					new Point(4, 40 - 1),
					new Point(this.Width - 4, 40 - 1)
				);
			}
			#endregion

			#region Fleet Info
			var bY = 40;
			if (this.Fleet != null)
			{
				var ships = this.Fleet.Ships;
				this.Height = 40 + ships.Length * 40 + 4 // Fleet info
					+ 72 * 2; // Repair & Construct dock;

				var nameWidth = ships
					.Max(x => (int)g.MeasureString(x.Info.Name, Constants.fontBig).Width);

				int i = 0;
				foreach (var ship in ships)
				{
					var rightWidth = (this.Width - 12 - nameWidth - 8);

					#region Name & Level
					g.DrawString(
						ship.Info.Name,
						Constants.fontBig,
						textColor,
						new Point(6, bY + 4 + i * 40)
					);

					g.DrawString(
						$"Lv. {ship.Level}",
						Constants.fontSmall,
						textColor,
						new Point(6, bY + 4 + i * 40 + 18)
					);
					#endregion

					#region HP (or state)
					DrawColorIndicator(
						g,
						6 + nameWidth + 8,
						bY + 4 + i * 40 + 4,
						rightWidth,
						6,
						ship.HP
					);

					if (ship.Situation.HasFlag(ShipSituation.Repair))
					{
						g.DrawString(
							i18n.Current.fleet_repairing,
							Constants.fontDefault,
							textColor,
							new Point(6 + nameWidth + 6, bY + 4 + i * 40 + 12)
						);
					}
					else
					{
						g.DrawString(
							$"{ship.HP.Current}/{ship.HP.Maximum}",
							Constants.fontDefault,
							textColor,
							new Point(6 + nameWidth + 6, bY + 4 + i * 40 + 12)
						);
					}
					#endregion

					#region Fuel & Ammo
					DrawColorIndicator(
						g,
						6 + nameWidth + 8 + rightWidth - (rightWidth / 4),
						bY + 4 + i * 40 + 4 + 8,
						rightWidth / 4, 6,
						ship.Fuel, 5
					);
					DrawColorIndicator(
						g,
						6 + nameWidth + 8 + rightWidth - (rightWidth / 4),
						bY + 4 + i * 40 + 4 + 8 + 8,
						rightWidth / 4, 6,
						ship.Bull, 5
					);
					#endregion

					#region Condition
					g.FillRectangle(
						new SolidBrush(GetConditionColor(ship.ConditionType)),
						6 + nameWidth + 8 + rightWidth - (rightWidth / 4) - 36,
						bY + 4 + i * 40 + 4 + 8 + 1,
						11, 11
					);
					g.DrawString(
						ship.Condition.ToString(),
						Constants.fontDefault,
						textColor,
						new Point(
							6 + nameWidth + 8 + rightWidth - (rightWidth / 4) - 24,
							bY + 4 + i * 40 + 4 + 8 + 1 - 3
						)
					);
					#endregion

					i++;
				}

				bY += ships.Length * 40;
			}
			else
				this.Height = 40 + 4 // Fleet info
					+ 72 * 2; // Repair & Construct dock

			g.DrawLine(
				Constants.penActiveFace,
				new Point(4, bY + 2 - 1),
				new Point(this.Width - 4, bY + 2 - 1)
			);
			bY += 4;
			#endregion

			#region Repair & Construct dock
			textColor = Brushes.White;

			var dockWidth = (this.Width - 18) / 2;
			for (var i = 0; i < 4; i++)
			{
				var dock = Homeport.Instance.Repairyard.Docks[i+1];

				if (dock?.State == RepairingDockState.Repairing)
				{
					g.FillRectangle(
						Constants.brushBlueAccent,
						6 + (dockWidth + 6) * (i % 2),
						bY + 6 + (i / 2) * 34,
						dockWidth, 28
					);
					g.DrawString(
						dock.Ship.Info.Name,
						Constants.fontDefault,
						textColor,
						new Point(
							6 + (dockWidth + 6) * (i % 2) + (dockWidth / 2),
							bY + 6 + (i / 2) * 34 - 1
						),
						new StringFormat { Alignment = StringAlignment.Center }
					);

					var txt = "00:00:00";
					if (dock.Remaining.HasValue)
						txt = $"{(int)dock.Remaining.Value.TotalHours:D2}:{dock.Remaining.Value.ToString(@"mm\:ss")}";

					g.DrawString(
						txt,
						Constants.fontDefault,
						textColor,
						new Point(
							6 + (dockWidth + 6) * (i % 2) + (dockWidth / 2),
							bY + 6 + (i / 2) * 34 - 1 + 14
						),
						new StringFormat { Alignment = StringAlignment.Center }
					);
				}
				else
				{
					g.FillRectangle(
						Constants.brushActiveFace,
						6 + (dockWidth + 6) * (i % 2),
						bY + 6 + (i / 2) * 34,
						dockWidth, 28
					);

					var txt = (
						(dock?.State ?? RepairingDockState.Locked) == RepairingDockState.Locked
						? i18n.Current.fleet_locked
						: i18n.Current.fleet_repair_empty
					) as string;
					var s = g.MeasureString(txt, Constants.fontDefault);

					g.DrawString(
						txt,
						Constants.fontDefault,
						textColor,
						new Point(
							6 + (dockWidth + 6) * (i % 2) + (dockWidth / 2) - (int)(s.Width / 2),
							bY + 6 + (i / 2) * 34 + 14 - (int)(s.Height / 2)
						)
					);
				}
			}
			bY += 72;

			for (var i = 0; i < 4; i++)
			{
				var dock = Homeport.Instance.Dockyard.Docks[i+1];

				if (dock?.State == BuildingDockState.Building || dock?.State == BuildingDockState.Completed)
				{
					g.FillRectangle(
						dock.State == BuildingDockState.Completed ? Constants.brushGreenAccent : Constants.brushBlueAccent,
						6 + (dockWidth + 6) * (i % 2),
						bY + 6 + (i / 2) * 34,
						dockWidth, 28
					);
					g.DrawString(
						dock.Ship.Name,
						Constants.fontDefault,
						textColor,
						new Point(
							6 + (dockWidth + 6) * (i % 2) + (dockWidth / 2),
							bY + 6 + (i / 2) * 34 - 1
						),
						new StringFormat { Alignment = StringAlignment.Center }
					);

					var txt = "00:00:00";
					if (dock.Remaining.HasValue)
						txt = $"{(int)dock.Remaining.Value.TotalHours:D2}:{dock.Remaining.Value.ToString(@"mm\:ss")}";

					g.DrawString(
						txt,
						Constants.fontDefault,
						textColor,
						new Point(
							6 + (dockWidth + 6) * (i % 2) + (dockWidth / 2),
							bY + 6 + (i / 2) * 34 - 1 + 14
						),
						new StringFormat { Alignment = StringAlignment.Center }
					);
				}
				else
				{
					g.FillRectangle(
						Constants.brushActiveFace,
						6 + (dockWidth + 6) * (i % 2),
						bY + 6 + (i / 2) * 34,
						dockWidth, 28
					);

					var txt = (
						(dock?.State ?? BuildingDockState.Locked) == BuildingDockState.Locked
							? i18n.Current.fleet_locked
							: i18n.Current.fleet_dock_empty
					) as string;
					var s = g.MeasureString(txt, Constants.fontDefault);

					g.DrawString(
						txt,
						Constants.fontDefault,
						textColor,
						new Point(
							6 + (dockWidth + 6) * (i % 2) + (dockWidth / 2) - (int)(s.Width / 2),
							bY + 6 + (i / 2) * 34 + 14 - (int)(s.Height / 2)
						)
					);
				}
			}
			bY += 72;
			#endregion
		}
		private void DrawColorIndicator(Graphics g, int X, int Y, int Width, int Height, LimitedValue Value, int Sep = 4)
		{
			Brush foreground;
			var p = GetPercent(Value);
			var pX = (double)Width / Sep;
			var hH = Height / 2;

			if (p <= 0.25) foreground = Constants.brushRedAccent;
			else if (p <= 0.5) foreground = Constants.brushOrangeAccent;
			else if (p <= 0.75) foreground = Constants.brushYellowAccent;
			else if (p < 1) foreground = Constants.brushGreenAccent;
			else foreground = Constants.brushDeepGreenAccent;

			var bound = new Rectangle(X, Y, Width, Height);
			var s = g.Save();

			g.SetClip(bound);
			g.FillRectangle(Constants.brushActiveFace, bound);
			g.FillRectangle(foreground, new Rectangle(X, Y, (int)(Width * p), Height));

			for (var i = 1; i < Sep; i++)
				g.DrawLine(
					Constants.penNormalFace,
					new Point(X + (int)(pX * i), Y),
					new Point(X + (int)(pX * i), Y + hH)
				);

			g.Restore(s);
		}
		private Color GetConditionColor(ConditionType c)
		{
			switch (c)
			{
				case ConditionType.Normal: return Color.White;
				case ConditionType.Tired: return FrameworkExtension.FromRgb(0xffcc80);
				case ConditionType.OrangeTired: return Constants.colorOrangeAccent;
				case ConditionType.RedTired: return Constants.colorRedAccent;
				case ConditionType.Brilliant: return Constants.colorYellowAccent;
			}
			return Constants.colorActiveFace;
		}
		private double GetPercent(LimitedValue Value)
		{
			var m = Value.Maximum - Value.Minimum;
			if (m == 0) return 0;
			return (double)(Value.Current - Value.Minimum) / m;
		}

		public void SetFleet(Fleet Fleet)
		{
			this.Fleet = Fleet;
			this.Fleet.PropertyEvent(nameof(this.Fleet.ShipsUpdated), () => this.Invalidate());

			this.Invalidate();
		}
	}
}
