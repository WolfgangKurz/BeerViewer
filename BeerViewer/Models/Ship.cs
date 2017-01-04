using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Core;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	/// <summary>
	/// 모항에 소지하고 있는 칸무스 데이터
	/// </summary>
	public class Ship : RawDataWrapper<kcsapi_ship2>, IIdentifiable
	{
		private readonly Homeport homeport;

		/// <summary>
		/// 이 칸무스를 식별하는 Id
		/// </summary>
		public int Id => this.RawData.api_id;
		public int FleetId
		{
			get
			{
				try
				{
					foreach (var fleet in homeport.Organization.Fleets)
						foreach (var ship in fleet.Value.Ships)
							if (ship.Id == this.Id)
								return fleet.Value.Id;
				}
				catch (Exception e)
				{
					Debug.WriteLine(e);
					return -1;
				}
				return -1;
			}
		}
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

		public string LvName => "[Lv." + this.Level + "]  " + this.Info.Name;
		public string RepairTimeString
		{
			get
			{
				TimeSpan? Remaining = new TimeSpan(0, 0, 0, 0, (int)this.TimeToRepair.TotalMilliseconds);
				return Remaining.HasValue
			? string.Format("{0:D2}:{1}",
				(int)Remaining.Value.TotalHours,
				Remaining.Value.ToString(@"mm\:ss"))
			: "--:--:--";
			}
		}
		/// <summary>
		/// 칸무스 도감 정보
		/// </summary>
		public ShipInfo Info { get; private set; }
		public int SortNumber => this.RawData.api_sortno;

		public int Level => this.RawData.api_lv;

		/// <summary>
		/// 자물쇠 여부
		/// </summary>
		public bool IsLocked => this.RawData.api_locked == 1;

		public int Exp => this.RawData.api_exp.Get(0) ?? 0;
		public int ExpForNextLevel => this.RawData.api_exp.Get(1) ?? 0;

		public int ExpForNextRemodelingLevel => Math.Max(Experience.GetShipExpForSpecifiedLevel(this.Exp, this.Info?.NextRemodelingLevel), 0);
		public int ExpForMarrige => Math.Max(Experience.GetShipExpForSpecifiedLevel(this.Exp, 99), 0);
		public int ExpForLevelMax => Experience.GetShipExpForSpecifiedLevel(this.Exp, 155);

		/// <summary>
		/// ExSlot 이 존재하는지 여부
		/// </summary>
		public bool ExSlotExists => this.RawData.api_slot_ex != 0;

		#region HP 프로퍼티
		private LimitedValue _HP;
		public LimitedValue HP
		{
			get { return this._HP; }
			private set
			{
				this._HP = value;
				this.RaisePropertyChanged();

				if (value.IsHeavilyDamage())
				{
					this.Situation |= ShipSituation.HeavilyDamaged;
				}
				else
				{
					this.Situation &= ~ShipSituation.HeavilyDamaged;
				}
			}
		}
		#endregion

		#region Fuel 프로퍼티
		private LimitedValue _Fuel;
		public LimitedValue Fuel
		{
			get { return this._Fuel; }
			private set
			{
				this._Fuel = value;
				this.RaisePropertyChanged();
				this.RaisePropertyChanged("UsedFuel");
			}
		}
		#endregion

		#region Bull 프로퍼티
		private LimitedValue _Bull;
		public LimitedValue Bull
		{
			get { return this._Bull; }
			private set
			{
				this._Bull = value;
				this.RaisePropertyChanged();
				this.RaisePropertyChanged("UsedBull");
			}
		}
		#endregion

		#region Firepower 프로퍼티
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

		#region Torpedo 프로퍼티
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

		#region YasenFp 프로퍼티
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

		#region AA 프로퍼티
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

		#region Armor 프로퍼티
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

		#region ASW 프로퍼티
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

		#region Luck 프로퍼티
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

		#region Slots 프로퍼티
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

		#region EquippedSlots 프로퍼티
		private ShipSlot[] _EquippedSlots;
		public ShipSlot[] EquippedItems
		{
			get { return this._EquippedSlots; }
			set
			{
				if (this._EquippedSlots != value)
				{
					this._EquippedSlots = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region ExSlot 프로퍼티
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

		#region TimeToRepair 프로퍼티
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

		#region Used 의존변수
		public int UsedFuel => (int)((this.Level <= 99 ? 1.0f : 0.85f) * (this.Fuel.Maximum - this.Fuel.Current));
		public int UsedBull => (int)((this.Level <= 99 ? 1.0f : 0.85f) * (this.Bull.Maximum - this.Bull.Current));
		#endregion

		public int ViewRange => this.RawData.api_sakuteki.Get(0) ?? 0;
		public bool IsMaxModernized => this.Firepower.IsMax && this.Torpedo.IsMax && this.AA.IsMax && this.Armor.IsMax;

		public int Condition => this.RawData.api_cond;
		public ConditionType ConditionType => ConditionTypeHelper.ToConditionType(this.RawData.api_cond);

		public int SallyArea => this.RawData.api_sally_area;

		#region Status 프로퍼티
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

		#region IntStatus 프로퍼티
		private int _IntStatus;
		public int IntStatus
		{
			get { return this._IntStatus; }
			set
			{
				if (this._IntStatus != value)
				{
					this._IntStatus = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		internal Ship(Homeport parent, kcsapi_ship2 RawData) : base(RawData)
		{
			this.homeport = parent;
			this.Update(RawData);
		}

		internal void Update(kcsapi_ship2 rawData)
		{
			this.UpdateRawData(rawData);

			this.Info = DataStorage.Instance.Master.Ships[rawData.api_ship_id] ?? ShipInfo.Dummy;
			this.HP = new LimitedValue(this.RawData.api_nowhp, this.RawData.api_maxhp, 0);
			this.Fuel = new LimitedValue(this.RawData.api_fuel, this.Info.RawData.api_fuel_max, 0);
			this.Bull = new LimitedValue(this.RawData.api_bull, this.Info.RawData.api_bull_max, 0);
			this.ASW = new ModernizableStatus(new int[] { 0, this.RawData.api_taisen[1] }, this.RawData.api_taisen[0]);

			double temp = (double)this.HP.Current / (double)this.HP.Maximum;

			if (temp <= 0.25) IntStatus = 3;
			else if (temp <= 0.5) IntStatus = 2;
			else if (temp <= 0.75) IntStatus = 1;
			else IntStatus = 0;

			if (this.RawData.api_kyouka.Length >= 5)
			{
				this.Firepower = new ModernizableStatus(this.Info.RawData.api_houg, this.RawData.api_kyouka[0]);
				this.Torpedo = new ModernizableStatus(this.Info.RawData.api_raig, this.RawData.api_kyouka[1]);
				this.YasenFp = new ModernizableStatus(
					new int[] {
						this.Info.RawData.api_houg[0] + this.Info.RawData.api_raig[0],
						this.Info.RawData.api_houg[1] + this.Info.RawData.api_raig[1]},
					this.RawData.api_kyouka[0] + this.RawData.api_kyouka[1]);
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
			this.EquippedItems = this.EnumerateAllEquippedItems().ToArray();

			if (this.EquippedItems.Any(x => x.Item.Info.Type == SlotItemType.応急修理要員))
			{
				this.Situation |= ShipSituation.DamageControlled;
			}
			else
			{
				this.Situation &= ~ShipSituation.DamageControlled;
			}

			//장비의 대잠 수치 제외
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
		{
			return $"ID = {this.Id}, Name = \"{this.Info.Name}\", ShipType = \"{this.Info.ShipType.Name}\", Level = {this.Level}";
		}

		private IEnumerable<ShipSlot> EnumerateAllEquippedItems()
		{
			foreach (var slot in this.Slots.Where(x => x.Equipped)) yield return slot;
			if (this.ExSlot.Equipped) yield return this.ExSlot;
		}
	}
}
