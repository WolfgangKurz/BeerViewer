using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

using BeerViewer.Core;
using BeerViewer.Models;

namespace BeerViewer.Views.Controls
{
	public partial class FleetView : UserControl
	{
		#region Fleet 프로퍼티
		private Fleet _Fleet { get; set; }
		public Fleet Fleet
		{
			get { return this._Fleet; }
			set
			{
				if (this._Fleet != value)
				{
					this._Fleet = value;
					this.RequestUpdate();
				}
			}
		}
		#endregion

		private Dictionary<Rectangle, ShipSlot> SlotItemMap { get; set; } = new Dictionary<Rectangle, ShipSlot>();
		private SlotItem CurrentItem { get; set; }

		private Size LatestSize { get; set; } = new Size(320, 64);

		public FleetView()
		{
			InitializeComponent();

			this.Paint += (s, e) =>
			{
				var g = e.Graphics;
				var Width = this.Width - this.Padding.Left - this.Padding.Right;
				var Height = this.Height - this.Padding.Top - this.Padding.Bottom;

				g.TranslateTransform(this.Padding.Left, this.Padding.Top);

				IEnumerable<Ship> Ships = null;

				int nw = 0, y = 0;
				#region Draw Header
				using (SolidBrush b = new SolidBrush(Color.FromArgb(0x30, 0x90, 0x90, 0x90)))
					g.FillRectangle(b, new Rectangle(0, 0, Width, 24));

				if (this.Fleet != null && this.Fleet.Ships != null)
				{
					int x = 4;

					FleetState state = this.Fleet.State;
					if (state != null)
					{
						string[] texts = new string[]
						{
							"레벨 합계: " + state.TotalLevel.ToString(),
							"평균: " + Math.Round(state.AverageLevel, 2).ToString(),
							"색적: " + Math.Round(state.ViewRange, 2).ToString(),
							string.Format(
								"제공: {0}-{1} ({2} %)",
								state.MinAirSuperiorityPotential.ToString(),
								state.MaxAirSuperiorityPotential.ToString(),
								Math.Round(state.EncounterPercent, 2)
							),
							state.Speed.ToStateString()
						};

						foreach (var text in texts)
						{
							Size sz = TextRenderer.MeasureText(text, this.Font);
							TextRenderer.DrawText(g, text, this.Font, new Rectangle(x, y + 5, sz.Width, sz.Height), Color.White);
							x += sz.Width + 8;
						}
						x -= 4;
					}
					nw = x;
				}
				y += 24 + 1;
				#endregion

				#region Draw Fleet Name
				using (SolidBrush b = new SolidBrush(Color.FromArgb(0x30, 0x90, 0x90, 0x90)))
					g.FillRectangle(b, new Rectangle(0, y, 30, Height - y));

				using (SolidBrush b = new SolidBrush(Color.White))
				{
					var FleetName = Fleet?.Name ?? "함대명";

					using (Bitmap buffer = new Bitmap(30, Height - y, PixelFormat.Format32bppArgb))
					{
						using (Graphics g2 = Graphics.FromImage(buffer))
						{
							Size sz = TextRenderer.MeasureText(FleetName, this.Font);
							g2.TranslateTransform(-sz.Width / 2, -sz.Height / 2, MatrixOrder.Append);
							g2.RotateTransform(-90, MatrixOrder.Append);
							g2.TranslateTransform(15, (Height - y) / 2, MatrixOrder.Append);
							g2.DrawString(FleetName, this.Font, b, 0, 0);
						}

						g.DrawImage(buffer, new Rectangle(0, y, 30, Height - y));
					}
				}
				#endregion

				if (this.Fleet == null || this.Fleet.Ships == null)
					return; // No elements to draw

				Ships = this.Fleet.Ships;

				#region Draw Ships
				var w1 = TextRenderer.MeasureText("Lv.", new Font(this.Font.FontFamily, 9)).Width - 4;
				var w2 = TextRenderer.MeasureText("HP:", new Font(this.Font.FontFamily, 9)).Width - 6;

				var cells = new int[] { 0, 0, 0, 0 };
				cells[0] = Ships.Max(x => TextRenderer.MeasureText(x.Info.ShipType.Name, new Font(this.Font.FontFamily, 8)).Width) + 4;
				cells[1] = Ships.Max(x => TextRenderer.MeasureText(x.Info.Name, new Font(this.Font.FontFamily, 15)).Width) + 4;
				cells[2] = w1 + Ships.Max(x => TextRenderer.MeasureText(x.Level.ToString(), new Font(this.Font.FontFamily, 10)).Width)+8;
				cells[3] = w2 + Ships.Max(x =>
					TextRenderer.MeasureText(x.HP.Current.ToString(), new Font(this.Font.FontFamily, 11)).Width - 8
					+ TextRenderer.MeasureText("/" + x.HP.Maximum.ToString(), new Font(this.Font.FontFamily, 9)).Width
				) + 8;

				SlotItemMap.Clear();

				foreach (var ship in Ships)
				{
					if (ship == null) continue;

					var x = 30;
					Size sz;

					#region 함종명
					TextRenderer.DrawText(
						g,
						ship.Info.ShipType.Name,
						new Font(this.Font.FontFamily, 8),
						new Rectangle(x + 4, y - 3, cells[0], 28),
						Color.FromArgb(0x30, 0x90, 0x90, 0x90),
						TextFormatFlags.Right | TextFormatFlags.Bottom
					);
					x += cells[0];
					#endregion

					#region 함선명
					TextRenderer.DrawText(
						g,
						ship.Info.Name,
						new Font(this.Font.FontFamily, 15),
						new Rectangle(x, y, cells[1], 28),
						Color.White,
						TextFormatFlags.Left | TextFormatFlags.Bottom
					);
					x += cells[1];
					#endregion

					#region 함선레벨
					TextRenderer.DrawText(
						g,
						"Lv.",
						new Font(this.Font.FontFamily, 9),
						new Rectangle(x, y - 2, w1, 28),
						Color.FromArgb(0x30, 0x90, 0x90, 0x90),
						TextFormatFlags.Left | TextFormatFlags.Bottom
					);
					TextRenderer.DrawText(
						g,
						ship.Level.ToString(),
						new Font(this.Font.FontFamily, 11),
						new Rectangle(x + w1, y - 1, cells[2] - w1, 28),
						Color.White,
						TextFormatFlags.Left | TextFormatFlags.Bottom
					);
					x += cells[2];
					#endregion

					#region 함선 체력
					sz = TextRenderer.MeasureText(ship.HP.Current.ToString(), new Font(this.Font.FontFamily, 11));

					TextRenderer.DrawText(
						g,
						"HP:",
						new Font(this.Font.FontFamily, 9),
						new Rectangle(x, y + 2, cells[1], 16),
						Color.FromArgb(0x30, 0x90, 0x90, 0x90),
						TextFormatFlags.Left | TextFormatFlags.Bottom
					);
					TextRenderer.DrawText(
						g,
						ship.HP.Current.ToString(),
						new Font(this.Font.FontFamily, 11),
						new Rectangle(x + w2, y, cells[1], 16),
						Color.White,
						TextFormatFlags.Left | TextFormatFlags.Bottom
					);
					TextRenderer.DrawText(
						g,
						"/" + ship.HP.Maximum.ToString(),
						new Font(this.Font.FontFamily, 9),
						new Rectangle(x + w2 + sz.Width - 8, y + 2, cells[1], 16),
						Color.FromArgb(0x30, 0x90, 0x90, 0x90),
						TextFormatFlags.Left | TextFormatFlags.Bottom
					);

					DrawProgress(g, new Rectangle(x + 2, y + 20, cells[3] - 8-4, 6), ship.HP);

					x += cells[3];
					#endregion

					#region 함선 피로도
					using (SolidBrush b = new SolidBrush(GetCondColor(ship.ConditionType)))
						g.FillRectangle(b, new Rectangle(x + 4, y + 4, 12, 12));

					TextRenderer.DrawText(
						g,
						ship.Condition.ToString(),
						new Font(this.Font.FontFamily, 9),
						new Rectangle(x + 16, y + 3, 24, 16),
						Color.White,
						TextFormatFlags.Left | TextFormatFlags.Top
					);
					TextRenderer.DrawText(
						g,
						"피로도",
						new Font(this.Font.FontFamily, 7),
						new Rectangle(x + 2, y + 18, 40, 16),
						Color.FromArgb(0x30,0x90,0x90,0x90),
						TextFormatFlags.Left | TextFormatFlags.Top
					);
					x += 48;
					#endregion

					#region 함선 연료/탄약
					DrawProgress(g, new Rectangle(x - 6, y + 6, 44, 6), ship.Fuel);
					DrawProgress(g, new Rectangle(x - 6, y + 18, 44, 6), ship.Bull);

					x += 44;
					#endregion

					#region 장비
					foreach (var item in ship.Slots)
					{
						if (item == null || !item.Equipped) continue;

						SlotItemMap.Add(new Rectangle(x, y + 2, 36, 28), item);
						DrawSlotItem(g, item, x, y + 2);
						x += 38;
					}

					if(ship.ExSlotExists && ship.ExSlot.Equipped)
					{
						using (Pen p = new Pen(Color.FromArgb(0x90, 0x90, 0x90, 0x90)))
							g.DrawLine(p, x+2, y + 2, x+2, y + 28);
						x += 7;

						var item = ship.ExSlot;
						SlotItemMap.Add(new Rectangle(x, y + 2, 36, 28), item);
						DrawSlotItem(g, item, x, y + 2);
						x += 38;
					}
					x -= 2;
					#endregion

					nw = Math.Max(nw, x);
					y += 32;
				}
				#endregion

				for (var i = 0; i < Ships.Count(x => x != null); i++)
				{
					using (Pen p = new Pen(Color.FromArgb(0x30, 0x90, 0x90, 0x90)))
						g.DrawLine(p, 30, y - i * 32 - 1, nw, y - i * 32 - 1);
				}

				var ResultSize = new Size(
					nw + this.Padding.Left + this.Padding.Right,
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
				if (!this.SlotItemMap.Any(x => x.Key.Contains(e.X, e.Y)))
				{
					CurrentItem = null;
					toolTip.Hide(this);
					return;
				}

				var item = this.SlotItemMap.FirstOrDefault(x => x.Key.Contains(e.X, e.Y)).Value.Item;
				if (item == CurrentItem) return;

				CurrentItem = item;
				toolTip.Show(CurrentItem.NameWithLevel, this);
			};
			this.MouseDown += (s, e) =>
			{
				if (!this.SlotItemMap.Any(x => x.Key.Contains(e.X, e.Y)))
				{
					CurrentItem = null;
					toolTip.Hide(this);
					return;
				}

				var item = this.SlotItemMap.FirstOrDefault(x => x.Key.Contains(e.X, e.Y)).Value.Item;
				CurrentItem = item;
				toolTip.Show(CurrentItem.NameWithLevel, this);
			};

			var toolTipFont = new Font(this.Font.FontFamily, 10);
			toolTip.Popup += (s, e) =>
			{
				if (CurrentItem == null)
				{
					e.Cancel = true;
					return;
				}

				var sz = TextRenderer.MeasureText(CurrentItem.NameWithLevel, toolTipFont);
				e.ToolTipSize = new Size(sz.Width + 6, sz.Height + 6);
			};
			toolTip.Draw += (s, e) =>
			{
				var g = e.Graphics;
				g.Clear(Color.FromArgb(0x27, 0x27, 0x2F));
				g.DrawRectangle(
					new Pen(Color.FromArgb(0x44, 0x44, 0x4A), 1.0f),
					new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width - 1, e.Bounds.Height - 1)
				);
				TextRenderer.DrawText(
					g,
					e.ToolTipText,
					toolTipFont,
					e.Bounds,
					Color.FromArgb(255, 255, 255),
					TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
				);
			};
		}
		public void RequestUpdate()
		{
			if (this.InvokeRequired)
			{
				this.Invoke(RequestUpdate);
				return;
			}

			this.PerformAutoScale();
			this.PerformLayout();
			this.Invalidate();
		}

