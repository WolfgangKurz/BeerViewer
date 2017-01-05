using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BeerViewer.Core;
using BeerViewer.Models;
using BeerViewer.Models.BattleInfo;

namespace BeerViewer.Views.Controls
{
	public partial class BattleFleetView : UserControl
	{
		#region FleetData
		private FleetData _FleetData { get; set; }
		public FleetData FleetData
		{
			get { return this._FleetData; }
			set
			{
				if (this._FleetData != value)
				{
					this._FleetData = value;
					this.AttachFleetDataHandler();
					this.RequestUpdate();
				}
			}
		}
		#endregion

		#region AirCombatResults
		private AirCombatResult[] _AirCombatResults { get; set; }
		public AirCombatResult[] AirCombatResults
		{
			get { return this._AirCombatResults; }
			set
			{
				if (this._AirCombatResults != value)
				{
					this._AirCombatResults = value;
					this.RequestUpdate();
				}
			}
		}
		#endregion

		protected Dictionary<Rectangle, ShipData> ShipDataMap { get; private set; } = new Dictionary<Rectangle, ShipData>();
		private Size LatestSize { get; set; } = new Size(160, 320);

		private BattleShipTooltip toolTip { get; }

		public BattleFleetView()
		{
			InitializeComponent();

			if (this.components == null) this.components = new Container();
			this.toolTip = new BattleShipTooltip(this, this.components);

			this.Paint += (s, e) =>
			{
				if (e == null) return;
				if (this.FleetData == null) return;

				var g = e.Graphics;
				var Width = this.Width - this.Padding.Left - this.Padding.Right;
				var Height = this.Height - this.Padding.Top - this.Padding.Bottom;

				g.TranslateTransform(this.Padding.Left, this.Padding.Top);

				Size renderSize = Size.Empty;
				Color colorWhite = Color.White;
				Color colorTextGray = Color.FromArgb(0x80, 0x90, 0x90, 0x90);
				Color colorGray = Color.FromArgb(0x30, 0x90, 0x90, 0x90);
				Color colorMvpYellow = Color.FromArgb(0xFF, 0xE5, 0x58);

				int w, h = TextRenderer.MeasureText(" ", this.Font).Height;
				int x = 2, y = 0;

				#region First Sector (Fleet Information)
				#region Fleet Name
				var fleetName = FleetData?.Name ?? string.Empty;
				if(fleetName == string.Empty)
				{
					if (FleetData?.FleetType == FleetType.Enemy)
						fleetName = "적함대";
					else if (FleetData?.FleetType == FleetType.SecondEnemy)
						fleetName = "적호위함대";
					else if (FleetData?.FleetType == FleetType.Second)
						fleetName = "호위함대";
					else
						fleetName = "???";
				}

				TextRenderer.DrawText(
					g,
					fleetName,
					this.Font,
					new Point(x, y),
					colorTextGray
				);
				w = TextRenderer.MeasureText(fleetName, this.Font).Width;
				x += w + 8;
				#endregion

				#region Fleet Formation
				var formation = (FleetData?.Formation ?? Formation.없음) != Formation.없음
					? FleetData?.Formation.ToString() : "";

				w = TextRenderer.MeasureText(formation, this.Font).Width;
				TextRenderer.DrawText(g, formation, this.Font, new Point(x, y), colorWhite);
				x += w - 2;
				#endregion

				renderSize.Width = Math.Max(renderSize.Width, x);
				x = 0;
				y += h;
				#endregion

				#region Separator
				using (Pen p = new Pen(colorGray))
					g.DrawLine(p, 2, y + 2, Width - 2, y + 2);
				y += 3;
				#endregion

				#region Second Sector (Air Combat Result)
				var AvailableFleet = (
					((FleetData?.FleetType ?? FleetType.Unknown) == FleetType.First)
					|| ((FleetData?.FleetType ?? FleetType.Unknown) == FleetType.Enemy)
				);

				if (AvailableFleet && this.AirCombatResults != null)
				{
					foreach (var result in this.AirCombatResults)
					{
						if (result == null) continue;

						Size sz;
						x = 2;

						var resName = result.Name + ":";
						sz = TextRenderer.MeasureText(resName, this.Font);
						TextRenderer.DrawText(
							g,
							resName,
							this.Font,
							new Point(x, y + 4),
							colorTextGray
						);
						x += sz.Width;

						string airResult = "없음";
						if (result.IsHappen)
							airResult = string.Format(
								"{0} / {1} (-{2})",
								FleetData.FleetType == FleetType.First ? result.FriendRemainingCount : result.EnemyRemainingCount,
								FleetData.FleetType == FleetType.First ? result.FriendCount : result.EnemyCount,
								FleetData.FleetType == FleetType.First ? result.FriendLostCount : result.EnemyLostCount
							);

						sz = TextRenderer.MeasureText(airResult, this.Font);
						TextRenderer.DrawText(
							g,
							airResult,
							this.Font,
							new Point(x, y + 4),
							colorWhite
						);

						x += sz.Width;
						y += h + 2;
						renderSize.Width = Math.Max(renderSize.Width, x + 2);
					}
					y += (h + 2) * (2 - Math.Min(2, this.AirCombatResults?.Length ?? 0));
				}
				else
					y += (h + 2) * Math.Max(2, this.AirCombatResults?.Length ?? 0);
				#endregion

				#region Separator
				using (Pen p = new Pen(colorGray))
					g.DrawLine(p, 2, y + 2, Width - 2, y + 2);
				y += 3;
				#endregion

				#region Third Sector (Fleet Damage Guage)
				#region Fleet Damage Guage
				string FleetGauge = (FleetData?.AttackGauge ?? string.Empty) != string.Empty
					? FleetData.AttackGauge
					: string.Empty;

				TextRenderer.DrawText(
					g,
					FleetGauge,
					this.Font,
					new Point(2, y + 4),
					colorTextGray
				);
				w = TextRenderer.MeasureText(FleetGauge, this.Font).Width;
				x += w;
				#endregion

				renderSize.Width = Math.Max(renderSize.Width, x);
				x = 0;
				y += h + 3;
				#endregion

				#region Separator
				using (Pen p = new Pen(colorGray))
					g.DrawLine(p, 2, y + 2, Width - 2, y + 2);
				y += 3;
				#endregion

				#region Fourth Sector (Ships)
				if (FleetData?.Ships != null)
				{
					int ny, nw = 0, nh = h;
					int hpsize = 0;

					ny = y;
					foreach (var ship in FleetData?.Ships)
					{
						Size sz;
						x = 2;
						ny += 3;

						#region First Line (Ship Name)
						using (Font f = new Font(this.Font.FontFamily, 11, ship.IsMvp ? FontStyle.Bold : FontStyle.Regular))
						{
							sz = TextRenderer.MeasureText(ship.Name, f);
							TextRenderer.DrawText(g, ship.Name, f, new Point(x, ny), ship.IsMvp ? colorMvpYellow : colorWhite);
							nw = Math.Max(nw, sz.Width);
							ny += sz.Height;
						}
						#endregion

						#region Second Line (Ship Info)
						using (Font f = new Font(this.Font.FontFamily, 8))
						{
							#region Ship Level
							TextRenderer.DrawText(g, "Lv ", f, new Point(x, ny), colorTextGray);
							x += TextRenderer.MeasureText("Lv ", f).Width - 8;

							TextRenderer.DrawText(g, ship.Level.ToString(), f, new Point(x, ny), colorWhite);
							x += TextRenderer.MeasureText(ship.Level.ToString(), f).Width;
							#endregion

							#region Ship Type
							TextRenderer.DrawText(g, ship.TypeName, f, new Point(x, ny), colorTextGray);

							sz = TextRenderer.MeasureText(ship.TypeName, f);
							x += sz.Width;
							ny += sz.Height;
							#endregion
						}
						#endregion

						#region HP Size check
						int hpsize_current = 0;
						sz = TextRenderer.MeasureText("HP:", new Font(this.Font.FontFamily, 9));
						hpsize_current += sz.Width - 4;
						sz = TextRenderer.MeasureText(ship.NowHP.ToString(), new Font(this.Font.FontFamily, 11));
						hpsize_current += sz.Width - 7;
						sz = TextRenderer.MeasureText("/" + ship.MaxHP.ToString(), new Font(this.Font.FontFamily, 9));
						hpsize_current += sz.Width;

						hpsize = Math.Max(hpsize, hpsize_current);
						#endregion

						nw = Math.Max(nw, x - 2);
						ny += 1;
					}
					nw += 4;

					ShipDataMap.Clear();

					ny = y;
					foreach (var ship in FleetData?.Ships)
					{
						Size sz;

						#region Right Sector (HP)
						x = 2 + nw;
						ny += 6;

						TextRenderer.DrawText(
							g,
							"HP:",
							new Font(this.Font.FontFamily, 9),
							new Point(x, ny + 2),
							Color.FromArgb(0x30, 0x90, 0x90, 0x90)
						);
						sz = TextRenderer.MeasureText("HP:", new Font(this.Font.FontFamily, 9));
						x += sz.Width - 4;

						TextRenderer.DrawText(
							g,
							ship.NowHP.ToString(),
							new Font(this.Font.FontFamily, 11),
							new Point(x, ny - 2),
							Color.White
						);
						sz = TextRenderer.MeasureText(ship.NowHP.ToString(), new Font(this.Font.FontFamily, 11));
						x += sz.Width - 7;
						nh = sz.Height;

						TextRenderer.DrawText(
							g,
							"/" + ship.MaxHP.ToString(),
							new Font(this.Font.FontFamily, 9),
							new Point(x, ny + 2),
							Color.FromArgb(0x30, 0x90, 0x90, 0x90)
						);
						sz = TextRenderer.MeasureText(ship.MaxHP.ToString(), new Font(this.Font.FontFamily, 9));
						x += sz.Width;
						ny += nh;

						DrawProgress(g, new Rectangle(nw + 2, ny - 2, hpsize, 6), new LimitedValue(ship.NowHP, ship.MaxHP, 0));
						ny += 6;
						#endregion

						#region Separator
						using (Pen p = new Pen(colorGray))
							g.DrawLine(p, 2, ny + 3, Width - 2, ny + 3);
						ny += 5;
						#endregion

						ShipDataMap.Add(new Rectangle(0, y, this.Width, ny - y), ship);

						renderSize.Width = Math.Max(renderSize.Width, x + 4);
						y = ny;
					}
				}
				#endregion

				renderSize.Width = Math.Max(120, renderSize.Width);

				var ResultSize = new Size(
					renderSize.Width + this.Padding.Left + this.Padding.Right,
					y + this.Padding.Top + this.Padding.Bottom
				);
				if (ResultSize.Width != LatestSize.Width || ResultSize.Height != LatestSize.Height)
				{
					LatestSize = ResultSize;
					this.PerformAutoScale();
					this.PerformLayout();
				}
			};

			this.MouseMove += (s, e) =>
			{
				if (!this.ShipDataMap.Any(x => x.Key.Contains(e.X, e.Y)))
				{
					toolTip.SetShip(null);
					return;
				}

				var item = this.ShipDataMap.FirstOrDefault(x => x.Key.Contains(e.X, e.Y)).Value;
				toolTip.SetShip(item);
			};
			this.MouseDown += (s, e) =>
			{
				if (!this.ShipDataMap.Any(x => x.Key.Contains(e.X, e.Y)))
				{
					toolTip.SetShip(null);
					return;
				}

				var item = this.ShipDataMap.FirstOrDefault(x => x.Key.Contains(e.X, e.Y)).Value;
				toolTip.SetShip(item, true);
			};
		}

