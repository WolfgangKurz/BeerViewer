using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ControlCollection = System.Windows.Forms.Control.ControlCollection;

using BeerViewer.Models;
using BeerViewer.Models.BattleInfo;

namespace BeerViewer.Views.Controls
{
	class BattleShipTooltip : ToolTip
	{
		private BattleFleetView Owner { get; }
		private ShipData Ship { get; set; }

		public BattleShipTooltip(BattleFleetView Owner, IContainer container) : base(container)
		{
			this.Owner = Owner;

			this.Popup += (s, e) =>
			{
				e.ToolTipSize = CalcSize(this.Ship);
			};
			this.Draw += (s, e) =>
			{
				var g = e.Graphics;
				g.Clear(Color.FromArgb(0x27, 0x27, 0x2F));
				g.DrawRectangle(
					new Pen(Color.FromArgb(0x44, 0x44, 0x4A), 1.0f),
					new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width - 1, e.Bounds.Height - 1)
				);
				if (this.Ship == null) return;

				Color colorWhite = Color.White;
				Color colorDarkGray = Color.FromArgb(0x20, 0x90, 0x90, 0x90);
				Color colorGray = Color.FromArgb(0x40, 0xC4, 0xC4, 0xC4);

				var ship = this.Ship;
				var f = this.Owner?.Font;
				int x, y;
				int h = TextRenderer.MeasureText(" ", f).Height;

				int[] cols = new int[2];
				string firepowerSum = ship.SumFirepower.ToString(),
					torpedoSum = ship.SumTorpedo.ToString(),
					aaSum = ship.SumAA.ToString(),
					armorSum = ship.SumArmor.ToString(),
					firepowerDetail = string.Format("({0}+{1})", ship.Firepower, ship.SlotsFirepower),
					torpedoDetail = string.Format("({0}+{1})", ship.Torpedo, ship.SlotsTorpedo),
					aaDetail = string.Format("({0}+{1})", ship.AA, ship.SlotsAA),
					armorDetail = string.Format("({0}+{1})", ship.Armor, ship.SlotsArmor),
					aswDetail = string.Format("(+{0})", ship.SlotsASW),
					hitDetail = string.Format("(+{0})", ship.SlotsHit),
					evadeDetail = string.Format("(+{0})", ship.SlotsEvade);

				#region Cell size calculate
				cols[0] = new int[] {
					TextRenderer.MeasureText("화력", f).Width
					+ TextRenderer.MeasureText(firepowerSum, f).Width
					+ TextRenderer.MeasureText(firepowerDetail, f).Width,

					TextRenderer.MeasureText("뇌장", f).Width
					+ TextRenderer.MeasureText(torpedoSum, f).Width
					+ TextRenderer.MeasureText(torpedoDetail, f).Width,

					TextRenderer.MeasureText("대공", f).Width
					+ TextRenderer.MeasureText(aaSum, f).Width
					+ TextRenderer.MeasureText(aaDetail, f).Width,

					TextRenderer.MeasureText("장갑", f).Width
					+ TextRenderer.MeasureText(armorSum, f).Width
					+ TextRenderer.MeasureText(armorDetail, f).Width
				}.Max();
				cols[1] = new int[] {
					TextRenderer.MeasureText("대잠", f).Width
					+ TextRenderer.MeasureText(aswDetail, f).Width,

					TextRenderer.MeasureText("명중", f).Width
					+ TextRenderer.MeasureText(hitDetail, f).Width,

					TextRenderer.MeasureText("회피", f).Width
					+ TextRenderer.MeasureText(evadeDetail, f).Width
				}.Max();
				#endregion


				#region First Column
				x = 4; y = 3;
				TextRenderer.DrawText(g, "화력", f, new Point(x, y), colorGray);
				x += TextRenderer.MeasureText("화력", f).Width;
				TextRenderer.DrawText(g, firepowerSum, f, new Point(x, y), colorWhite);
				x += TextRenderer.MeasureText(firepowerSum, f).Width;
				TextRenderer.DrawText(g, firepowerDetail, f, new Point(x, y), colorGray);

				x = 4; y += h;
				TextRenderer.DrawText(g, "뇌장", f, new Point(x, y), colorGray);
				x += TextRenderer.MeasureText("뇌장", f).Width;
				TextRenderer.DrawText(g, torpedoSum, f, new Point(x, y), colorWhite);
				x += TextRenderer.MeasureText(torpedoSum, f).Width;
				TextRenderer.DrawText(g, torpedoDetail, f, new Point(x, y), colorGray);

				x = 4; y += h;
				TextRenderer.DrawText(g, "대공", f, new Point(x, y), colorGray);
				x += TextRenderer.MeasureText("대공", f).Width;
				TextRenderer.DrawText(g, aaSum, f, new Point(x, y), colorWhite);
				x += TextRenderer.MeasureText(aaSum, f).Width;
				TextRenderer.DrawText(g, aaDetail, f, new Point(x, y), colorGray);

				x = 4; y += h;
				TextRenderer.DrawText(g, "장갑", f, new Point(x, y), colorGray);
				x += TextRenderer.MeasureText("장갑", f).Width;
				TextRenderer.DrawText(g,armorSum, f, new Point(x, y), colorWhite);
				x += TextRenderer.MeasureText(armorSum, f).Width;
				TextRenderer.DrawText(g, armorDetail, f, new Point(x, y), colorGray);
				#endregion

				#region Second Column
				x = 4 + cols[0]; y = 3;
				TextRenderer.DrawText(g, "대잠", f, new Point(x, y), colorGray);
				x += TextRenderer.MeasureText("대잠", f).Width;
				TextRenderer.DrawText(g, aswDetail, f, new Point(x, y), colorGray);

				x = 4 + cols[0]; y += h;
				TextRenderer.DrawText(g, "명중", f, new Point(x, y), colorGray);
				x += TextRenderer.MeasureText("명중", f).Width;
				TextRenderer.DrawText(g, hitDetail, f, new Point(x, y), colorGray);

				x = 4 + cols[0]; y += h;
				TextRenderer.DrawText(g, "회피", f, new Point(x, y), colorGray);
				x += TextRenderer.MeasureText("회피", f).Width;
				TextRenderer.DrawText(g, evadeDetail, f, new Point(x, y), colorGray);
				#endregion

				#region Last Column
				x = 4 + cols[0] + cols[1]; y = 3;
				foreach (var slot in ship.Slots.Where(_ => _.Equipped))
				{
					var icon = ImageAssets.GetSlotIconImage(slot.Source.IconType);
					g.DrawImage(icon, new Rectangle(x + 1, y + 1, 14, 14));

					TextRenderer.DrawText(g, slot.Source.Name, f, new Point(x + 14 + 2, y), colorWhite);
					y += h;
				}
				if(ship.ExSlot?.Equipped ?? false)
				{
					using (Pen p = new Pen(colorGray))
						g.DrawLine(p, x + 2, y, e.Bounds.Width - 2, y);

					var slot = ship.ExSlot;
					var icon = ImageAssets.GetSlotIconImage(slot.Source.IconType);
					g.DrawImage(icon, new Rectangle(x + 1, y + 1, 14, 14));

					TextRenderer.DrawText(g, slot.Source.Name, f, new Point(x + 14 + 2, y), colorWhite);
					y += h;
				}
				#endregion
			};
			this.OwnerDraw = true;
		}
		public void SetShip(ShipData ship, bool mustShow = false)
		{
			if (!mustShow && this.Ship == ship) return;
			this.Hide(this.Owner);

			this.Ship = ship;
			if (this.Ship != null)
				this.Show(this.Ship.Id.ToString(), this.Owner);
		}

