using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Core;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	/// <summary>
	/// 함대 데이터
	/// </summary>
	public class Organization : Notifier
	{
		private readonly Homeport homeport;

		private readonly List<int> evacuatedShipsIds = new List<int>();
		private readonly List<int> towShipIds = new List<int>();

		#region Ships 프로퍼티
		private MemberTable<Ship> _Ships;
		public MemberTable<Ship> Ships
		{
			get { return this._Ships; }
			private set
			{
				if (this._Ships != value)
				{
					this._Ships = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Fleets 프로퍼티
		private MemberTable<Fleet> _Fleets;
		public MemberTable<Fleet> Fleets
		{
			get { return this._Fleets; }
			private set
			{
				if (this._Fleets != value)
				{
					this._Fleets = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Combined 프로퍼티
		private bool _Combined;
		public bool Combined
		{
			get { return this._Combined; }
			set
			{
				if (this._Combined != value)
				{
					this._Combined = value;
					this.Combine(value);
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region CombinedFleet 프로퍼티
		private CombinedFleet _CombinedFleet;
		public CombinedFleet CombinedFleet
		{
			get { return this._CombinedFleet; }
			set
			{
				if (this._CombinedFleet != value)
				{
					this._CombinedFleet = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public Organization(Homeport parent)
		{
			var proxy = Proxy.Instance;
			this.homeport = parent;

			this.Ships = new MemberTable<Ship>();
			this.Fleets = new MemberTable<Fleet>();

			proxy.Register(Proxy.api_get_member_ship, e =>
			{
				var x = e.TryParse<kcsapi_ship2[]>();
				if (x == null) return;

				this.Update(x.Data);
			});
			proxy.Register(Proxy.api_get_member_ship2, e => {
				var x = e.TryParse<kcsapi_ship2[]>();
				if (x == null) return;

				this.Update(x.Data);
				this.Update(x.Fleets);
			});
			proxy.Register(Proxy.api_get_member_ship3, e => {
				var x = e.TryParse<kcsapi_ship3>();
				if (x == null) return;

				this.Update(x.Data.api_ship_data);
				this.Update(x.Data.api_deck_data);
			});

			proxy.Register(Proxy.api_get_member_deck, e =>
			{
				var x = e.TryParse<kcsapi_deck[]>();
				if (x == null) return;

				this.Update(x.Data);
			});
			proxy.Register(Proxy.api_get_member_deck_port, e => {
				var x = e.TryParse<kcsapi_deck[]>();
				if (x == null) return;

				this.Update(x.Data);
			});
			proxy.Register(Proxy.api_get_member_ship_deck, e => {
				var x = e.TryParse<kcsapi_ship_deck>();
				if (x == null) return;

				this.Update(x.Data);
			});
			proxy.Register(Proxy.api_req_hensei_preset_select, e => {
				var x = e.TryParse<kcsapi_deck>();
				if (x == null) return;

				this.Update(x.Data);
			});

			proxy.Register(Proxy.api_req_hensei_change, e => {
				var x = e.TryParse();
				if (x == null) return;

				this.Change(x);
			});
			proxy.Register(Proxy.api_req_hokyu_charge, e => {
				var x = e.TryParse<kcsapi_charge>();
				if (x == null) return;

				this.Charge(x.Data);
			});
			proxy.Register(Proxy.api_req_kaisou_powerup, e => {
				var x = e.TryParse<kcsapi_powerup>();
				if (x == null) return;

				this.Powerup(x);
			});
			proxy.Register(Proxy.api_req_kaisou_slot_exchange_index, e => {
				var x = e.TryParse<kcsapi_slot_exchange_index>();
				if (x == null) return;

				this.ExchangeSlot(x);
			});
			proxy.Register(Proxy.api_req_kaisou_slot_deprive, e => {
				var x = e.TryParse<kcsapi_slot_deprive>();
				if (x == null) return;

				this.DepriveSlotItem(x.Data);
			});

			proxy.Register(Proxy.api_req_kousyou_getship, e => {
				var x = e.TryParse<kcsapi_kdock_getship>();
				if (x == null) return;

				this.GetShip(x.Data);
			});
			proxy.Register(Proxy.api_req_kousyou_destroyship, e => {
				var x = e.TryParse<kcsapi_destroyship>();
				if (x == null) return;

				this.DestoryShip(x);
			});
			proxy.Register(Proxy.api_req_member_updatedeckname, e => {
				var x = e.TryParse();
				if (x == null) return;

				this.UpdateFleetName(x);
			});

			proxy.Register(Proxy.api_req_hensei_combined, e =>
			{
				var x = e.TryParse<kcsapi_hensei_combined>();
				if (x == null) return;

				this.Combined = x.Data.api_combined != 0;
			});

			this.SubscribeSortieSessions();
		}

		/// <summary>
		/// 칸무스가 속한 함대를 탐색
		/// </summary>
		internal Fleet GetFleet(int shipId)
		{
			return this.Fleets.Select(x => x.Value).SingleOrDefault(x => x.Ships.Any(s => s.Id == shipId));
		}

		private void UpdateFleetName(SvData data)
		{
			if (data == null || !data.IsSuccess) return;

			try
			{
				var fleet = this.Fleets[int.Parse(data.Request["api_deck_id"])];
				var name = data.Request["api_name"];

				fleet.Name = name;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("함대명 변경에 실패: {0}", ex);
			}
		}

		private void RaiseShipsChanged()
		{
			this.RaisePropertyChanged("Ships");
		}


		#region 모항 / 함대편성 (Update / Change)
		internal void Update(kcsapi_ship2[] source)
		{
			if (source.Length <= 1)
			{
				foreach (var ship in source)
				{
					var target = this.Ships[ship.api_id];
					if (target == null) continue;

					target.Update(ship);
					this.GetFleet(target.Id)?.State.Calculate();
				}
			}
			else
			{
				this.Ships = new MemberTable<Ship>(source.Select(x => new Ship(this.homeport, x)));

				if (DataStorage.Instance.IsInSortie)
				{
					foreach (var id in this.evacuatedShipsIds) this.Ships[id].Situation |= ShipSituation.Evacuation;
					foreach (var id in this.towShipIds) this.Ships[id].Situation |= ShipSituation.Tow;
				}

				foreach (var fleet in this.Fleets.Values)
				{
					fleet.State.Update();
					fleet.State.Calculate();
				}
			}
		}

		internal void Update(kcsapi_deck[] source)
		{
			if (this.Fleets.Count == source.Length)
			{
				foreach (var raw in source) this.Fleets[raw.api_id]?.Update(raw);
			}
			else
			{
				foreach (var fleet in this.Fleets) fleet.Value?.Dispose();
				this.Fleets = new MemberTable<Fleet>(source.Select(x => new Fleet(this.homeport, x)));
			}
		}

		internal void Update(kcsapi_deck source)
		{
			var fleet = this.Fleets[source.api_id];
			if (fleet != null)
			{
				fleet.Update(source);
				fleet.RaiseShipsUpdated();
			}
		}

		private void Change(SvData data)
		{
			if (data == null || !data.IsSuccess) return;

			try
			{
				var fleet = this.Fleets[int.Parse(data.Request["api_id"])];
				fleet.RaiseShipsUpdated();

				var index = int.Parse(data.Request["api_ship_idx"]);
				if (index == -1)
				{
					// 旗艦以外をすべて外すケース
					fleet.UnsetAll();
					return;
				}

				var ship = this.Ships[int.Parse(data.Request["api_ship_id"])];
				if (ship == null)
				{
					// 艦を外すケース
					fleet.Unset(index);
					return;
				}

				var currentFleet = this.GetFleet(ship.Id);
				if (currentFleet == null)
				{
					// ship が、現状どの艦隊にも所属していないケース
					fleet.Change(index, ship);
					return;
				}

				// ship が、現状いずれかの艦隊に所属しているケース
				var currentIndex = Array.IndexOf(currentFleet.Ships, ship);
				var old = fleet.Change(index, ship);

				// Fleet.Change(int, Ship) は、変更前の艦を返す (= old) ので、
				// ship の移動元 (currentFleet + currentIndex) に old を書き込みにいく
				currentFleet.Change(currentIndex, old);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("편성 변경에 실패: {0}", ex);
			}
		}

		private void Combine(bool combine)
		{
			this.CombinedFleet?.Dispose();
			this.CombinedFleet = combine
				? new CombinedFleet(this.homeport, this.Fleets.OrderBy(x => x.Key).Select(x => x.Value).Take(2).ToArray())
				: null;
		}

		private void ExchangeSlot(SvData<kcsapi_slot_exchange_index> data)
		{
			try
			{
				var ship = this.Ships[int.Parse(data.Request["api_id"])];
				if (ship == null) return;

				ship.RawData.api_slot = data.Data.api_slot;
				ship.UpdateSlots();

				var fleet = this.Fleets.Values.FirstOrDefault(x => x.Ships.Any(y => y.Id == ship.Id));
				if (fleet == null) return;

				fleet.State.Calculate();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("장비 변경에 실패: {0}", ex);
			}
		}
		#endregion

		#region 보급 / 근대화개수 (Charge / Powerup)
		private void Charge(kcsapi_charge source)
		{
			Fleet fleet = null;

			foreach (var ship in source.api_ship)
			{
				var target = this.Ships[ship.api_id];
				if (target == null) continue;

				target.Charge(ship.api_fuel, ship.api_bull, ship.api_onslot);

				if (fleet == null)
				{
					fleet = this.GetFleet(target.Id);
				}
			}

			if (fleet != null)
			{
				fleet.State.Update();
				fleet.State.Calculate();
			}
		}

		private void Powerup(SvData<kcsapi_powerup> svd)
		{
			try
			{
				this.Ships[svd.Data.api_ship.api_id]?.Update(svd.Data.api_ship);

				var items = svd.Request["api_id_items"]
					.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(int.Parse)
					.Where(x => this.Ships.ContainsKey(x))
					.Select(x => this.Ships[x])
					.ToArray();

				// (改修に使った艦娘のこと item って呼ぶのどうなの…)

				foreach (var x in items)
				{
					this.homeport.Itemyard.RemoveFromShip(x);
					this.Ships.Remove(x);
				}

				this.RaiseShipsChanged();
				this.Update(svd.Data.api_deck);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("근대화개수에 의한 갱신 실패: {0}", ex);
			}
		}
		#endregion

		#region 개장 (DepriveSlotItem)
		private void DepriveSlotItem(kcsapi_slot_deprive source)
		{
			this.Ships[source.api_ship_data.api_unset_ship.api_id]?.Update(source.api_ship_data.api_unset_ship);
			this.Ships[source.api_ship_data.api_set_ship.api_id]?.Update(source.api_ship_data.api_set_ship);
		}
		#endregion

		#region 공창 (Get / Destroy)
		private void GetShip(kcsapi_kdock_getship source)
		{
			this.homeport.Itemyard.AddFromDock(source);

			this.Ships.Add(new Ship(this.homeport, source.api_ship));
			this.RaiseShipsChanged();
		}

		private void DestoryShip(SvData<kcsapi_destroyship> svd)
		{
			try
			{
				var ship = this.Ships[int.Parse(svd.Request["api_ship_id"])];
				if (ship != null)
				{
					this.homeport.Itemyard.RemoveFromShip(ship);

					this.Ships.Remove(ship);
					this.RaiseShipsChanged();
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("해체에 의한 갱신 실패: {0}", ex);
			}
		}
		#endregion

		#region 출격 (Sortie / Homing / Escape)
		private void SubscribeSortieSessions()
		{
			var proxy = Proxy.Instance;

			proxy.Register(Proxy.api_req_map_start, e =>
			{
				var x = e.TryParse();
				if (x == null) return;

				DataStorage.Instance.IsInSortie = true;
				this.Sortie(x);
			});
			proxy.Register(Proxy.api_port, e =>
			{
				var x = e.TryParse();
				if (x == null) return;

				DataStorage.Instance.IsInSortie = false;
				this.Homing();
			});

			int[] evacuationOfferedShipIds = null;
			int[] towOfferedShipIds = null;

			proxy.Register(Proxy.api_req_combined_battle_battleresult, e => {
				var x = e.TryParse<kcsapi_combined_battle_battleresult>();
				if (x == null) return;
				if (x.Data.api_escape == null) return;
				if (this.CombinedFleet == null) return;

				var ships = this.CombinedFleet.Fleets.SelectMany(f => f.Ships).ToArray();
				evacuationOfferedShipIds = x.Data.api_escape.api_escape_idx.Select(idx => ships[idx - 1].Id).ToArray();
				towOfferedShipIds = x.Data.api_escape.api_tow_idx.Select(idx => ships[idx - 1].Id).ToArray();
			});
			proxy.Register(Proxy.api_req_combined_battle_goback_port, e => {
				var x = e.TryParse();
				if (x == null) return;

				if (DataStorage.Instance.IsInSortie
					&& evacuationOfferedShipIds != null
					&& evacuationOfferedShipIds.Length >= 1
					&& towOfferedShipIds != null
					&& towOfferedShipIds.Length >= 1)
				{
					this.evacuatedShipsIds.Add(evacuationOfferedShipIds[0]);
					this.towShipIds.Add(towOfferedShipIds[0]);
				}
			});
			proxy.Register(Proxy.api_get_member_ship_deck, e => {
				var x = e.TryParse();
				if (x == null) return;

				evacuationOfferedShipIds = null;
				towOfferedShipIds = null;
			});
		}

		private void Sortie(SvData data)
		{
			if (data == null || !data.IsSuccess) return;

			try
			{
				var id = int.Parse(data.Request["api_deck_id"]);
				var fleet = this.Fleets[id];
				fleet.Sortie();

				if (this.Combined && id == 1) this.Fleets[2].Sortie();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("함대 출격 감지 실패: {0}", ex);
			}
		}

		private void Homing()
		{
			this.evacuatedShipsIds.Clear();
			this.towShipIds.Clear();

			foreach (var ship in this.Ships.Values)
			{
				if (ship.Situation.HasFlag(ShipSituation.Evacuation)) ship.Situation &= ~ShipSituation.Evacuation;
				if (ship.Situation.HasFlag(ShipSituation.Tow)) ship.Situation &= ~ShipSituation.Tow;
			}

			foreach (var target in this.Fleets.Values)
			{
				target.Homing();
			}
		}

		private void Update(kcsapi_ship_deck source)
		{
			if (source.api_ship_data != null)
			{
				foreach (var ship in source.api_ship_data)
				{
					var target = this.Ships[ship.api_id];
					target.Update(ship);

					if (this.evacuatedShipsIds.Any(x => target.Id == x)) target.Situation |= ShipSituation.Evacuation;
					if (this.towShipIds.Any(x => target.Id == x)) target.Situation |= ShipSituation.Tow;
				}
			}

			if (source.api_deck_data != null)
			{
				foreach (var deck in source.api_deck_data)
				{
					var target = this.Fleets[deck.api_id];
					target.Update(deck);
				}
			}
		}
		#endregion
	}
}
