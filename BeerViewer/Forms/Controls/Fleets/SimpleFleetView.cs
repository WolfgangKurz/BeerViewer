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
	internal class SimpleFleetView : FrameworkContainer
	{
		private struct ColorText
		{
			public ColorText(string Text, Color Color)
			{
				this.Text = Text;
				this.Color = Color;
			}

			public string Text;
			public Color Color;
		}

		public Fleet Fleet { get; protected set; }

		#region Initializers
		public SimpleFleetView() : base()
			=> this.Initialize();

		public SimpleFleetView(FrameworkRenderer Renderer) : base(Renderer)
			=> this.Initialize();

		public SimpleFleetView(int X, int Y) : base(X, Y)
			=> this.Initialize();

		public SimpleFleetView(FrameworkRenderer Renderer, int X, int Y) : base(Renderer, X, Y)
			=> this.Initialize();

		public SimpleFleetView(int X, int Y, int Width, int Height) : base(X, Y, Width, Height)
			=> this.Initialize();

		public SimpleFleetView(FrameworkRenderer Renderer, int X, int Y, int Width, int Height) : base(Renderer, X, Y, Width, Height)
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

			var pY = 18;
			var bY = 0;

			g.Clear(Constants.colorNormalFace);
			#region Fleet Top Info
			{
				ColorText[] texts;

				if ((this.Fleet?.Ships?.Length ?? 0) == 0)
				{
					texts = new ColorText[]
					{
						new ColorText($"{i18n.Current.fleet_level}: -", Color.Gray),
						new ColorText("-", Color.Gray),
						new ColorText($"{i18n.Current.fleet_los}: -", Color.Gray),
						new ColorText($"{i18n.Current.fleet_aa}: -", Color.Gray)
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

					texts = new ColorText[]
					{
						new ColorText($"{i18n.Current.fleet_level}: {levels}", Color.White),
						new ColorText($"{i18n.Current["fleet_speed_"+speed]}", Color.White),
						new ColorText($"{i18n.Current.fleet_los}: {los}", Color.White),
						new ColorText($"{i18n.Current.fleet_aa}: {aa}", Color.White)
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

					using (var brush = new SolidBrush(texts[i].Color))
						g.DrawString(
							texts[i].Text,
							Constants.fontDefault,
							brush,
							new Point(
								6 + itemWidth * (i % perLine),
								2 + pY * (i / perLine)
							)
						);

					g.Restore(s);
				}
				bY += lines * 20;

				if ((this.Fleet?.Ships?.Length ?? 0) > 0)
				{
					bY += 2;

					var dock = Homeport.Instance.Repairyard.Docks;
					string text = "";
					Color bcolor = Color.DarkGray;

					if (dock.Any(x => this.Fleet.Ships.Contains(x.Value?.Ship))) // Repairing ship exists
					{
						text = i18n.Current.fleet_status_repair;
						bcolor = Constants.colorBrownAccent;
					}
					else if (this.Fleet.Expedition.IsInExecution) // in Expedition
					{
						text = i18n.Current.fleet_status_expedition;
						bcolor = Constants.colorBlueAccent;
					}
					else if (this.Fleet.IsInSortie) // in Sortie
					{
						text = i18n.Current.fleet_status_sortie;
						bcolor = Constants.colorRedAccent;
					}
					else if (this.Fleet.Ships.Any(x => x.HP.Percentage <= 0.25)) // Critical ship exists
					{
						text = i18n.Current.fleet_status_critical;
						bcolor = Constants.colorRedAccent;
					}
					else if (this.Fleet.Ships.Any(x => x.Fuel.Percentage < 1 || x.Bull.Percentage < 1)) // Not supplied ship exists
					{
						text = i18n.Current.fleet_status_supply;
						bcolor = Constants.colorBrownAccent;
					}
					else if (this.Fleet.Ships.Any(x => x.Condition < 49)) // Need to rejuvenate condition
					{
						text = this.Fleet.RejuvenateText;
						bcolor = Constants.colorBrownAccent;
					}
					else if (this.Fleet.Ships.FirstOrDefault().Info.ShipType.Id == 19) // Flagship is Repair Ship
					{
						text = i18n.Current.fleet_status_repairship;
						bcolor = Constants.colorBrownAccent;
					}
					else
					{
						text = i18n.Current.fleet_status_ready;
						bcolor = Constants.colorGreenAccent;
					}

					using (var brush = new SolidBrush(bcolor))
						g.FillRectangle(
							brush,
							new Rectangle(4, bY, this.Width - 8 + 1, 20 - 1)
						);

					g.DrawString(
						text,
						Constants.fontDefault,
						Brushes.White,
						new Point(6, bY + 1)
					);
					bY += 20;
				}

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
				var textColor = Brushes.White;
				var ships = this.Fleet.Ships;

				var nameWidth = ships
					.Max(x => (int)g.MeasureString(
						i18n.Current[x.Info.Name],
						Constants.fontBig
					).Width);

				var itemWidth = this.Width - 12;
				if (itemWidth < 240) itemWidth = (itemWidth - 0) / 1;
				else if (itemWidth < 480) itemWidth = (itemWidth - 4) / 2;
				else itemWidth = (itemWidth - 8) / 3;
				itemWidth = Math.Max(itemWidth, 1);

				var rightWidth = (itemWidth - nameWidth - 8);
				var miniMode = (rightWidth < 128);
				var miniMode2 = (rightWidth < 84);

				var perLine = Math.Max(1, (this.Width-12) / itemWidth);
				var lines = (int)Math.Ceiling((double)ships.Length / perLine);

				this.Height = bY + lines * 36 + 6; // Fleet info

				int i = 0;
				foreach (var ship in ships)
				{
					#region Name & Level
					g.DrawString(
						i18n.Current[ship.Info.Name],
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
						bY + 4 + (i / perLine) * 36 + 6,
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
								bY + 4 + (i / perLine) * 36 + 14
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
								bY + 4 + (i / perLine) * 36 + 14
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
							bY + 4 + (i / perLine) * 36 + 4 + 12,
							rightWidth / 4, 5,
							ship.Fuel, 5
						);
						DrawColorIndicator(
							g,
							6 + (itemWidth + 4) * (i % perLine) + nameWidth + 8 + rightWidth - (rightWidth / 4),
							bY + 4 + (i / perLine) * 36 + 4 + 12 + 8,
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
							bY + 4 + (i / perLine) * 36 + 4 + 10 + 3,
							11, 11
						);
					}
					else
					{
						g.FillRectangle(
							new SolidBrush(GetConditionColor(ship.ConditionType)),
							6 + (itemWidth + 4) * (i % perLine) + nameWidth + 8 + rightWidth - (miniMode ? 0 : rightWidth / 4) - 36,
							bY + 4 + (i / perLine) * 36 + 4 + 10 + 3,
							11, 11
						);
						g.DrawString(
							ship.Condition.ToString(),
							Constants.fontDefault,
							textColor,
							new Point(
								6 + (itemWidth + 4) * (i % perLine) + nameWidth + 8 + rightWidth - (miniMode ? 0 : rightWidth / 4) - 24,
								bY + 4 + (i / perLine) * 36 + 4 + 10 + 3 - 3
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
			this.Fleet.PropertyEvent(nameof(this.Fleet.ShipsUpdated), () => this.UpdateShips());
			this.Fleet.PropertyEvent(nameof(this.Fleet.Ships), () => this.UpdateShips());
			this.Fleet.PropertyEvent(nameof(this.Fleet.IsInSortie), () => this.UpdateShips());
			this.Fleet.PropertyEvent(nameof(this.Fleet.RejuvenateText), () => this.UpdateShips());
			this.Fleet.Expedition.PropertyEvent(nameof(this.Fleet.Expedition.IsInExecution), () => this.UpdateShips());

			this.Invalidate();
		}
		private void UpdateShips()
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
		}
	}
}
