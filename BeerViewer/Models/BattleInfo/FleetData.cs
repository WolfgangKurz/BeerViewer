using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Core;
using BeerViewer.Models;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models.BattleInfo
{
	public class FleetData : Notifier
	{
		#region FleetType 프로퍼티
		private FleetType _FleetType;
		public FleetType FleetType
		{
			get { return this._FleetType; }
			set
			{
				if (this._FleetType != value)
				{
					this._FleetType = value;
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

		#region AttackGauge 프로퍼티
		private string _AttackGauge;
		public string AttackGauge
		{
			get { return this._AttackGauge; }
			set
			{
				if (this._AttackGauge != value)
				{
					this._AttackGauge = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region IsCritical 프로퍼티
		private bool _IsCritical;
		public bool IsCritical
		{
			get { return this._IsCritical; }
			set
			{
				if (this._IsCritical != value)
				{
					this._IsCritical = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region TotalDamaged 프로퍼티
		private int _TotalDamaged;
		public int TotalDamaged
		{
			get { return this._TotalDamaged; }
			set
			{
				if (value == 0) this.SinkCount = 0;

				if (this._TotalDamaged != value)
				{
					this._TotalDamaged = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region SinkCount 프로퍼티
		private int _SinkCount;
		public int SinkCount
		{
			get { return this._SinkCount; }
			set
			{
				if (this._SinkCount != value)
				{
					this._SinkCount = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Ships 프로퍼티
		private IEnumerable<ShipData> _Ships;
		public IEnumerable<ShipData> Ships
		{
			get { return this._Ships; }
			set
			{
				if (this._Ships != value)
				{
					this._Ships = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Formation 프로퍼티
		private Formation _Formation;
		public Formation Formation
		{
			get { return this._Formation; }
			set
			{
				if (this._Formation != value)
				{
					this._Formation = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Rank 프로퍼티
		private int[] _Rank;
		public int[] Rank
		{
			get
			{ return this._Rank; }
			set
			{
				if (this._Rank != value)
				{
					this._Rank = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public FleetData() : this(new ShipData[0], Formation.없음, "", FleetType.Enemy)
		{
		}

		public FleetData(IEnumerable<ShipData> ships, Formation formation, string name, FleetType type, int[] rank = null)
		{
			this._Ships = ships;
			this._Formation = formation;
			this._Name = name;
			this._FleetType = type;
			this._Rank = rank ?? new[] { 0 };

			if (type == FleetType.Enemy || type == FleetType.SecondEnemy) return;
			this.IsCritical = this.Ships?
				.Any(x => (x.NowHP / (double)x.MaxHP <= 0.25) && (x.NowHP / (double)x.MaxHP > 0))
				?? false;
		}
	}

	public static class FleetDataExtensions
	{
		/// <summary>
		/// Actionを使用して値を設定
		/// Zipするので要素数が少ない方に合わせられる
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="source"></param>
		/// <param name="values"></param>
		/// <param name="setter"></param>
		public static void SetValues<TSource, TValue>(
			this IEnumerable<TSource> source,
			IEnumerable<TValue> values,
			Action<TSource, TValue> setter)
		{
			source.Zip(values, (s, v) => new { s, v })
				.ToList()
				.ForEach(x => setter(x.s, x.v));
		}

		/// <summary>
		/// ダメージ適用
		/// </summary>
		/// <param name="fleet">艦隊</param>
		/// <param name="damages">適用ダメージリスト</param>
		public static void CalcDamages(this FleetData fleet, params FleetDamages[] damages)
		{
			foreach (var damage in damages)
				fleet.Ships.SetValues(damage.ToArray(), (s, d) => s.NowHP -= d);

			if (fleet.Ships != null)
			{
				foreach (var item in fleet.Ships)
				{
					if (!item.Situation.HasFlag(ShipSituation.Evacuation) && !item.Situation.HasFlag(ShipSituation.Tow))
					{
						int tempHP = item.NowHP;
						if (item.NowHP < 0) tempHP = 0;
						fleet.TotalDamaged += (item.BeforeNowHP - tempHP);
					}
					if (item.NowHP <= 0) fleet.SinkCount++;
				}

				fleet.IsCritical = fleet.Ships
					.Where(x => x.MaxHP > 0)
					.Where(x => (x.NowHP / (double)x.MaxHP) <= 0.25)
					.Any(x => !x.Situation.HasFlag(ShipSituation.DamageControlled) &&
							  !x.Situation.HasFlag(ShipSituation.Evacuation) &&
							  !x.Situation.HasFlag(ShipSituation.Tow)
					);
			}
			foreach (var damage in damages)
			{
				// 다메콘에 의한 회복 처리. 같은 전투에서 두 번 사용되지 않는다는 전제...
				// 다메콘 우선도: 확장 슬롯 -> 인덱스 순
				var dameconState = fleet.Ships.Select(x => new { HasDamecon = x.HasDamecon(), HasMegami = x.HasMegami() });
				fleet.Ships.SetValues(dameconState, (s, d) =>
				{
					if (0 < s.NowHP) return;
					s.IsUsedDamecon = d.HasDamecon || d.HasMegami;

					if (d.HasDamecon) s.NowHP = (int)Math.Floor(s.MaxHP * 0.2);
					else if (d.HasMegami) s.NowHP = s.MaxHP;
				});
			}
			InputCritical(fleet);
		}
		public static void InputCritical(FleetData fleet)
		{
			if (fleet.FleetType == FleetType.Enemy || fleet.FleetType == FleetType.SecondEnemy) return;
			else if (fleet.FleetType == FleetType.First) Settings.BattleInfo_FirstIsCritical.Value = fleet.IsCritical;
			else if (fleet.FleetType == FleetType.Second) Settings.BattleInfo_SecondIsCritical.Value = fleet.IsCritical;
		}

		/// <summary>
		/// 演習ダメージ適用
		/// </summary>
		/// <param name="fleet">艦隊</param>
		/// <param name="damages">適用ダメージリスト</param>
		public static void CalcPracticeDamages(this FleetData fleet, params FleetDamages[] damages)
		{
			foreach (var damage in damages)
			{
				fleet.Ships.SetValues(damage.ToArray(), (s, d) => s.NowHP -= d);
			}
			if (fleet.Ships == null) return;
			foreach (var item in fleet.Ships)
			{
				int tempHP = item.NowHP;
				if (item.NowHP < 0) tempHP = 0;
				fleet.TotalDamaged += (item.BeforeNowHP - tempHP);
				if (item.NowHP <= 0) fleet.SinkCount++;
			}
		}

		private static bool HasDamecon(this ShipData ship)
		{
			return ship?.ExSlot?.Source?.Id == 42
				|| ship?.FirstDameconOrNull()?.Source?.Id == 42;
		}

		private static bool HasMegami(this ShipData ship)
		{
			return ship?.ExSlot?.Source?.Id == 43
				|| ship?.FirstDameconOrNull()?.Source?.Id == 43;
		}

		private static ShipSlotData FirstDameconOrNull(this ShipData ship)
		{
			return ship?.Slots?.FirstOrDefault(x => x?.Source?.Id == 42 || x?.Source?.Id == 43);
		}
		public static bool CriticalCheck(this FleetData fleet)
		{
			if (fleet.Ships
					.Where(x => !x.Situation.HasFlag(ShipSituation.DamageControlled))
					.Where(x => !x.Situation.HasFlag(ShipSituation.Evacuation))
					.Where(x => !x.Situation.HasFlag(ShipSituation.Tow))
					.Any(x => x.Situation.HasFlag(ShipSituation.HeavilyDamaged)))
				return true;
			else return false;
		}
	}
}