		public override Size GetPreferredSize(Size proposedSize) => LatestSize;

		public void SetFleet(Fleet fleet)
		{
			this.Fleet = fleet;

			foreach (var ship in fleet.Ships)
				ship.PropertyEvent("Slots", () => this.RequestUpdate());

			fleet.State.Updated += (s, e) =>this.RequestUpdate();
			this.RequestUpdate();
		}

		private void DrawProgress(Graphics g, Rectangle size, LimitedValue HP)
		{
			int width = (HP.Current - HP.Minimum) * size.Width / (HP.Maximum - HP.Minimum);
			Color color = GetHPColor(HP);

			using (SolidBrush b = new SolidBrush(Color.FromArgb(0x44, 0x90, 0x90, 0x90)))
				g.FillRectangle(b, size);

			using (SolidBrush b = new SolidBrush(color))
				g.FillRectangle(b, new Rectangle(size.Left, size.Top, width, size.Height));

			var step = size.Width / 4;
			using (Pen p = new Pen(frmMain.Instance.BackColor, 1.0f)) {
				for (var i = 1; i <= 3; i++)
					g.DrawLine(p, size.Left + step * i, size.Top, size.Left + step * i, size.Top + size.Height / 2 - 1);
			}
		}
		private Color GetCondColor(ConditionType condType)
		{
			switch (condType)
			{
				case ConditionType.Brilliant:
					return Color.FromArgb(0xFF, 0xFF, 0x40);
				case ConditionType.Tired:
					return Color.FromArgb(0xFF, 0xC8, 0x80);
				case ConditionType.OrangeTired:
					return Color.FromArgb(0xFF, 0x80, 0x20);
				case ConditionType.RedTired:
					return Color.FromArgb(0xFF, 0x20, 0x20);
				default:
					return Color.White;
			}
		}
		private Color GetHPColor(LimitedValue HP)
		{
			double ratio = (double)(HP.Current - HP.Minimum) / (HP.Maximum - HP.Minimum);

			if (ratio <= 0.25) // 대파
				return Color.FromArgb(255, 32, 32);
			else if (ratio <= 0.5) // 중파
				return Color.FromArgb(240, 128, 32);
			else if (ratio <= 0.75) // 소파
				return Color.FromArgb(240, 240, 0);
			else // 정상
				return Color.FromArgb(64, 200, 32);
		}
		private Image GetSlotIconImage(SlotItemIconType iconType)
		{
			switch (iconType)
			{
				case SlotItemIconType.MainCanonLight: return Properties.Resources.slotitem_MainCanonLight;
				case SlotItemIconType.MainCanonMedium: return Properties.Resources.slotitem_MainCanonMedium;
				case SlotItemIconType.MainCanonHeavy: return Properties.Resources.slotitem_MainCanonHeavy;
				case SlotItemIconType.SecondaryCanon: return Properties.Resources.slotitem_SecondaryCanon;

				case SlotItemIconType.Torpedo: return Properties.Resources.slotitem_Torpedo;

				case SlotItemIconType.Fighter: return Properties.Resources.slotitem_Fighter;
				case SlotItemIconType.DiveBomber: return Properties.Resources.slotitem_DiveBomber;
				case SlotItemIconType.TorpedoBomber: return Properties.Resources.slotitem_TorpedoBomber;
				case SlotItemIconType.ReconPlane: return Properties.Resources.slotitem_ReconPlane;
				case SlotItemIconType.ReconSeaplane: return Properties.Resources.slotitem_ReconSeaplane;

				case SlotItemIconType.Rader: return Properties.Resources.slotitem_Rader;
				case SlotItemIconType.AAShell: return Properties.Resources.slotitem_AAShell;
				case SlotItemIconType.APShell: return Properties.Resources.slotitem_APShell;
				case SlotItemIconType.DamageControl: return Properties.Resources.slotitem_DamageControl;

				case SlotItemIconType.AAGun: return Properties.Resources.slotitem_AAGun;
				case SlotItemIconType.HighAngleGun: return Properties.Resources.slotitem_HighAngleGun;

				case SlotItemIconType.ASW: return Properties.Resources.slotitem_ASW;
				case SlotItemIconType.Soner: return Properties.Resources.slotitem_Soner;

				case SlotItemIconType.EngineImprovement: return Properties.Resources.slotitem_EngineImprovement;
				case SlotItemIconType.LandingCraft: return Properties.Resources.slotitem_LandingCraft;

				case SlotItemIconType.Autogyro: return Properties.Resources.slotitem_Autogyro;
				case SlotItemIconType.ArtillerySpotter: return Properties.Resources.slotitem_ArtillerySpotter;

				case SlotItemIconType.AntiTorpedoBulge: return Properties.Resources.slotitem_AntiTorpedoBulge;

				case SlotItemIconType.Searchlight: return Properties.Resources.slotitem_Searchlight;
				case SlotItemIconType.DrumCanister: return Properties.Resources.slotitem_DrumCanister;
				case SlotItemIconType.Facility: return Properties.Resources.slotitem_Facility;
				case SlotItemIconType.Flare: return Properties.Resources.slotitem_Flare;

				case SlotItemIconType.FleetCommandFacility: return Properties.Resources.slotitem_FleetCommandFacility;
				case SlotItemIconType.MaintenancePersonnel: return Properties.Resources.slotitem_MaintenancePersonnel;

				case SlotItemIconType.AntiAircraftFireDirector: return Properties.Resources.slotitem_AntiAircraftFireDirector;
				case SlotItemIconType.RocketLauncher: return Properties.Resources.slotitem_RocketLauncher;

				case SlotItemIconType.SurfaceShipPersonnel: return Properties.Resources.slotitem_SurfaceShipPersonnel;
				case SlotItemIconType.FlyingBoat: return Properties.Resources.slotitem_FlyingBoat;

				case SlotItemIconType.CombatRations: return Properties.Resources.slotitem_CombatRations;
				case SlotItemIconType.OffshoreResupply: return Properties.Resources.slotitem_OffshoreResupply;

				case SlotItemIconType.AmphibiousLandingCraft: return Properties.Resources.slotitem_AmphibiousLandingCraft;

				case SlotItemIconType.LandBasedAttacker: return Properties.Resources.slotitem_LandBasedAttacker;
				case SlotItemIconType.LandBasedFighter: return Properties.Resources.slotitem_LandBasedFighter;

				case SlotItemIconType.JetbombFighter_A: return Properties.Resources.slotitem_JetbombFighter_A;
				case SlotItemIconType.JetBombFighter_B: return Properties.Resources.slotitem_JetBombFighter_B;

				default: return Properties.Resources.slotitem_Unknown;
			}
		}
		private Image GetProficiencyImage(int proficiency)
		{
			switch (proficiency)
			{
				case 1:
					return Properties.Resources.slotitem_proficiency_1;
				case 2:
					return Properties.Resources.slotitem_proficiency_2;
				case 3:
					return Properties.Resources.slotitem_proficiency_3;
				case 4:
					return Properties.Resources.slotitem_proficiency_4;
				case 5:
					return Properties.Resources.slotitem_proficiency_5;
				case 6:
					return Properties.Resources.slotitem_proficiency_6;
				case 7:
					return Properties.Resources.slotitem_proficiency_7;
				default:
					return null;
			}
		}
		private void DrawSlotItem(Graphics g, ShipSlot slotItem, int x, int y)
		{
			var font8 = new Font(this.Font.FontFamily, 8);
			var item = slotItem.Item;

			var sloticon = GetSlotIconImage(item.Info.IconType);
			g.DrawImage(sloticon, new Rectangle(x + 4, y, 28, 28));

			if (item.Level > 0)
			{
				using (SolidBrush b = new SolidBrush(Color.FromArgb(0x40, 0x00, 0x00, 0x00)))
					g.FillRectangle(b, new Rectangle(x, y + 16, 36, 12));

				TextRenderer.DrawText(
					g,
					item.LevelText,
					font8,
					new Rectangle(x, y + 16, 36, 12),
					Color.White,
					TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
				);
			}

			if (item.Proficiency > 0)
			{
				using (SolidBrush b = new SolidBrush(Color.FromArgb(0x60, 0x00, 0x00, 0x00)))
					g.FillRectangle(b, new Rectangle(x + 36 - 12, y, 12, 12));

				var proficiency = GetProficiencyImage(item.Proficiency);
				g.DrawImage(proficiency, new Rectangle(x + 36 - 11, y + 1, 10, 10));
			}

			var w = TextRenderer.MeasureText(slotItem.Current.ToString(), font8).Width;
			using (SolidBrush b = new SolidBrush(Color.FromArgb(0x60, 0x00, 0x00, 0x00)))
				g.FillRectangle(b, new Rectangle(x + 2, y, w - 1, 12));

			var currentColor = slotItem.Lost == 0 ? Color.White : Color.FromArgb(0xDD, 0x35, 0x35);
			if (!slotItem.IsAirplane)
				currentColor = Color.FromArgb(
					currentColor.R * 5 / 7,
					currentColor.G * 5 / 7,
					currentColor.B * 5 / 7
				);
			TextRenderer.DrawText(
				g,
				slotItem.Current.ToString(),
				font8,
				new Rectangle(x + 2, y, 36, 12),
				currentColor,
				TextFormatFlags.Left | TextFormatFlags.VerticalCenter
			);
		}
	}
}
