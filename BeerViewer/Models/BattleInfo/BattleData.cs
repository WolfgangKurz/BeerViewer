using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Core;
using BeerViewer.Models.Raw;
using BeerViewer.Models.BattleInfo.Raw;

namespace BeerViewer.Models.BattleInfo
{
	public class BattleData : Notifier
	{
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

		#region UpdatedTime 프로퍼티
		private DateTimeOffset _UpdatedTime;
		public DateTimeOffset UpdatedTime
		{
			get { return this._UpdatedTime; }
			set
			{
				if (this._UpdatedTime != value)
				{
					this._UpdatedTime = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region CellData 프로퍼티
		private MapCellData _CellData { get; set; } = null;
		public MapCellData CellData
		{
			get { return this._CellData; }
			set
			{
				if (this._CellData != value)
				{
					this._CellData = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region BattleSituation 프로퍼티
		private BattleSituation _BattleSituation;
		public BattleSituation BattleSituation
		{
			get { return this._BattleSituation; }
			set
			{
				if (this._BattleSituation != value)
				{
					this._BattleSituation = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region FirstFleet 프로퍼티
		private FleetData _FirstFleet;
		public FleetData FirstFleet
		{
			get { return this._FirstFleet; }
			set
			{
				if (this._FirstFleet != value)
				{
					this._FirstFleet = value;
					this._FirstFleet.FleetType = FleetType.First;
					Settings.BattleInfo_FirstIsCritical.Value = this._FirstFleet.CriticalCheck();
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region SecondFleet 프로퍼티
		private FleetData _SecondFleet;
		public FleetData SecondFleet
		{
			get { return this._SecondFleet; }
			set
			{
				if (this._SecondFleet != value)
				{
					this._SecondFleet = value;
					this._SecondFleet.FleetType = FleetType.Second;
					Settings.BattleInfo_SecondIsCritical.Value = this._SecondFleet.CriticalCheck();
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Enemies 프로퍼티
		private FleetData _Enemies;
		public FleetData Enemies
		{
			get { return this._Enemies; }
			set
			{
				if (this._Enemies != value)
				{
					this._Enemies = value;
					this._Enemies.FleetType = FleetType.Enemy;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region SecondEnemies 프로퍼티
		private FleetData _SecondEnemies;
		public FleetData SecondEnemies
		{
			get { return this._SecondEnemies; }
			set
			{
				if (this._SecondEnemies != value)
				{
					this._SecondEnemies = value;
					this._SecondEnemies.FleetType = FleetType.SecondEnemy;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region RankResult 프로퍼티
		private BattleRank _RankResult;
		public BattleRank RankResult
		{
			get { return this._RankResult; }
			set
			{
				if (this._RankResult != value)
				{
					this._RankResult = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region AirRankResult 프로퍼티
		private BattleRank _AirRankResult;
		public BattleRank AirRankResult
		{
			get { return this._AirRankResult; }
			set
			{
				if (this._AirRankResult != value)
				{
					this._AirRankResult = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region FriendAirSupremacy 프로퍼티
		private AirSupremacy _FriendAirSupremacy = AirSupremacy.항공전없음;
		public AirSupremacy FriendAirSupremacy
		{
			get { return this._FriendAirSupremacy; }
			set
			{
				if (this._FriendAirSupremacy != value)
				{
					this._FriendAirSupremacy = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region AirCombatResults 프로퍼티
		private AirCombatResult[] _AirCombatResults = new AirCombatResult[0];
		public AirCombatResult[] AirCombatResults
		{
			get { return this._AirCombatResults; }
			set
			{
				if (!this._AirCombatResults.Equals(value))
				{
					this._AirCombatResults = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region DropShipName 프로퍼티
		private string _DropShipName;
		public string DropShipName
		{
			get { return this._DropShipName; }
			set
			{
				if (this._DropShipName != value)
				{
					this._DropShipName = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region FlareUsed 프로퍼티
		private UsedFlag _FlareUsed;
		public UsedFlag FlareUsed
		{
			get { return this._FlareUsed; }
			set
			{
				if (this._FlareUsed != value)
				{
					this._FlareUsed = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region NightReconScouted 프로퍼티
		private UsedFlag _NightReconScouted;
		public UsedFlag NightReconScouted
		{
			get { return this._NightReconScouted; }
			set
			{
				if (this._NightReconScouted != value)
				{
					this._NightReconScouted = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region AntiAirFired 프로퍼티
		private AirFireFlag _AntiAirFired;
		public AirFireFlag AntiAirFired
		{
			get { return this._AntiAirFired; }
			set
			{
				if (this._AntiAirFired != value)
				{
					this._AntiAirFired = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region SupportUsed 프로퍼티
		private UsedSupport _SupportUsed;
		public UsedSupport SupportUsed
		{
			get { return this._SupportUsed; }
			set
			{
				if (this._SupportUsed != value)
				{
					this._SupportUsed = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion


		private int CurrentDeckId { get; set; }

		public BattleData()
		{
			if (Helper.IsInDesignMode) return;

			var proxy = Proxy.Instance;

			proxy.Register(Proxy.api_port, e =>
			{
				var x = e.TryParse<battle_midnight_battle>();
				if (x == null) return;

				this.ResultClear();
			});

			proxy.Register("/kcsapi/api_req_battle_midnight/battle", e =>
			{
				var x = e.TryParse<battle_midnight_battle>();
				if (x == null) return;

				this.Update(x.Data);
			});
			proxy.Register("/kcsapi/api_req_battle_midnight/sp_midnight", e=>{
				var x = e.TryParse<battle_midnight_sp_midnight>();
				if(x == null) return;

				this.Update(x.Data);
			});
			proxy.Register(Proxy.api_req_combined_battle_airbattle, e=>{
				var x = e.TryParse<combined_battle_airbattle>();
				if(x == null) return;

				this.Update(x.Data);
			});
			proxy.Register(Proxy.api_req_combined_battle_battle, e=>{
				var x = e.TryParse<combined_battle_battle>();
				if(x == null) return;

				this.Update(x.Data);
			});
			proxy.Register("/kcsapi/api_req_combined_battle/battle_water", e=>{
				var x = e.TryParse<combined_battle_battle_water>();
				if(x == null) return;

				this.Update(x.Data);
			});
			proxy.Register("/kcsapi/api_req_combined_battle/midnight_battle", e=>{
				var x = e.TryParse<combined_battle_midnight_battle>();
				if(x == null) return;

				this.Update(x.Data);
			});
			proxy.Register("/kcsapi/api_req_combined_battle/sp_midnight", e=>{
				var x = e.TryParse<combined_battle_sp_midnight>();
				if(x == null) return;

				this.Update(x.Data);
			});
			proxy.Register("/kcsapi/api_req_practice/battle", e=>{
				var x = e.TryParse<practice_battle>();
				if(x == null) return;

				this.Update(x.Data);
			});
			proxy.Register("/kcsapi/api_req_practice/midnight_battle", e=>{
				var x = e.TryParse<practice_midnight_battle>();
				if(x == null) return;

				this.Update(x.Data);
			});
			proxy.Register("/kcsapi/api_req_practice/battle_result", e=>{
				var x = e.TryParse<battle_result>();
				if(x == null) return;

				this.Update(x.Data);
			});
			proxy.Register("/kcsapi/api_req_sortie/airbattle", e=>{
				var x = e.TryParse<sortie_airbattle>();
				if(x == null) return;

				this.Update(x.Data);
			});
			proxy.Register(Proxy.api_req_sortie_battle, e=>{
				var x = e.TryParse<sortie_battle>();
				if(x == null) return;

				this.Update(x.Data);
			});
			proxy.Register("/kcsapi/api_req_sortie/ld_airbattle", e=>{
				var x = e.TryParse<sortie_ld_airbattle>();
				if(x == null) return;

				this.Update(x.Data);
			});
			proxy.Register("/kcsapi/api_req_combined_battle/ld_airbattle", e=>{
				var x = e.TryParse<combined_battle_ld_airbattle>();
				if(x == null) return;

				this.Update(x.Data);
			});
			proxy.Register("/kcsapi/api_req_combined_battle/ec_battle", e=>{
				var x = e.TryParse<combined_battle_each_battle>();
				if(x == null) return;

				this.Update(x.Data, false);
			});
			proxy.Register("/kcsapi/api_req_combined_battle/each_battle", e=>{
				var x = e.TryParse<combined_battle_each_battle>();
				if(x == null) return;

				this.Update(x.Data, true);
			});
			proxy.Register("/kcsapi/api_req_combined_battle/ec_midnight_battle", e=>{
				var x = e.TryParse<combined_battle_ec_midnight_battle>();
				if(x == null) return;

				this.Update(x.Data);
			});

			proxy.Register(Proxy.api_req_sortie_battleresult, e=>{
				var x = e.TryParse<battle_result>();
				if(x == null) return;

				this.Update(x.Data);
			});
			proxy.Register(Proxy.api_req_combined_battle_battleresult, e=>{
				var x = e.TryParse<battle_result>();
				if(x == null) return;

				this.Update(x.Data);
			});

			proxy.Register("/kcsapi/api_req_map/start", e=>{
				var x = e.TryParse<map_start_next>();
				if(x == null) return;

				this.UpdateFleetsByStartNext(x.Data, x.Request["api_deck_id"]);
			});
			proxy.Register("/kcsapi/api_req_map/next", e=>{
				var x = e.TryParse<map_start_next>();
				if(x == null) return;

				this.UpdateFleetsByStartNext(x.Data);
			});
		}

		private void AutoSelectTab()
		{
			if (Settings.BattleInfo_AutoSelectTab.Value)
				frmMain.Instance.UpdateTab("Battle");
		}

		#region Update From Battle SvData
		public void Update(battle_midnight_battle data)
		{
			AutoSelectTab();
			this.Name = "통상 - 야전";

			int BeforedayBattleHP = this.FirstFleet.Ships
				.Where(x => !x.Situation.HasFlag(ShipSituation.Tow) && !x.Situation.HasFlag(ShipSituation.Evacuation))
				.Sum(x => x.BeforeNowHP);//리스트 갱신하기전에 아군 HP최대값을 저장
			int EnemyBeforedayBattle = this.Enemies.Ships.Sum(x => x.BeforeNowHP);

			this.UpdateFleets(data.api_deck_id, data);
			this.UpdateMaxHP(data.api_maxhps);
			this.UpdateNowHP(data.api_nowhps);

			this.UpdateUsedFlag(data.api_flare_pos, data.api_touch_plane);

			this.FirstFleet.CalcDamages(data.api_hougeki.GetFriendDamages());

			this.Enemies.CalcDamages(data.api_hougeki.GetEnemyDamages());

			this.RankResult = this.CalcRank(false, true, BeforedayBattleHP, EnemyBeforedayBattle);
		}

		public void Update(battle_midnight_sp_midnight data)
		{
			AutoSelectTab();
			this.Name = "통상 - 개막야전";

			this.UpdateFleets(data.api_deck_id, data, data.api_formation);
			this.UpdateMaxHP(data.api_maxhps);
			this.UpdateNowHP(data.api_nowhps);

			this.UpdateUsedFlag(data.api_flare_pos, data.api_touch_plane);

			this.FirstFleet.CalcDamages(data.api_hougeki.GetFriendDamages());

			this.Enemies.CalcDamages(data.api_hougeki.GetEnemyDamages());

			this.FriendAirSupremacy = AirSupremacy.항공전없음;

			this.RankResult = this.CalcRank();
		}

		public void Update(combined_battle_airbattle data)
		{
			AutoSelectTab();
			this.Name = "연합함대 - 항공전 - 주간";

			this.UpdateFleets(data.api_deck_id, data, data.api_formation);
			this.UpdateMaxHP(data.api_maxhps, data.api_maxhps_combined);
			this.UpdateNowHP(data.api_nowhps, data.api_nowhps_combined);

			this.UpdateUsedFlag(data.api_kouku?.api_stage2?.api_air_fire, data.api_kouku2?.api_stage2?.api_air_fire);
			this.UpdateUsedFlag(data.api_support_info);

			this.FirstFleet.CalcDamages(
				data.api_kouku.GetFirstFleetDamages(),
				data.api_kouku2.GetFirstFleetDamages()
				);

			this.SecondFleet.CalcDamages(
				data.api_kouku.GetSecondFleetDamages(),
				data.api_kouku2.GetSecondFleetDamages()
				);

			this.Enemies.CalcDamages(
				data.api_air_base_attack.GetEnemyDamages(),
				data.api_support_info.GetEnemyDamages(),
				data.api_kouku.GetEnemyDamages(),
				data.api_kouku2.GetEnemyDamages()
				);

			this.FriendAirSupremacy = data.api_kouku.GetAirSupremacy(); //航空戦2回目はスルー

			this.AirCombatResults = data.api_air_base_attack.ToResult().Concat(data.api_kouku.ToResult("1회차/"))
							.Concat(data.api_kouku2.ToResult("2회차/")).ToArray();

			this.RankResult = this.CalcRank(true);
		}

		public void Update(combined_battle_battle data)
		{
			AutoSelectTab();
			this.Name = "연합함대 - 기동부대 - 주간전";

			this.UpdateFleets(data.api_deck_id, data, data.api_formation);
			this.UpdateMaxHP(data.api_maxhps, data.api_maxhps_combined);
			this.UpdateNowHP(data.api_nowhps, data.api_nowhps_combined);

			this.UpdateUsedFlag(data.api_kouku?.api_stage2?.api_air_fire);
			this.UpdateUsedFlag(data.api_support_info);

			this.FirstFleet.CalcDamages(
				data.api_kouku.GetFirstFleetDamages(),
				data.api_hougeki2.GetFriendDamages(),
				data.api_hougeki3.GetFriendDamages()
			);

			this.SecondFleet.CalcDamages(
				data.api_kouku.GetSecondFleetDamages(),
				data.api_opening_taisen.GetFriendDamages(),
				data.api_opening_atack.GetFriendDamages(),
				data.api_hougeki1.GetFriendDamages(),
				data.api_raigeki.GetFriendDamages()
			);

			this.Enemies.CalcDamages(
				data.api_air_base_attack.GetEnemyDamages(),
				data.api_support_info.GetEnemyDamages(),
				data.api_kouku.GetEnemyDamages(),
				data.api_opening_taisen.GetEnemyDamages(),
				data.api_opening_atack.GetEnemyDamages(),
				data.api_hougeki1.GetEnemyDamages(),
				data.api_raigeki.GetEnemyDamages(),
				data.api_hougeki2.GetEnemyDamages(),
				data.api_hougeki3.GetEnemyDamages()
			);

			this.FriendAirSupremacy = data.api_kouku.GetAirSupremacy();

			this.AirCombatResults = data.api_air_base_attack.ToResult().Concat(data.api_kouku.ToResult()).ToArray();

			this.RankResult = this.CalcRank(true);
		}

		public void Update(combined_battle_battle_water data)
		{
			AutoSelectTab();
			this.Name = "연합함대 - 수상부대 - 주간전";

			this.UpdateFleets(data.api_deck_id, data, data.api_formation);
			this.UpdateMaxHP(data.api_maxhps, data.api_maxhps_combined);
			this.UpdateNowHP(data.api_nowhps, data.api_nowhps_combined);

			this.UpdateUsedFlag(data.api_kouku?.api_stage2?.api_air_fire);
			this.UpdateUsedFlag(data.api_support_info);

			this.FirstFleet.CalcDamages(
				data.api_kouku.GetFirstFleetDamages(),
				data.api_hougeki1.GetFriendDamages(),
				data.api_hougeki2.GetFriendDamages()
				);

			this.SecondFleet.CalcDamages(
				data.api_kouku.GetSecondFleetDamages(),
				data.api_opening_taisen.GetFriendDamages(),
				data.api_opening_atack.GetFriendDamages(),
				data.api_hougeki3.GetFriendDamages(),
				data.api_raigeki.GetFriendDamages()
				);

			this.Enemies.CalcDamages(
				data.api_air_base_attack.GetEnemyDamages(),
				data.api_support_info.GetEnemyDamages(),
				data.api_kouku.GetEnemyDamages(),
				data.api_opening_taisen.GetEnemyDamages(),
				data.api_opening_atack.GetEnemyDamages(),
				data.api_hougeki1.GetEnemyDamages(),
				data.api_hougeki2.GetEnemyDamages(),
				data.api_hougeki3.GetEnemyDamages(),
				data.api_raigeki.GetEnemyDamages()
				);

			this.FriendAirSupremacy = data.api_kouku.GetAirSupremacy();

			this.AirCombatResults = data.api_air_base_attack.ToResult().Concat(data.api_kouku.ToResult()).ToArray();

			this.RankResult = this.CalcRank(true);
		}

		public void Update(combined_battle_midnight_battle data)
		{
			AutoSelectTab();
			this.Name = "연합함대 - 야전";

			int BeforedayBattleHP = this.FirstFleet.Ships
				.Where(x => !x.Situation.HasFlag(ShipSituation.Tow) && !x.Situation.HasFlag(ShipSituation.Evacuation))
				.Sum(x => x.BeforeNowHP);//리스트 갱신하기전에 아군 HP최대값을 저장
			int EnemyBeforedayBattle = this.Enemies.Ships.Sum(x => x.BeforeNowHP);
			BeforedayBattleHP += this.SecondFleet.Ships
				.Where(x => !x.Situation.HasFlag(ShipSituation.Tow) && !x.Situation.HasFlag(ShipSituation.Evacuation))
				.Sum(x => x.BeforeNowHP);//리스트 갱신하기전에 아군 HP최대값을 저장

			this.UpdateFleets(data.api_deck_id, data);
			this.UpdateMaxHP(data.api_maxhps, data.api_maxhps_combined);
			this.UpdateNowHP(data.api_nowhps, data.api_nowhps_combined);

			this.UpdateUsedFlag(data.api_flare_pos, data.api_touch_plane);

			this.SecondFleet.CalcDamages(data.api_hougeki.GetFriendDamages());

			this.Enemies.CalcDamages(data.api_hougeki.GetEnemyDamages());

			this.RankResult = this.CalcRank(true, true, BeforedayBattleHP, EnemyBeforedayBattle);
		}

		public void Update(combined_battle_sp_midnight data)
		{
			AutoSelectTab();
			this.Name = "연합함대 - 개막야전";

			this.UpdateFleets(data.api_deck_id, data, data.api_formation);
			this.UpdateMaxHP(data.api_maxhps, data.api_maxhps_combined);
			this.UpdateNowHP(data.api_nowhps, data.api_nowhps_combined);

			this.UpdateUsedFlag(data.api_flare_pos, data.api_touch_plane);

			this.SecondFleet.CalcDamages(data.api_hougeki.GetFriendDamages());

			this.Enemies.CalcDamages(data.api_hougeki.GetEnemyDamages());

			this.FriendAirSupremacy = AirSupremacy.항공전없음;

			this.RankResult = this.CalcRank(true);
		}

		public void Update(practice_battle data)
		{
			AutoSelectTab();
			this.Clear();

			this.Name = "연습 - 주간전";
			this.CellData = new MapCellData
			{
				CellType = CellType.연습전,
				CellName = ""
			};

			this.UpdateFleets(data.api_dock_id, data, data.api_formation);
			this.UpdateMaxHP(data.api_maxhps);
			this.UpdateNowHP(data.api_nowhps);

			this.UpdateUsedFlag(data.api_kouku?.api_stage2?.api_air_fire);

			this.FirstFleet.CalcPracticeDamages(
				data.api_kouku.GetFirstFleetDamages(),
				data.api_opening_taisen.GetFriendDamages(),
				data.api_opening_atack.GetFriendDamages(),
				data.api_hougeki1.GetFriendDamages(),
				data.api_hougeki2.GetFriendDamages(),
				data.api_raigeki.GetFriendDamages()
				);

			this.Enemies.CalcPracticeDamages(
				data.api_kouku.GetEnemyDamages(),
				data.api_opening_taisen.GetEnemyDamages(),
				data.api_opening_atack.GetEnemyDamages(),
				data.api_hougeki1.GetEnemyDamages(),
				data.api_hougeki2.GetEnemyDamages(),
				data.api_raigeki.GetEnemyDamages()
				);

			this.FriendAirSupremacy = data.api_kouku.GetAirSupremacy();

			this.AirCombatResults = data.api_kouku.ToResult();

			this.RankResult = this.CalcRank();
		}

		public void Update(practice_midnight_battle data)
		{
			AutoSelectTab();
			this.Name = "연습 - 야전";

			int BeforedayBattleHP = this.FirstFleet.Ships
				.Where(x => !x.Situation.HasFlag(ShipSituation.Tow) && !x.Situation.HasFlag(ShipSituation.Evacuation))
				.Sum(x => x.BeforeNowHP);//리스트 갱신하기전에 아군 HP최대값을 저장
			int EnemyBeforedayBattle = this.Enemies.Ships.Sum(x => x.BeforeNowHP);

			this.UpdateFleets(data.api_deck_id, data);
			this.UpdateMaxHP(data.api_maxhps);
			this.UpdateNowHP(data.api_nowhps);

			this.UpdateUsedFlag(data.api_flare_pos, data.api_touch_plane);

			this.FirstFleet.CalcPracticeDamages(data.api_hougeki.GetFriendDamages());

			this.Enemies.CalcPracticeDamages(data.api_hougeki.GetEnemyDamages());

			this.RankResult = this.CalcRank(false, true, BeforedayBattleHP, EnemyBeforedayBattle);
		}

		private void Update(sortie_airbattle data)
		{
			AutoSelectTab();
			this.Name = "항공전 - 주간전";

			this.UpdateFleets(data.api_dock_id, data, data.api_formation);
			this.UpdateMaxHP(data.api_maxhps);
			this.UpdateNowHP(data.api_nowhps);

			this.UpdateUsedFlag(data.api_kouku?.api_stage2?.api_air_fire, data.api_kouku2?.api_stage2?.api_air_fire);
			this.UpdateUsedFlag(data.api_support_info);

			this.FirstFleet.CalcDamages(
				data.api_kouku.GetFirstFleetDamages(),
				data.api_kouku2.GetFirstFleetDamages()
				);

			this.Enemies.CalcDamages(
				data.api_air_base_attack.GetEnemyDamages(),
				data.api_support_info.GetEnemyDamages(),    //将来的に増える可能性を想定して追加しておく
				data.api_kouku.GetEnemyDamages(),
				data.api_kouku2.GetEnemyDamages()
				);

			this.FriendAirSupremacy = data.api_kouku.GetAirSupremacy(); // 航空戦2回目はスルー

			this.AirCombatResults = data.api_air_base_attack.ToResult().Concat(data.api_kouku.ToResult("1회차/"))
							.Concat(data.api_kouku2.ToResult("2회차/")).ToArray();

			this.RankResult = this.CalcRank();
		}

		private void Update(sortie_battle data)
		{
			AutoSelectTab();
			this.Name = "통상 - 주간전";

			this.UpdateFleets(data.api_dock_id, data, data.api_formation);
			this.UpdateMaxHP(data.api_maxhps);
			this.UpdateNowHP(data.api_nowhps);

			this.UpdateUsedFlag(data.api_kouku?.api_stage2?.api_air_fire);
			this.UpdateUsedFlag(data.api_support_info);

			this.FirstFleet.CalcDamages(
				data.api_kouku.GetFirstFleetDamages(),
				data.api_opening_taisen.GetFriendDamages(),
				data.api_opening_atack.GetFriendDamages(),
				data.api_hougeki1.GetFriendDamages(),
				data.api_hougeki2.GetFriendDamages(),
				data.api_raigeki.GetFriendDamages()
				);

			this.Enemies.CalcDamages(
				data.api_air_base_attack.GetEnemyDamages(),
				data.api_support_info.GetEnemyDamages(),
				data.api_kouku.GetEnemyDamages(),
				data.api_opening_taisen.GetEnemyDamages(),
				data.api_opening_atack.GetEnemyDamages(),
				data.api_hougeki1.GetEnemyDamages(),
				data.api_hougeki2.GetEnemyDamages(),
				data.api_raigeki.GetEnemyDamages()
				);

			this.FriendAirSupremacy = data.api_kouku.GetAirSupremacy();

			this.AirCombatResults = data.api_air_base_attack.ToResult().Concat(data.api_kouku.ToResult()).ToArray();

			this.RankResult = this.CalcRank();
		}

		private void Update(sortie_ld_airbattle data)
		{
			AutoSelectTab();
			this.Name = "공습전 - 주간";

			this.UpdateFleets(data.api_dock_id, data, data.api_formation);
			this.UpdateMaxHP(data.api_maxhps);
			this.UpdateNowHP(data.api_nowhps);

			this.UpdateUsedFlag(data.api_kouku?.api_stage2?.api_air_fire);

			this.FirstFleet.CalcDamages(
				data.api_kouku.GetFirstFleetDamages()
			);
			this.Enemies.CalcDamages(
				data.api_air_base_attack.GetEnemyDamages()
			);

			this.FriendAirSupremacy = data.api_kouku.GetAirSupremacy();

			this.AirCombatResults = data.api_air_base_attack.ToResult().Concat(data.api_kouku.ToResult()).ToArray();

			this.AirRankResult = this.CalcRank3();
			this.RankResult = BattleRank.공습전;
		}

		private void Update(combined_battle_ld_airbattle data)
		{
			AutoSelectTab();
			this.Name = "연합함대 - 공습전 - 주간";

			this.UpdateFleets(data.api_deck_id, data, data.api_formation);
			this.UpdateMaxHP(data.api_maxhps, data.api_maxhps_combined);
			this.UpdateNowHP(data.api_nowhps, data.api_nowhps_combined);

			this.UpdateUsedFlag(data.api_kouku?.api_stage2?.api_air_fire);

			this.FirstFleet.CalcDamages(
				data.api_kouku.GetFirstFleetDamages()
			);
			this.SecondFleet.CalcDamages(
				data.api_kouku.GetSecondFleetDamages()
			);

			this.Enemies.CalcDamages(
				data.api_air_base_attack.GetEnemyDamages()
			);

			this.FriendAirSupremacy = data.api_kouku.GetAirSupremacy();

			this.AirCombatResults = data.api_air_base_attack.ToResult().Concat(data.api_kouku.ToResult()).ToArray();

			this.AirRankResult = this.CalcRank3(true);
			this.RankResult = BattleRank.공습전;
		}

		public void Update(combined_battle_each_battle data, bool isCombined)
		{
			AutoSelectTab();
			this.Name = "연합함대 - 주간전";

			this.UpdateFleets2(data.api_deck_id, data, data.api_formation);
			this.UpdateMaxHP(data.api_maxhps, data.api_maxhps_combined);
			this.UpdateNowHP(data.api_nowhps, data.api_nowhps_combined);

			this.UpdateUsedFlag(data.api_kouku?.api_stage2?.api_air_fire);
			this.UpdateUsedFlag(data.api_support_info);

			if (isCombined)
			{
				this.FirstFleet.CalcDamages(
					data.api_kouku.GetFirstFleetDamages(),
					data.api_opening_taisen.GetEachFirstFriendDamages(),
					data.api_opening_atack.GetEachFirstFriendDamages(),
					data.api_hougeki1.GetEachFirstFriendDamages(),
					data.api_raigeki.GetEachFirstFriendDamages(),
					data.api_hougeki3.GetEachFirstFriendDamages()
				);

				this.SecondFleet.CalcDamages(
					data.api_kouku.GetSecondFleetDamages(),
					data.api_opening_taisen.GetEachSecondFriendDamages(),
					data.api_opening_atack.GetEachSecondFriendDamages(),
					data.api_hougeki2.GetEachSecondFriendDamages(),
					data.api_raigeki.GetEachSecondFriendDamages(),
					data.api_hougeki3.GetEachSecondFriendDamages()
				);

				this.Enemies.CalcDamages(
					data.api_air_base_attack.GetEachFirstEnemyDamages(),
					data.api_support_info.GetEachFirstEnemyDamages(),
					data.api_kouku.GetEnemyDamages(),
					data.api_opening_taisen.GetEachFirstEnemyDamages(),
					data.api_opening_atack.GetEachFirstEnemyDamages(),
					data.api_hougeki1.GetEachFirstEnemyDamages(),
					data.api_raigeki.GetEachFirstEnemyDamages(),
					data.api_hougeki3.GetEachFirstEnemyDamages()
				);

				this.SecondEnemies.CalcDamages(
					data.api_air_base_attack.GetEachSecondEnemyDamages(),
					data.api_support_info.GetEachSecondEnemyDamages(),
					data.api_kouku.GetSecondEnemyDamages(),
					data.api_opening_taisen.GetEachSecondEnemyDamages(),
					data.api_opening_atack.GetEachSecondEnemyDamages(),
					data.api_hougeki2.GetEachSecondEnemyDamages(),
					data.api_raigeki.GetEachSecondEnemyDamages(),
					data.api_hougeki3.GetEachSecondEnemyDamages()
				);
			}
			else
			{
				this.FirstFleet.CalcDamages(
					data.api_kouku.GetFirstFleetDamages(),
					data.api_opening_taisen.GetEachFirstFriendDamages(),
					data.api_opening_atack.GetEachFirstFriendDamages(),
					data.api_hougeki1.GetEachFirstFriendDamages(),
					data.api_hougeki2.GetEachFirstFriendDamages(),
					data.api_raigeki.GetEachFirstFriendDamages(),
					data.api_hougeki3.GetEachFirstFriendDamages()
				);

				this.Enemies.CalcDamages(
					data.api_air_base_attack.GetEachFirstEnemyDamages(),
					data.api_support_info.GetEachFirstEnemyDamages(),
					data.api_kouku.GetEnemyDamages(),
					data.api_opening_taisen.GetEachFirstEnemyDamages(),
					data.api_opening_atack.GetEachFirstEnemyDamages(),
					data.api_hougeki1.GetEachFirstEnemyDamages(),
					data.api_raigeki.GetEachFirstEnemyDamages(),
					data.api_hougeki2.GetEachFirstEnemyDamages(),
					data.api_hougeki3.GetEachFirstEnemyDamages()
				);

				this.SecondEnemies.CalcDamages(
					data.api_air_base_attack.GetEachSecondEnemyDamages(),
					data.api_support_info.GetEachSecondEnemyDamages(),
					data.api_kouku.GetSecondEnemyDamages(),
					data.api_opening_taisen.GetEachSecondEnemyDamages(),
					data.api_opening_atack.GetEachSecondEnemyDamages(),
					data.api_hougeki1.GetEachSecondEnemyDamages(),
					data.api_raigeki.GetEachSecondEnemyDamages(),
					data.api_hougeki2.GetEachSecondEnemyDamages(),
					data.api_hougeki3.GetEachSecondEnemyDamages()
				);
			}

			this.FriendAirSupremacy = data.api_kouku.GetAirSupremacy();

			this.AirCombatResults = data.api_air_base_attack.ToResult().Concat(data.api_kouku.ToResult()).ToArray();

			this.RankResult = this.CalcRank2(true, true);
		}

		public void Update(combined_battle_ec_midnight_battle data)
		{
			AutoSelectTab();
			this.Name = "연합함대 - 야전";

			int BeforedayBattleHP = this.FirstFleet.Ships
				.Where(x => !x.Situation.HasFlag(ShipSituation.Tow) && !x.Situation.HasFlag(ShipSituation.Evacuation))
				.Sum(x => x.BeforeNowHP); // 리스트 갱신하기전에 아군 HP최대값을 저장
			BeforedayBattleHP += this.SecondFleet.Ships
				.Where(x => !x.Situation.HasFlag(ShipSituation.Tow) && !x.Situation.HasFlag(ShipSituation.Evacuation))
				.Sum(x => x.BeforeNowHP); // 리스트 갱신하기전에 아군 HP최대값을 저장

			int EnemyBeforedayBattle = this.Enemies.Ships.Sum(x => x.BeforeNowHP);
			EnemyBeforedayBattle += this.SecondEnemies.Ships.Sum(x => x.BeforeNowHP);

			this.UpdateFleets2(data.api_deck_id, data);
			this.UpdateEnemyMaxHP(data.api_maxhps, data.api_maxhps_combined);
			this.UpdateNowHP(data.api_nowhps, data.api_nowhps_combined);

			this.UpdateUsedFlag(data.api_flare_pos, data.api_touch_plane);

			if (data.api_active_deck[0] == 1) this.FirstFleet.CalcDamages(data.api_hougeki.GetFriendDamages());
			else this.SecondFleet.CalcDamages(data.api_hougeki.GetFriendDamages());

			if (data.api_active_deck[1] == 1) this.Enemies.CalcDamages(data.api_hougeki.GetEnemyDamages());
			else this.SecondEnemies.CalcDamages(data.api_hougeki.GetEnemyDamages());

			this.RankResult = this.CalcRank2(true, true, true, BeforedayBattleHP, EnemyBeforedayBattle);
		}

		#endregion

		public void Update(battle_result data)
		{
			this.DropShipName = DataStorage.Instance.Master.Ships.SingleOrDefault(x => x.Value.Id == data.api_get_ship?.api_ship_id).Value?.Name;

			foreach (var item in FirstFleet.Ships) item.IsMvp = false;
			foreach (var item in SecondFleet.Ships) item.IsMvp = false;

			bool[] firstMvp = new bool[6] { false, false, false, false, false, false };
			bool[] secondMvp = new bool[6] { false, false, false, false, false, false };

			if (data.api_mvp > 0 && FirstFleet != null)
			{
				firstMvp[data.api_mvp - 1] = true;
				FirstFleet.Ships.SetValues(firstMvp, (s, v) => s.IsMvp = v);
			}
			if (data.api_mvp_combined > 0 && SecondFleet != null)
			{
				secondMvp[(data.api_mvp_combined > 6 ? data.api_mvp_combined - 6 : data.api_mvp_combined) - 1] = true;
				SecondFleet.Ships.SetValues(secondMvp, (s, v) => s.IsMvp = v);
			}
		}

		private void UpdateFleetsByStartNext(map_start_next startNext, string api_deck_id = null)
		{
			this.Clear();

			string Cell = "";
			if (startNext.api_no != 0)
				Cell = startNext.api_maparea_id + "-" + startNext.api_mapinfo_no + "-" + startNext.api_no;

			this.CellData = new MapCellData
			{
				CellType = startNext.api_event_id.ToCellType(),
				CellName = Cell,
				IsFirst = api_deck_id != null
			};

			this.RankResult = BattleRank.없음;
			this.AirRankResult = BattleRank.없음;

			if (api_deck_id != null) this.CurrentDeckId = int.Parse(api_deck_id);
			if (this.CurrentDeckId < 1) return;

			this.UpdateFriendFleets(this.CurrentDeckId);

			if (this.FirstFleet != null) this.FirstFleet.TotalDamaged = 0;
			if (this.SecondFleet != null) this.SecondFleet.TotalDamaged = 0;

			AutoSelectTab();
		}
		private void UpdateFleets(int api_deck_id, ICommonBattleMembers data, int[] api_formation = null)
		{
			this.UpdatedTime = DateTimeOffset.Now;
			this.UpdateFriendFleets(api_deck_id);

			var eTotal = 0;
			if (this.Enemies != null) eTotal = this.Enemies.TotalDamaged;
			this.Enemies = new FleetData(
				data.ToMastersShipDataArray(),
				this.Enemies?.Formation ?? Formation.없음,
				this.Enemies?.Name ?? "",
				FleetType.Enemy,
				this.Enemies?.Rank);
			this.Enemies.TotalDamaged = eTotal;

			this.SecondEnemies = new FleetData(
				new MembersShipData[0],
				Formation.없음,
				"",
				FleetType.SecondEnemy,
				this.SecondEnemies?.Rank
			);


			if (api_formation != null)
			{
				this.BattleSituation = (BattleSituation)api_formation[2];
				if (this.FirstFleet != null) this.FirstFleet.Formation = (Formation)api_formation[0];
				if (this.Enemies != null) this.Enemies.Formation = (Formation)api_formation[1];
			}

			this.CurrentDeckId = api_deck_id;
		}
		private void UpdateFleets2(int api_deck_id, ICommonEachBattleMembers data, int[] api_formation = null)
		{
			this.UpdatedTime = DateTimeOffset.Now;
			this.UpdateFriendFleets(api_deck_id);

			var eTotal = 0;
			if (this.Enemies != null) eTotal = this.Enemies.TotalDamaged;
			this.Enemies = new FleetData(
				data.ToMastersShipDataArray(),
				this.Enemies?.Formation ?? Formation.없음,
				this.Enemies?.Name ?? "",
				FleetType.Enemy,
				this.Enemies?.Rank);
			this.Enemies.TotalDamaged = eTotal;

			eTotal = 0;
			if (this.SecondEnemies != null) eTotal = this.SecondEnemies.TotalDamaged;
			this.SecondEnemies = new FleetData(
				data.ToMastersSecondShipDataArray(),
				this.SecondEnemies?.Formation ?? Formation.없음,
				this.SecondEnemies?.Name ?? "",
				FleetType.SecondEnemy,
				this.SecondEnemies?.Rank);
			this.SecondEnemies.TotalDamaged = eTotal;

			if (api_formation != null)
			{
				this.BattleSituation = (BattleSituation)api_formation[2];
				if (this.FirstFleet != null) this.FirstFleet.Formation = (Formation)api_formation[0];
				if (this.Enemies != null) this.Enemies.Formation = (Formation)api_formation[1];
			}

			this.CurrentDeckId = api_deck_id;
		}
		private void UpdateFriendFleets(int deckID)
		{
			var organization = DataStorage.Instance.Homeport.Organization;

			int firstTotal = 0;
			int secondTotal = 0;
			if (this.FirstFleet != null) firstTotal = this.FirstFleet.TotalDamaged;
			if (this.SecondFleet != null) secondTotal = this.SecondFleet.TotalDamaged;

			this.FirstFleet = new FleetData(
				organization.Fleets[deckID].Ships.Select(s => new MembersShipData(s)).ToArray(),
				this.FirstFleet?.Formation ?? Formation.없음,
				organization.Fleets[deckID].Name,
				FleetType.First);
			this.FirstFleet.TotalDamaged = firstTotal;

			this.SecondFleet = new FleetData(
				organization.Combined && deckID == 1
					? organization.Fleets[2].Ships.Select(s => new MembersShipData(s)).ToArray()
					: new MembersShipData[0],
				this.SecondFleet?.Formation ?? Formation.없음,
				organization.Fleets[2].Name,
				FleetType.Second);
			this.SecondFleet.TotalDamaged = secondTotal;
		}

		private void UpdateMaxHP(int[] api_maxhps, int[] api_maxhps_combined = null)
		{
			this.FirstFleet.Ships.SetValues(api_maxhps.GetFriendData(), (s, v) => s.MaxHP = v);
			this.Enemies.Ships.SetValues(api_maxhps.GetEnemyData(), (s, v) => s.MaxHP = v);

			if (api_maxhps_combined == null) return;
			this.SecondFleet.Ships.SetValues(api_maxhps_combined.GetFriendData(), (s, v) => s.MaxHP = v);
			this.SecondEnemies.Ships.SetValues(api_maxhps_combined.GetEnemyData(), (s, v) => s.MaxHP = v);
		}
		private void UpdateNowHP(int[] api_nowhps, int[] api_nowhps_combined = null)
		{
			this.FirstFleet.Ships.SetValues(api_nowhps.GetFriendData(), (s, v) => s.NowHP = v);
			this.FirstFleet.Ships.SetValues(api_nowhps.GetFriendData(), (s, v) => s.BeforeNowHP = v);

			this.Enemies.Ships.SetValues(api_nowhps.GetEnemyData(), (s, v) => s.NowHP = v);
			this.Enemies.Ships.SetValues(api_nowhps.GetEnemyData(), (s, v) => s.BeforeNowHP = v);

			if (api_nowhps_combined == null) return;
			this.SecondFleet.Ships.SetValues(api_nowhps_combined.GetFriendData(), (s, v) => s.NowHP = v);
			this.SecondFleet.Ships.SetValues(api_nowhps_combined.GetFriendData(), (s, v) => s.BeforeNowHP = v);

			this.SecondEnemies.Ships.SetValues(api_nowhps_combined.GetEnemyData(), (s, v) => s.NowHP = v);
			this.SecondEnemies.Ships.SetValues(api_nowhps_combined.GetEnemyData(), (s, v) => s.BeforeNowHP = v);
		}

		private void UpdateEnemyMaxHP(int[] api_maxhps, int[] api_maxhps_combined = null)
		{
			this.Enemies.Ships.SetValues(api_maxhps.GetEnemyData(), (s, v) => s.MaxHP = v);

			if (api_maxhps_combined == null) return;
			this.SecondEnemies.Ships.SetValues(api_maxhps_combined.GetEnemyData(), (s, v) => s.MaxHP = v);
		}

		private void ResultClear()
		{
			if (this.FirstFleet != null) this.FirstFleet.TotalDamaged = 0;
			if (this.SecondFleet != null) this.SecondFleet.TotalDamaged = 0;
		}
		private void Clear()
		{
			this.UpdatedTime = DateTimeOffset.Now;
			this.Name = "";
			this.DropShipName = null;

			if (this.FirstFleet != null) FirstFleet.Ships.SetValues(new bool[6] { false, false, false, false, false, false }, (s, v) => s.IsMvp = v);
			if (this.SecondFleet != null) SecondFleet.Ships.SetValues(new bool[6] { false, false, false, false, false, false }, (s, v) => s.IsMvp = v);

			this.FlareUsed = UsedFlag.Unset;
			this.NightReconScouted = UsedFlag.Unset;
			this.AntiAirFired = AirFireFlag.Unset;
			this.SupportUsed = UsedSupport.Unset;

			this.BattleSituation = BattleSituation.없음;
			this.FriendAirSupremacy = AirSupremacy.항공전없음;
			this.AirCombatResults = new AirCombatResult[0];
			if (this.FirstFleet != null) this.FirstFleet.Formation = Formation.없음;
			this.Enemies = new FleetData();
			this.SecondEnemies = new FleetData();
		}

		private bool CalcOverKill(int MaxCount, int SinkCount)
		{
			if (MaxCount == 1)
			{
				if (MaxCount == SinkCount) return true;
				else return false;
			}
			else if (MaxCount == 2)
			{
				if (SinkCount >= 1) return true;
				else return false;
			}

			// x / 3m * 2m
			return (int)decimal.Floor(MaxCount / 1.5m) <= SinkCount;
		}
		private BattleRank CalcRank(bool IsCombined = false, bool IsMidnight = false, int BeforeHP = 0, int EnemyBefore = 0)
		{
			try
			{
				var AliasMax = this.FirstFleet.Ships
					.Where(x => !x.Situation.HasFlag(ShipSituation.Tow) && !x.Situation.HasFlag(ShipSituation.Evacuation))
					.Sum(x => x.BeforeNowHP);

				var EnemyTotal = this.Enemies.TotalDamaged;
				var EnemyMax = this.Enemies.Ships.Sum(x => x.BeforeNowHP);

				var AliasTotal = this.FirstFleet.TotalDamaged;
				var IsShipSink = this.FirstFleet.SinkCount > 0; // ? true : false;
				ShipData EnemyFlag = this.Enemies.Ships.First();

				decimal EnemyDamagedPercent = EnemyTotal / (decimal)EnemyMax; // 적이 받은 총 데미지
				decimal AliasDamagedPercent = AliasTotal / (decimal)AliasMax; // 아군이 받은 총 데미지

				bool IsThreeTime = false;
				bool IsOverDamage = false;
				bool IsMidDamage = false;
				bool IsScratch = false;

				int MaxCount = this.FirstFleet.Ships
					.Where(x => !x.Situation.HasFlag(ShipSituation.Tow) && !x.Situation.HasFlag(ShipSituation.Evacuation))
					.Count();
				int EnemyMaxCount = this.Enemies.Ships.Count();
				int SinkCount = this.FirstFleet.SinkCount;

				if (IsCombined)
				{
					AliasTotal += this.SecondFleet.TotalDamaged;
					AliasMax += this.SecondFleet.Ships.Sum(x => x.BeforeNowHP);
					AliasDamagedPercent = AliasTotal / (decimal)AliasMax; // 아군이 받은 총 데미지

					if (!IsShipSink) IsShipSink = this.SecondFleet.SinkCount > 0 ? true : false;
					MaxCount += this.SecondFleet.Ships
					.Where(x => !x.Situation.HasFlag(ShipSituation.Tow) && !x.Situation.HasFlag(ShipSituation.Evacuation))
					.Count();
					SinkCount += this.SecondFleet.SinkCount;
				}

				if (IsMidnight)
				{
					AliasMax = BeforeHP;
					EnemyMax = EnemyBefore;

					EnemyDamagedPercent = EnemyTotal / (decimal)EnemyMax; // 적이 받은 총 데미지
					AliasDamagedPercent = AliasTotal / (decimal)AliasMax; // 아군이 받은 총 데미지
				}

				this.FirstFleet.AttackGauge = this.MakeGaugeText(EnemyTotal, EnemyMax, EnemyDamagedPercent);
				this.Enemies.AttackGauge = this.MakeGaugeText(AliasTotal, AliasMax, AliasDamagedPercent);

				bool IsOverKill = CalcOverKill(EnemyMaxCount, this.Enemies.SinkCount);
				bool IsOverKilled = CalcOverKill(MaxCount, SinkCount);

				if (AliasTotal > 0)
				{
					decimal EnemyValue = decimal.Floor(EnemyDamagedPercent * 100m); // 적군 피격 데미지 비율 (소숫점 제외)
					decimal AliasValue = decimal.Floor(AliasDamagedPercent * 100m); // 아군 피격 데미지 비율 (소숫점 제외)

					if (AliasValue == 0)
					{
						if (EnemyTotal == 0) IsScratch = true;
						else IsOverDamage = true; // 아군피해 0인 경우
					}
					else
					{
						var CalcPercent = Math.Round(EnemyValue / AliasValue, 2, MidpointRounding.AwayFromZero);
						if (CalcPercent > 2.5m)
							IsOverDamage = true;// 2.5배 초과 데미지
						else if (CalcPercent > 1m)
							IsMidDamage = true;// 1초과 2.5이하
						else
							IsScratch = true;// 1미만

						if (IsShipSink)
						{
							if (CalcPercent > 3m)
							{
								IsThreeTime = true;
								IsOverDamage = true;
								IsMidDamage = false;
								IsScratch = false;
							}
						}
					}
				}
				else if (AliasTotal == 0)
				{
					if (EnemyTotal == 0) IsScratch = true;
					else IsOverDamage = true; // 아군피해 0인 경우
				}

				if (AliasTotal == 0 && EnemyTotal == 0) return BattleRank.D패배;//d
				if (EnemyDamagedPercent < 0.0005m) return BattleRank.D패배;//d
				else if (IsShipSink)
				{
					if (EnemyFlag.NowHP <= 0)
					{
						if (IsOverKill) return BattleRank.B승리;
						else return BattleRank.D패배;//d
					}
					else if (IsMidDamage) return BattleRank.C패배;//c
					else
					{
						if (IsOverKilled) return BattleRank.E패배;//e
						else
						{
							if (!IsOverKill && IsThreeTime) return BattleRank.B승리;
							if (IsOverKill && IsOverDamage) return BattleRank.B승리;
							else return BattleRank.C패배;//c
						}
					}
				}
				else
				{
					if (EnemyFlag.NowHP <= 0)
					{
						if (EnemyMaxCount == this.Enemies.SinkCount)
						{
							if (AliasTotal > 0) return BattleRank.S승리;
							else return BattleRank.완전승리S;
						}
						else
						{
							if (IsOverKill) return BattleRank.A승리;
							else return BattleRank.B승리;
						}
					}
					else
					{
						if (IsOverKill) return BattleRank.A승리;

						if (IsOverDamage) return BattleRank.B승리;
						else if (IsMidDamage) return BattleRank.C패배;//c
						else if (IsScratch) return BattleRank.D패배;//d
						else return BattleRank.D패배;//d
					}
				}
			}
			catch (Exception ex)
			{
				// DataStorage.Instance.CatchedErrorLogWriter.ReportException(ex.Source, ex);
				System.IO.File.AppendAllText("battleinfo_error.log", ex.ToString() + Environment.NewLine);
				Debug.WriteLine(ex);
				return BattleRank.에러;
			}
		}
		private BattleRank CalcRank2(bool IsCombined = false, bool IsEnemyCombined = false, bool IsMidnight = false, int BeforeHP = 0, int EnemyBefore = 0)
		{
			try
			{
				var AliasMax = this.FirstFleet.Ships
					.Where(x => !x.Situation.HasFlag(ShipSituation.Tow) && !x.Situation.HasFlag(ShipSituation.Evacuation))
					.Sum(x => x.BeforeNowHP);

				var EnemyTotal = this.Enemies.TotalDamaged;
				var EnemyMax = this.Enemies.Ships.Sum(x => x.BeforeNowHP);

				var AliasTotal = this.FirstFleet.TotalDamaged;
				var IsShipSink = this.FirstFleet.SinkCount > 0; // ? true : false;
				ShipData EnemyFlag = this.Enemies.Ships.First();

				decimal EnemyDamagedPercent = EnemyTotal / (decimal)EnemyMax; // 적이 받은 총 데미지
				decimal AliasDamagedPercent = AliasTotal / (decimal)AliasMax; // 아군이 받은 총 데미지

				bool IsThreeTime = false;
				bool IsOverDamage = false;
				bool IsMidDamage = false;
				bool IsScratch = false;

				int MaxCount = this.FirstFleet.Ships
					.Where(x => !x.Situation.HasFlag(ShipSituation.Tow) && !x.Situation.HasFlag(ShipSituation.Evacuation))
					.Count();
				int EnemyMaxCount = this.Enemies.Ships.Count();
				int SinkCount = this.FirstFleet.SinkCount;

				if (IsCombined)
				{
					AliasTotal += this.SecondFleet.TotalDamaged;
					AliasMax += this.SecondFleet.Ships.Sum(x => x.BeforeNowHP);
					AliasDamagedPercent = AliasTotal / (decimal)AliasMax; // 아군이 받은 총 데미지

					if (!IsShipSink) IsShipSink = this.SecondFleet.SinkCount > 0 ? true : false;
					MaxCount += this.SecondFleet.Ships
					.Where(x => !x.Situation.HasFlag(ShipSituation.Tow) && !x.Situation.HasFlag(ShipSituation.Evacuation))
					.Count();
					SinkCount += this.SecondFleet.SinkCount;
				}
				if (IsEnemyCombined)
				{
					EnemyMaxCount += this.SecondEnemies.Ships.Count();
					EnemyTotal += this.SecondEnemies.TotalDamaged;
					EnemyMax += this.SecondEnemies.Ships.Sum(x => x.BeforeNowHP);
					EnemyDamagedPercent = EnemyTotal / (decimal)EnemyMax; // 적군이 받은 총 데미지
				}

				if (IsMidnight)
				{
					AliasMax = BeforeHP;
					EnemyMax = EnemyBefore;

					EnemyDamagedPercent = EnemyTotal / (decimal)EnemyMax; // 적이 받은 총 데미지
					AliasDamagedPercent = AliasTotal / (decimal)AliasMax; // 아군이 받은 총 데미지
				}

				this.FirstFleet.AttackGauge = this.MakeGaugeText(EnemyTotal, EnemyMax, EnemyDamagedPercent);
				this.Enemies.AttackGauge = this.MakeGaugeText(AliasTotal, AliasMax, AliasDamagedPercent);

				bool IsOverKill = CalcOverKill(EnemyMaxCount, this.Enemies.SinkCount + (IsEnemyCombined ? this.SecondEnemies.SinkCount : 0));
				bool IsOverKilled = CalcOverKill(MaxCount, SinkCount);

				if (AliasTotal > 0)
				{
					decimal EnemyValue = decimal.Floor(EnemyDamagedPercent * 100m); // 적군 피격 데미지 비율 (소숫점 제외)
					decimal AliasValue = decimal.Floor(AliasDamagedPercent * 100m); // 아군 피격 데미지 비율 (소숫점 제외)

					if (AliasValue == 0)
					{
						if (EnemyTotal == 0) IsScratch = true;
						else IsOverDamage = true; // 아군피해 0인 경우
					}
					else
					{
						var CalcPercent = Math.Round(EnemyValue / AliasValue, 2, MidpointRounding.AwayFromZero);
						if (CalcPercent > 2.5m)
							IsOverDamage = true;// 2.5배 초과 데미지
						else if (CalcPercent > 1m)
							IsMidDamage = true;// 1초과 2.5이하
						else
							IsScratch = true;// 1미만

						if (IsShipSink)
						{
							if (CalcPercent > 3m)
							{
								IsThreeTime = true;
								IsOverDamage = true;
								IsMidDamage = false;
								IsScratch = false;
							}
						}
					}
				}
				else if (AliasTotal == 0)
				{
					if (EnemyTotal == 0) IsScratch = true;
					else IsOverDamage = true; // 아군피해 0인 경우
				}

				if (AliasTotal == 0 && EnemyTotal == 0) return BattleRank.D패배;//d
				if (EnemyDamagedPercent < 0.0005m) return BattleRank.D패배;//d
				else if (IsShipSink)
				{
					if (EnemyFlag.NowHP <= 0)
					{
						if (IsOverKill) return BattleRank.B승리;
						else return BattleRank.D패배;//d
					}
					else if (IsMidDamage) return BattleRank.C패배;//c
					else
					{
						if (IsOverKilled) return BattleRank.E패배;//e
						else
						{
							if (!IsOverKill && IsThreeTime) return BattleRank.B승리;
							if (IsOverKill && IsOverDamage) return BattleRank.B승리;
							else return BattleRank.C패배;//c
						}
					}
				}
				else
				{
					if (EnemyFlag.NowHP <= 0)
					{
						if (EnemyMaxCount == this.Enemies.SinkCount)
						{
							if (AliasTotal > 0) return BattleRank.S승리;
							else return BattleRank.완전승리S;
						}
						else
						{
							if (IsOverKill) return BattleRank.A승리;
							else return BattleRank.B승리;
						}
					}
					else
					{
						if (IsOverKill) return BattleRank.A승리;

						if (IsOverDamage) return BattleRank.B승리;
						else if (IsMidDamage) return BattleRank.C패배;//c
						else if (IsScratch) return BattleRank.D패배;//d
						else return BattleRank.D패배;//d
					}
				}
			}
			catch (Exception ex)
			{
				// DataStorage.Instance.CatchedErrorLogWriter.ReportException(ex.Source, ex);
				System.IO.File.AppendAllText("battleinfo_error.log", "-----------------------------------------------------------------" + Environment.NewLine);
				System.IO.File.AppendAllText("battleinfo_error.log", DateTime.Now.ToString() + Environment.NewLine);
				System.IO.File.AppendAllText("battleinfo_error.log", ex.ToString() + Environment.NewLine);
				Debug.WriteLine(ex);
				return BattleRank.에러;
			}
		}
		private BattleRank CalcRank3(bool IsCombined = false)
		{
			try
			{
				var AliasMax = this.FirstFleet.Ships
					.Where(x => !x.Situation.HasFlag(ShipSituation.Tow) && !x.Situation.HasFlag(ShipSituation.Evacuation))
					.Sum(x => x.BeforeNowHP);

				var EnemyTotal = this.Enemies.TotalDamaged;
				var EnemyMax = this.Enemies.Ships.Sum(x => x.BeforeNowHP);

				var AliasTotal = this.FirstFleet.TotalDamaged;
				var IsShipSink = this.FirstFleet.SinkCount > 0; // ? true : false;

				decimal EnemyDamagedPercent = EnemyTotal / (decimal)EnemyMax; // 적이 받은 총 데미지
				decimal AliasDamagedPercent = AliasTotal / (decimal)AliasMax; // 아군이 받은 총 데미지

				int MaxCount = this.FirstFleet.Ships
					.Where(x => !x.Situation.HasFlag(ShipSituation.Tow) && !x.Situation.HasFlag(ShipSituation.Evacuation))
					.Count();
				int SinkCount = this.FirstFleet.SinkCount;

				if (IsCombined)
				{
					AliasTotal += this.SecondFleet.TotalDamaged;
					AliasMax += this.SecondFleet.Ships.Sum(x => x.BeforeNowHP);
					AliasDamagedPercent = AliasTotal / (decimal)AliasMax; // 아군이 받은 총 데미지

					if (!IsShipSink) IsShipSink = this.SecondFleet.SinkCount > 0 ? true : false;
					MaxCount += this.SecondFleet.Ships
					.Where(x => !x.Situation.HasFlag(ShipSituation.Tow) && !x.Situation.HasFlag(ShipSituation.Evacuation))
					.Count();
					SinkCount += this.SecondFleet.SinkCount;
				}

				this.FirstFleet.AttackGauge = this.MakeGaugeText(EnemyTotal, EnemyMax, EnemyDamagedPercent);
				this.Enemies.AttackGauge = this.MakeGaugeText(AliasTotal, AliasMax, AliasDamagedPercent);

				bool IsOverKilled = CalcOverKill(MaxCount, SinkCount);

				if (AliasTotal > 0)
				{
					decimal AliasValue = decimal.Floor(AliasDamagedPercent * 100m); // 아군 피격 데미지 비율 (소숫점 제외)

					if (SinkCount > 0) return BattleRank.E패배;
					else if (AliasValue == 0) return BattleRank.A승리;
					else if (AliasValue > 0 && AliasValue < 10) return BattleRank.A승리;
					else if (AliasValue > 10 && AliasValue < 20) return BattleRank.B승리;
					else if (AliasValue > 20 && AliasValue < 50) return BattleRank.C패배;
					else return BattleRank.D패배;
				}
				else return BattleRank.완전승리S;
			}
			catch (Exception ex)
			{
				// DataStorage.Instance.CatchedErrorLogWriter.ReportException(ex.Source, ex);
				System.IO.File.AppendAllText("battleinfo_error.log", ex.ToString() + Environment.NewLine);
				Debug.WriteLine(ex);
				return BattleRank.에러;
			}
		}

		private void UpdateUsedFlag(int[] flare, int[] nightrecon)
		{
			try
			{
				if (flare[0] == -1 && flare[1] == -1)
					this.FlareUsed = UsedFlag.Unused;
				else if (flare[0] != -1)
					this.FlareUsed = UsedFlag.Used;
				else if (flare[1] != -1)
					this.FlareUsed = UsedFlag.EnemyUsed;
				else
					this.FlareUsed = UsedFlag.BothUsed;

				if (nightrecon[0] == -1 && nightrecon[1] == -1)
					this.NightReconScouted = UsedFlag.Unused;
				else if (nightrecon[0] != -1)
					this.NightReconScouted = UsedFlag.Used;
				else if (nightrecon[1] != -1)
					this.NightReconScouted = UsedFlag.EnemyUsed;
				else
					this.NightReconScouted = UsedFlag.BothUsed;
			}
			catch
			{
				this.FlareUsed = UsedFlag.Unset;
				this.NightReconScouted = UsedFlag.Unset;
			}
		}
		private void UpdateUsedFlag(Api_Air_Fire data)
		{
			try
			{
				this.AntiAirFired = data == null ? AirFireFlag.Unused : AirFireFlag.Used;
			}
			catch
			{
				this.AntiAirFired = AirFireFlag.Unset;
			}
		}
		private void UpdateUsedFlag(Api_Air_Fire data1, Api_Air_Fire data2)
		{
			try
			{
				if (data1 == null && data2 == null)
					this.AntiAirFired = AirFireFlag.Unused;
				else if (data1 != null)
					this.AntiAirFired = AirFireFlag.Used1;
				else if (data2 != null)
					this.AntiAirFired = AirFireFlag.Used2;
				else
					this.AntiAirFired = AirFireFlag.UsedAll;
			}
			catch
			{
				this.AntiAirFired = AirFireFlag.Unset;
			}
		}
		private void UpdateUsedFlag(Api_Support_Info data)
		{
			try
			{
				if (data == null)
					this.SupportUsed = UsedSupport.Unused;
				else if (data.api_support_airatack != null)
					this.SupportUsed = UsedSupport.Kouku;
				else if (data.api_support_hourai != null)
					this.SupportUsed = UsedSupport.Hourai;
				else
					this.SupportUsed = UsedSupport.Unset; ;
			}
			catch
			{
				this.SupportUsed = UsedSupport.Unset; ;
			}
		}

		private string MakeGaugeText(int current, int max, decimal percent)
		{
			StringBuilder temp = new StringBuilder();

			temp.Append("(" + current + "/" + max + ") ");
			temp.Append(Math.Round(percent * 100, 2, MidpointRounding.AwayFromZero) + "%");
			return temp.ToString();
		}
	}
}