		private void AttachFleetDataHandler()
		{
			if (this.FleetData == null) return;
			if(this.FleetData.Ships == null) return;

			foreach(var ship in this.FleetData.Ships)
			{
				ship.PropertyEvent(nameof(ship.NowHP), () => this.RequestUpdate());
				ship.PropertyEvent(nameof(ship.MaxHP), () => this.RequestUpdate());
				ship.PropertyEvent(nameof(ship.IsMvp), () => this.RequestUpdate());
				ship.PropertyEvent(nameof(ship.Name), () => this.RequestUpdate());
				ship.PropertyEvent(nameof(ship.Level), () => this.RequestUpdate());
				ship.PropertyEvent(nameof(ship.TypeName), () => this.RequestUpdate());
			}
		}
		public void RequestUpdate()
		{
			if (this.InvokeRequired)
			{
				this.Invoke(RequestUpdate);
				return;
			}
			this.Invalidate();
			this.PerformAutoScale();
			this.PerformLayout();
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			if (this.FleetData == null || (this.FleetData.Ships?.Count() ?? 0) == 0)
				LatestSize = new Size(1, 1);
			return LatestSize;
		}

		private void DrawProgress(Graphics g, Rectangle size, LimitedValue HP)
		{
			Color color = GetHPColor(HP);

			int width = 0;
			if (HP.Maximum - HP.Minimum > 0)
				width = (HP.Current - HP.Minimum) * size.Width / (HP.Maximum - HP.Minimum);

			using (SolidBrush b = new SolidBrush(Color.FromArgb(0x44, 0x90, 0x90, 0x90)))
				g.FillRectangle(b, size);

			using (SolidBrush b = new SolidBrush(color))
				g.FillRectangle(b, new Rectangle(size.Left, size.Top, width, size.Height));

			var step = size.Width / 4;
			using (Pen p = new Pen(frmMain.Instance.BackColor, 1.0f))
			{
				for (var i = 1; i <= 3; i++)
					g.DrawLine(p, size.Left + step * i, size.Top, size.Left + step * i, size.Top + size.Height / 2 - 1);
			}
		}
		private Color GetHPColor(LimitedValue HP)
		{
			double ratio = 0;
			if (HP.Maximum - HP.Minimum > 0)
				ratio = (double)(HP.Current - HP.Minimum) / (HP.Maximum - HP.Minimum);

			if (ratio <= 0.25) // 대파
				return Color.FromArgb(255, 32, 32);
			else if (ratio <= 0.5) // 중파
				return Color.FromArgb(240, 128, 32);
			else if (ratio <= 0.75) // 소파
				return Color.FromArgb(240, 240, 0);
			else // 정상
				return Color.FromArgb(64, 200, 32);
		}
	}
}
