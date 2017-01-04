using System;
using System.Collections.Generic;
using System.Linq;

using BeerViewer.Core;
using BeerViewer.Models;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models.BattleInfo
{
	public class ShipData : Notifier
	{
		#region Id 프로퍼티
		private int _Id;
		public int Id
		{
			get { return this._Id; }
			set
			{
				if (this._Id != value)
				{
					this._Id = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Name 프로퍼티
		private string _Name;
		public string Name
		{
			get { return this._Name; }
			set
			{
				if (this._Name != value)
				{
					this._Name = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region AdditionalName 프로퍼티
		private string _AdditionalName;
		public string AdditionalName
		{
			get { return this._AdditionalName; }
			set
			{
				if (this._AdditionalName != value)
				{
					this._AdditionalName = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region TypeName 프로퍼티
		private string _TypeName;
		public string TypeName
		{
			get { return this._TypeName; }
			set
			{
				if (this._TypeName != value)
				{
					this._TypeName = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Level 프로퍼티
		private int _Level;
		public int Level
		{
			get { return this._Level; }
			set
			{
				if (this._Level != value)
				{
					this._Level = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Situation 프로퍼티
		private ShipSituation _Situation;
		public ShipSituation Situation
		{
			get { return _Situation; }
			set
			{
				if (_Situation != value)
				{
					_Situation = value;
					RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region MaxHP 프로퍼티
		private int _MaxHP;
		public int MaxHP
		{
			get { return this._MaxHP; }
			set
			{
				if (this._MaxHP != value)
				{
					this._MaxHP = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(nameof(this.HP));
				}
			}
		}
		#endregion

		#region NowHP 프로퍼티
		private int _NowHP;
		public int NowHP
		{
			get { return this._NowHP; }
			set
			{
				if (this._NowHP != value)
				{
					this._NowHP = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(nameof(this.HP));
				}
			}
		}
		#endregion

		public int BeforeNowHP { get; set; }

		#region Firepower 프로퍼티
		private int _Firepower;
		public int Firepower
		{
			get { return this._Firepower; }
			set
			{
				if (this._Firepower != value)
				{
					this._Firepower = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Torpedo 프로퍼티
		private int _Torpedo;
		public int Torpedo
		{
			get { return this._Torpedo; }
			set
			{
				if (this._Torpedo != value)
				{
					this._Torpedo = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region AA 프로퍼티
		private int _AA;
		public int AA
		{
			get { return this._AA; }
			set
			{
				if (this._AA != value)
				{
					this._AA = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Armor 프로퍼티
		private int _Armor;
		public int Armor
		{
			get { return this._Armor; }
			set
			{
				if (this._Armor != value)
				{
					this._Armor = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Luck 프로퍼티
		private int _Luck;
		public int Luck
		{
			get { return this._Luck; }
			set
			{
				if (this._Luck != value)
				{
					this._Luck = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Slots 프로퍼티
		private IEnumerable<ShipSlotData> _Slots;
		public IEnumerable<ShipSlotData> Slots
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

		#region ExSlot 프로퍼티
		private ShipSlotData _ExSlot;
		public ShipSlotData ExSlot
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

		#region IsUsedDamecon 프로퍼티
		private bool _IsUsedDamecon;
		public bool IsUsedDamecon
		{
			get { return this._IsUsedDamecon; }
			set
			{
				if (this._IsUsedDamecon != value)
				{
					this._IsUsedDamecon = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region IsMvp 프로퍼티
		private bool _IsMvp;
		public bool IsMvp
		{
			get { return this._IsMvp; }
			set
			{
				if (this._IsMvp != value)
				{
					this._IsMvp = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public int SlotsFirepower => this.Slots.Sum(x => x.Firepower);
		public int SlotsTorpedo => this.Slots.Sum(x => x.Torpedo);
		public int SlotsAA => this.Slots.Sum(x => x.AA);
		public int SlotsArmor => this.Slots.Sum(x => x.Armor);
		public int SlotsASW => this.Slots.Sum(x => x.ASW);
		public int SlotsHit => this.Slots.Sum(x => x.Hit);
		public int SlotsEvade => this.Slots.Sum(x => x.Evade);

		public int SumFirepower => 0 < this.Firepower ? this.Firepower + this.SlotsFirepower : this.Firepower;
		public int SumTorpedo => 0 < this.Torpedo ? this.Torpedo + this.SlotsTorpedo : this.Torpedo;
		public int SumAA => 0 < this.AA ? this.AA + this.SlotsAA : this.AA;
		public int SumArmor => 0 < this.Armor ? this.Armor + this.SlotsArmor : this.Armor;

		public LimitedValue HP => new LimitedValue(this.NowHP, this.MaxHP, 0);

		public AttackType DayAttackType
			=> this.HasScout() && this.Count(Type2.주포) == 2 && this.Count(Type2.철갑탄) == 1 ? AttackType.주주컷인
			: this.HasScout() && this.Count(Type2.주포) == 1 && this.Count(Type2.부포) == 1 && this.Count(Type2.철갑탄) == 1 ? AttackType.주철컷인
			: this.HasScout() && this.Count(Type2.주포) == 1 && this.Count(Type2.부포) == 1 && this.Count(Type2.전탐) == 1 ? AttackType.주전컷인
			: this.HasScout() && this.Count(Type2.주포) >= 1 && this.Count(Type2.부포) >= 1 ? AttackType.주부컷인
			: this.HasScout() && this.Count(Type2.주포) >= 2 ? AttackType.연격
			: AttackType.통상;

		public AttackType NightAttackType
			=> this.Count(Type2.어뢰) >= 2 ? AttackType.뇌격컷인
			: this.Count(Type2.주포) >= 3 ? AttackType.주주주컷인
			: this.Count(Type2.주포) == 2 && this.Count(Type2.부포) >= 1 ? AttackType.주주부컷인
			: this.Count(Type2.주포) == 2 && this.Count(Type2.부포) == 0 && this.Count(Type2.어뢰) == 1 ? AttackType.주뢰컷인
			: this.Count(Type2.주포) == 1 && this.Count(Type2.어뢰) == 1 ? AttackType.주뢰컷인
			: this.Count(Type2.주포) == 2 && this.Count(Type2.부포) == 0 && this.Count(Type2.어뢰) == 0 ? AttackType.연격
			: this.Count(Type2.주포) == 1 && this.Count(Type2.부포) >= 1 && this.Count(Type2.어뢰) == 0 ? AttackType.연격
			: this.Count(Type2.주포) == 0 && this.Count(Type2.부포) >= 2 && this.Count(Type2.어뢰) <= 1 ? AttackType.연격
			: AttackType.통상;

		public ShipData()
		{
			this._Name = "???";
			this._AdditionalName = "";
			this._TypeName = "???";
			this._Situation = ShipSituation.None;
			this._Slots = new ShipSlotData[0];
		}
	}

	public static class ShipDataExtensions
	{
		public static int Count(this ShipData data, Type2 type2)
		{
			return data.Slots.Count(x => x.Type2 == type2);
		}

		public static bool HasScout(this ShipData data)
		{
			return data.Slots
				.Where(x => x.Source.Type == SlotItemType.水上偵察機
							|| x.Source.Type == SlotItemType.水上爆撃機)
				.Any(x => 0 < x.Current);
		}
	}

	public class MembersShipData : ShipData
	{
		#region Source 프로퍼티
		private Ship _Source;
		public Ship Source
		{
			get { return this._Source; }
			set
			{
				if (this._Source != value)
				{
					this._Source = value;
					this.RaisePropertyChanged();
					this.UpdateFromSource();
				}
			}
		}
		#endregion

		public MembersShipData()
		{
		}

		public MembersShipData(Ship ship) : this()
		{
			this._Source = ship;
			this.UpdateFromSource();
		}

		private void UpdateFromSource()
		{
			this.Id = this.Source.Id;
			this.Name = this.Source.Info.Name;
			this.TypeName = this.Source.Info.ShipType.Name;
			this.Level = this.Source.Level;
			this.Situation = this.Source.Situation;
			this.NowHP = this.Source.HP.Current;
			this.MaxHP = this.Source.HP.Maximum;
			this.Slots = this.Source.Slots
				.Where(s => s != null)
				.Where(s => s.Equipped)
				.Select(s => new ShipSlotData(s))
				.ToArray();
			this.ExSlot = new ShipSlotData(this.Source.ExSlot);

			this.Firepower = this.Source.Firepower.Current;
			this.Torpedo = this.Source.Torpedo.Current;
			this.AA = this.Source.AA.Current;
			this.Armor = this.Source.Armor.Current;
			this.Luck = this.Source.Luck.Current;
		}
	}

	public class MastersShipData : ShipData
	{
		#region Source 프로퍼티
		private ShipInfo _Source;
		public ShipInfo Source
		{
			get { return this._Source; }
			set
			{
				if (this._Source != value)
				{
					this._Source = value;
					this.RaisePropertyChanged();
					this.UpdateFromSource();
				}
			}
		}
		#endregion

		public MastersShipData()
		{
		}

		public MastersShipData(ShipInfo info) : this()
		{
			this._Source = info;
			this.UpdateFromSource();
		}

		private void UpdateFromSource()
		{
			this.Id = this.Source.Id;
			this.Name = this.Source.Name;

			var isEnemyID = 500 < this.Source.Id && this.Source.Id < 901;
			var m = DataStorage.Instance.Master.Ships
				.Select(x => x.Value)
				.Single(x => x.Id == this.Source.Id);

			this.AdditionalName = isEnemyID ? m.Kana : "";
			this.TypeName = this.Source.ShipType.Name;
		}
	}
}
