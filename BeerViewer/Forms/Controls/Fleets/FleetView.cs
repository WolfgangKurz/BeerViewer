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

			var pY = 18;
			var bY = 0;

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

				var itemWidth = this.Width - 12;
				if (itemWidth < 180) itemWidth = (itemWidth - 0) / 1;
				else itemWidth = (itemWidth - 4) / 2;
				itemWidth = Math.Max(itemWidth, 1);

				var perLine = Math.Max(1, (this.Width - 12) / itemWidth);
				var lines = (int)Math.Ceiling((double)texts.Length / perLine);

				for (var i = 0; i < texts.Length; i++) { 
					var s = g.Save();

					g.SetClip(new Rectangle(
						6 + itemWidth * (i % perLine),
						2 + pY * (i / perLine),
						itemWidth, pY
					));
					g.DrawString(
						texts[i],
						Constants.fontDefault,
						textColor,
						new Point(
							6 + itemWidth * (i % perLine),
							2 + pY * (i / perLine)
						)
					);

					g.Restore(s);
				}

				bY += lines * 20;
				g.DrawLine(
					Constants.penActiveFace,
					new Point(4, bY - 1),
					new Point(this.Width - 4, bY - 1)
				);
			}
			#endregion

			#region Fleet Info
			if (this.Fleet != null)
			{
				var ships = this.Fleet.Ships;

				var nameWidth = ships
					.Max(x => (int)g.MeasureString(x.Info.Name, Constants.fontBig).Width);

				var itemWidth = this.Width - 12;
				if (itemWidth < 240) itemWidth = (itemWidth - 0) / 1;
				else if (itemWidth < 480) itemWidth = (itemWidth - 4) / 2;
				else itemWidth = (itemWidth - 8) / 3;
				itemWidth = Math.Max(itemWidth, 1);

				var rightWidth = (itemWidth - nameWidth - 8);
				var miniMode = (rightWidth < 156);
				var miniMode2 = (rightWidth < 84);

				var perLine = Math.Max(1, (this.Width-12) / itemWidth);
				var lines = (int)Math.Ceiling((double)ships.Length / perLine);

				this.Height = bY + lines * 36 + 6; // Fleet info

				int i = 0;
				foreach (var ship in ships)
				{
					#region Name & Level
					g.DrawString(
						ship.Info.Name,
						Constants.fontBig,
						textColor,
						new Point(
							6 + (itemWidth + 4) * (i % perLine),
							bY + 4 + (i / perLine) * 36
						)
					);

					g.DrawString(
						$"Lv. {ship.Level}",
						Constants.fontSmall,
						textColor,
						new Point(
							6 + (itemWidth + 4) * (i % perLine),
							bY + 4 + (i / perLine) * 36 + 18
						)
					);
					#endregion

					#region HP (or state)
					DrawColorIndicator(
						g,
						6 + (itemWidth + 4) * (i % perLine) + nameWidth + 8,
						bY + 4 + (i / perLine) * 36 + 4,
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
							new Point(
								6 + (itemWidth + 4) * (i % perLine) + nameWidth + 6,
								bY + 4 + (i / perLine) * 36 + 12
							)
						);
					}
					else
					{
						g.DrawString(
							$"{ship.HP.Current}/{ship.HP.Maximum}",
							Constants.fontDefault,
							textColor,
							new Point(
								6 + (itemWidth + 4) * (i % perLine) + nameWidth + 6,
								bY + 4 + (i / perLine) * 36 + 12
							)
						);
					}
					#endregion

					#region Fuel & Ammo
					if (!miniMode)
					{
						DrawColorIndicator(
							g,
							6 + (itemWidth + 4) * (i % perLine) + nameWidth + 8 + rightWidth - (rightWidth / 4),
							bY + 4 + (i / perLine) * 36 + 4 + 11,
							rightWidth / 4, 5,
							ship.Fuel, 5
						);
						DrawColorIndicator(
							g,
							6 + (itemWidth + 4) * (i % perLine) + nameWidth + 8 + rightWidth - (rightWidth / 4),
							bY + 4 + (i / perLine) * 36 + 4 + 11 + 7,
							rightWidth / 4, 5,
							ship.Bull, 5
						);
					}
					#endregion

					#region Condition
					if (miniMode2)
					{
						g.FillRectangle(
							new SolidBrush(GetConditionColor(ship.ConditionType)),
							6 + (itemWidth + 4) * (i % perLine) + nameWidth + 8 + rightWidth - (miniMode ? 0 : rightWidth / 4) - 15,
							bY + 4 + (i / perLine) * 36 + 4 + 8 + 3,
							11, 11
						);
					}
					else
					{
						g.FillRectangle(
							new SolidBrush(GetConditionColor(ship.ConditionType)),
							6 + (itemWidth + 4) * (i % perLine) + nameWidth + 8 + rightWidth - (miniMode ? 0 : rightWidth / 4) - 36,
							bY + 4 + (i / perLine) * 36 + 4 + 8 + 3,
							11, 11
						);
						g.DrawString(
							ship.Condition.ToString(),
							Constants.fontDefault,
							textColor,
							new Point(
								6 + (itemWidth + 4) * (i % perLine) + nameWidth + 8 + rightWidth - (miniMode ? 0 : rightWidth / 4) - 24,
								bY + 4 + (i / perLine) * 36 + 4 + 8 + 3 - 3
							)
						);
					}
					#endregion

					i++;
				}
				bY += lines * 36 + 4;
			}
			else
				this.Height = bY + 4; // Fleet info

			g.DrawLine(
				Constants.penActiveFace,
				new Point(4, bY + 2 - 1),
				new Point(this.Width - 4, bY + 2 - 1)
			);
			bY += 4;
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
					new Point(X + (int)(pX * i), Y + hH - 1)
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
			this.Fleet.PropertyEvent(nameof(this.Fleet.ShipsUpdated), () =>
			{
				var ships = this.Fleet.Ships;
				ships.ForEach(x =>
				{
					if (x == null) return;
					x.PropertyEvent(nameof(x.Fuel), () => this.Invalidate());
					x.PropertyEvent(nameof(x.Bull), () => this.Invalidate());
					x.PropertyEvent(nameof(x.HP), () => this.Invalidate());
					x.PropertyEvent(nameof(x.Level), () => this.Invalidate());
				});

				this.Invalidate();
			}, true);

			this.Invalidate();
		}
	}
}