		private Size CalcSize(ShipData ship)
		{
			var f = this.Owner?.Font;
			if (ship == null)
			{
				return new Size(
					80 + 8,
					(TextRenderer.MeasureText(" ", f).Height * 4) + 6
				);
			}

			int x = 0, w = 0, y = 0;

			string firepowerSum = ship.SumFirepower.ToString(),
				torpedoSum = ship.SumTorpedo.ToString(),
				aaSum = ship.SumAA.ToString(),
				armorSum = ship.SumArmor.ToString(),
				firepowerDetail = string.Format("({0}+{1})", ship.Firepower, ship.SlotsFirepower),
				torpedoDetail = string.Format("({0}+{1})", ship.Torpedo, ship.SlotsTorpedo),
				aaDetail = string.Format("({0}+{1})", ship.AA, ship.SlotsAA),
				armorDetail = string.Format("({0}+{1})", ship.Armor, ship.SlotsArmor),
				aswDetail = string.Format("(+{0})", ship.SlotsASW),
				hitDetail = string.Format("(+{0})", ship.SlotsHit),
				evadeDetail = string.Format("(+{0})", ship.SlotsEvade);

			w = new int[] {
				TextRenderer.MeasureText("화력", f).Width
				+ TextRenderer.MeasureText(firepowerSum, f).Width
				+ TextRenderer.MeasureText(firepowerDetail, f).Width,

				TextRenderer.MeasureText("뇌장", f).Width
				+ TextRenderer.MeasureText(torpedoSum, f).Width
				+ TextRenderer.MeasureText(torpedoDetail, f).Width,

				TextRenderer.MeasureText("대공", f).Width
				+ TextRenderer.MeasureText(aaSum, f).Width
				+ TextRenderer.MeasureText(aaDetail, f).Width,

				TextRenderer.MeasureText("장갑", f).Width
				+ TextRenderer.MeasureText(armorSum, f).Width
				+ TextRenderer.MeasureText(armorDetail, f).Width
			}.Max();
			x += w;

			w = new int[] {
				TextRenderer.MeasureText("대잠", f).Width
				+ TextRenderer.MeasureText(aswDetail, f).Width,

				TextRenderer.MeasureText("명중", f).Width
				+ TextRenderer.MeasureText(hitDetail, f).Width,

				TextRenderer.MeasureText("회피", f).Width
				+ TextRenderer.MeasureText(evadeDetail, f).Width
			}.Max();
			x += w;

			w = ship.Slots
				.Where(_ => _.Equipped)
				.Max(_ => TextRenderer.MeasureText(_.Source.Name, f).Width);
			w += (ship.ExSlot?.Equipped ?? false)
				? TextRenderer.MeasureText(ship.ExSlot.Source.Name, f).Width
				: 0;
			w += 14 + 2; // icon size
			x += w;

			y = Math.Max(4, ship.Slots.Count(_ => _.Equipped) + ((ship.ExSlot?.Equipped ?? false) ? 1 : 0));
			y *= TextRenderer.MeasureText(" ", f).Height;
			return new Size(x + 8, y + 6);
		}
	}
}
