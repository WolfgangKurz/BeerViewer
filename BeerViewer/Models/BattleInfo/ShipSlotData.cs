using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Core;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models.BattleInfo
{
	public class ShipSlotData : Notifier
	{
		public SlotItemInfo Source { get; private set; }

		public int Maximum { get; private set; }
		public bool Equipped => this.Source != null && this.Source.Id != 0;

		#region Current 프로퍼티
		private int _Current;
		public int Current
		{
			get { return this._Current; }
			set
			{
				if (this._Current != value)
				{
					this._Current = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public int Firepower { get; set; }
		public int Torpedo { get; set; }
		public int AA { get; set; }
		public int Armor { get; set; }
		public int Bomb { get; set; }
		public int ASW { get; set; }
		public int Hit { get; set; }
		public int Evade { get; set; }
		public int LOS { get; set; }

		public Type2 Type2 { get; set; }

		public string ToolTip => (this.Firepower != 0 ? "화력:" + this.Firepower : "")
								 + (this.Torpedo != 0 ? " 뇌장:" + this.Torpedo : "")
								 + (this.AA != 0 ? " 대공:" + this.AA : "")
								 + (this.Armor != 0 ? " 장갑:" + this.Armor : "")
								 + (this.Bomb != 0 ? " 폭장:" + this.Bomb : "")
								 + (this.ASW != 0 ? " 대잠:" + this.ASW : "")
								 + (this.Hit != 0 ? " 명중:" + this.Hit : "")
								 + (this.Evade != 0 ? " 회피:" + this.Evade : "")
								 + (this.LOS != 0 ? " 색적:" + this.LOS : "");

		public ShipSlotData(SlotItemInfo item, int maximum = -1, int current = -1)
		{
			this.Source = item;
			this.Maximum = maximum;
			this._Current = current;

			if (item == null) return;

			var m = DataStorage.Instance.Master.SlotItems
				.Select(x => x.Value)
				.SingleOrDefault(x => x.Id == item.Id);
			if (m == null) return;

			this.Armor = m.Armor;
			this.Firepower = m.Firepower;
			this.Torpedo = m.Torpedo;
			this.Bomb = m.Bomb;
			this.AA = m.AA;
			this.ASW = m.ASW;
			this.Hit = m.Hit;
			this.Evade = m.Evade;
			this.LOS = m.ViewRange;
			this.Type2 = (Type2)m.RawData.api_type[1];
		}

		public ShipSlotData(ShipSlot slot)
			: this(slot.Item?.Info, slot.Maximum, slot.Current)
		{
		}
	}
}
