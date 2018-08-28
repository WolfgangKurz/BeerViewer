using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Network;
using BeerViewer.Models.Enums;
using BeerViewer.Models.Raw;
using BeerViewer.Models.Wrapper;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models
{
	public class Ship : RawDataWrapper<kcsapi_ship2>, IIdentifiable
	{
		private readonly Homeport homeport;

		public int Id => this.RawData.api_id;
		public int FleetId
		{
			get
			{
				try
				{
					return homeport.Organization.Fleets
						.Where(x => x.Value.Ships.Any(y => y.Id == this.Id))
						.FirstOrDefault().Value?.Id ?? -1;
				}
				catch { }

				return -1;
			}
		}

		public ShipInfo Info { get; private set; }
		public int SortNumber => this.RawData.api_sortno;

		public int Level => this.RawData.api_lv;
		public int RemodelLevel
		{
			get
			{
				if (this.Info.NextRemodelingLevel.HasValue)
				{
					if (this.Info.NextRemodelingLevel.Value <= this.Level)
						return -1;
					else return this.Info.NextRemodelingLevel.Value;
				}
				else
					return 0;
			}
		}

		public string RepairTimeString
		{
			get
			{
				TimeSpan? Remaining = new TimeSpan(0, 0, 0, 0, (int)this.TimeToRepair.TotalMilliseconds);
				return Remaining.HasValue
					? string.Format(
						"{0:D2}:{1}",
						(int)Remaining.Value.TotalHours,
						Remaining.Value.ToString(@"mm\:ss")
					)
					: "--:--:--";
			}
		}

		public bool IsLocked => this.RawData.api_locked == 1;

		public int Exp => this.RawData.api_exp.Get(0) ?? 0;
		public int ExpForNextLevel => this.RawData.api_exp.Get(1) ?? 0;

		public int ExpForNextRemodelingLevel => Math.Max(Experience.GetShipExpForSpecifiedLevel(this.Exp, this.Info?.NextRemodelingLevel), 0);
		public int ExpForMarrige => Math.Max(Experience.GetShipExpForSpecifiedLevel(this.Exp, 99), 0);
		public int ExpForLevelMax => Experience.GetShipExpForSpecifiedLevel(this.Exp, 155);

		public bool ExSlotExists => this.RawData.api_slot_ex != 0;

		#region HP Property
		private LimitedValue _HP;
		public LimitedValue HP
		{
			get { return this._HP; }
			private set
			{
				this._HP = value;
				this.RaisePropertyChanged();

				if (value.IsHeavilyDamage())
					this.Situation |= ShipSituation.HeavilyDamaged;
				else
					this.Situation &= ~ShipSituation.HeavilyDamaged;
			}
		}
		#endregion

		public ShipSpeed Speed => (ShipSpeed)this.RawData.api_soku;

		#region Fuel Property
		private LimitedValue _Fuel;
		public LimitedValue Fuel
		{
			get { return this._Fuel; }
			private set
			{
				this._Fuel = value;
				this.RaisePropertyChanged();
				this.RaisePropertyChanged(nameof(this.UsedFuel));
			}
		}
		#endregion

		#region Bull Property
		private LimitedValue _Bull;
		public LimitedValue Bull
		{
			get { return this._Bull; }
			private set
			{
				this._Bull = value;
				this.RaisePropertyChanged();
				this.RaisePropertyChanged(nameof(this.UsedBull));
			}
		}
		#endregion

		#region Firepower Property
		private ModernizableStatus _Firepower;
		public ModernizableStatus Firepower
		{
			get { return this._Firepower; }
			private set
			{
				this._Firepower = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region Torpedo Property
		private ModernizableStatus _Torpedo;
		public ModernizableStatus Torpedo
		{
			get { return this._Torpedo; }
			private set
			{
				this._Torpedo = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region YasenFp Property
		private ModernizableStatus _YasenFp;
		public ModernizableStatus YasenFp
		{
			get { return this._YasenFp; }
			private set
			{
				this._YasenFp = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region AA Property
		private ModernizableStatus _AA;
		public ModernizableStatus AA
		{
			get { return this._AA; }
			private set
			{
				this._AA = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region Armor Property
		private ModernizableStatus _Armor;
		public ModernizableStatus Armor
		{
			get { return this._Armor; }
			private set
			{
				this._Armor = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region ASW Property
		private ModernizableStatus _ASW;
		public ModernizableStatus ASW
		{
			get { return this._ASW; }
			private set
			{
				this._ASW = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region Luck Property
		private ModernizableStatus _Luck;
		public ModernizableStatus Luck
		{
			get { return this._Luck; }
			private set
			{
				this._Luck = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion


		#region Slots Property
		private ShipSlot[] _Slots;
		public ShipSlot[] Slots
		{
			get { return this._Slots; }
			set
			{
				if (this._Slots != value)
				{
					this._Slots = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region ExSlot Property
		private ShipSlot _ExSlot;
		public ShipSlot ExSlot
		{
			get { return this._ExSlot; }
			set
			{
				if (this._ExSlot != value)
				{
					this._ExSlot = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public ShipSlot[] EquippedItems
			=> this.Slots
				.Concat(this.ExSlotExists ? new ShipSlot[] { this.ExSlot } : new ShipSlot[] { })
				.Where(x => x.Equipped)
				.ToArray();

		#region TimeToRepair Property
		private TimeSpan _TimeToRepair;
		public TimeSpan TimeToRepair
		{
			get { return this._TimeToRepair; }
			set
			{
				if (this._TimeToRepair != value)
				{
					this._TimeToRepair = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion


		public int UsedFuel => (int)((this.Level <= 99 ? 1.0 : 0.85) * (this.Fuel.Maximum - this.Fuel.Current));
		public int UsedBull => (int)((this.Level <= 99 ? 1.0 : 0.85) * (this.Bull.Maximum - this.Bull.Current));

		public int LOS => this.RawData.api_sakuteki.Get(0) ?? 0;
		public bool IsMaxModernized => this.Firepower.IsMax && this.Torpedo.IsMax && this.AA.IsMax && this.Armor.IsMax;

		public int Condition => this.RawData.api_cond;
		public ConditionType ConditionType => this.RawData.api_cond.ToConditionType();

		public int SallyArea => this.RawData.api_sally_area;

		#region Status Property
		private ShipSituation situation;
		public ShipSituation Situation
		{
			get { return this.situation; }
			set
			{
				if (this.situation != value)
				{
					this.situation = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		internal Ship(Homeport parent, kcsapi_ship2 Data) : base(Data)
		{
			this.homeport = parent;
			this.Update(Data);
		}

		internal void Update(kcsapi_ship2 Data)
		{
			this.UpdateData(Data);

			this.Info = Master.Instance.Ships[Data.api_ship_id] ?? ShipInfo.Empty;
			this.HP = new LimitedValue(this.RawData.api_nowhp, this.RawData.api_maxhp, 0);
			this.Fuel = new LimitedValue(this.RawData.api_fuel, this.Info.RawData.api_fuel_max, 0);
			this.Bull = new LimitedValue(this.RawData.api_bull, this.Info.RawData.api_bull_max, 0);
			this.ASW = new ModernizableStatus(new int[] { 0, this.RawData.api_taisen[1] }, this.RawData.api_taisen[0]);

			if (this.RawData.api_kyouka.Length >= 5)
			{
				this.Firepower = new ModernizableStatus(this.Info.RawData.api_houg, this.RawData.api_kyouka[0]);
				this.Torpedo = new ModernizableStatus(this.Info.RawData.api_raig, this.RawData.api_kyouka[1]);
				this.YasenFp = new ModernizableStatus(
					new int[] {
						this.Info.RawData.api_houg[0] + this.Info.RawData.api_raig[0],
						this.Info.RawData.api_houg[1] + this.Info.RawData.api_raig[1]
					},
					this.RawData.api_kyouka[0] + this.RawData.api_kyouka[1]
				);

				this.AA = new ModernizableStatus(this.Info.RawData.api_tyku, this.RawData.api_kyouka[2]);
				this.Armor = new ModernizableStatus(this.Info.RawData.api_souk, this.RawData.api_kyouka[3]);
				this.Luck = new ModernizableStatus(this.Info.RawData.api_luck, this.RawData.api_kyouka[4]);
			}

			this.TimeToRepair = TimeSpan.FromMilliseconds(this.RawData.api_ndock_time);
			this.UpdateSlots();
		}

		public void UpdateSlots()
		{
			this.Slots = this.RawData.api_slot
				.Select(id => this.homeport.Itemyard.SlotItems[id])
				.Select((t, i) => new ShipSlot(this, t, this.Info.RawData.api_maxeq.Get(i) ?? 0, this.RawData.api_onslot.Get(i) ?? 0))
				.ToArray();
			this.ExSlot = new ShipSlot(this, this.homeport.Itemyard.SlotItems[this.RawData.api_slot_ex], 0, 0);

			if (this.Slots.Any(x => x.Item.Info.Type == SlotItemType.DamageControl))
				this.Situation |= ShipSituation.DamageControlled;
			else
				this.Situation &= ~ShipSituation.DamageControlled;

			this.ASW = this.ASW.Update(this.ASW.Upgraded - this.Slots.Sum(slot => slot.Item.Info.ASW));
		}

		internal void Charge(int fuel, int bull, int[] onslot)
		{
			this.Fuel = this.Fuel.Update(fuel);
			this.Bull = this.Bull.Update(bull);
			for (var i = 0; i < this.Slots.Length; i++) this.Slots[i].Current = onslot.Get(i) ?? 0;
		}

		internal void Repair()
		{
			var max = this.HP.Maximum;
			this.HP = this.HP.Update(max);
		}

		public override string ToString()
			=> $"ID = {this.Id}, Name = \"{this.Info.Name}\", ShipType = \"{this.Info.ShipType.Name}\", Level = {this.Level}";
	}
}
