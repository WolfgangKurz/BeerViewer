using System;
using System.Collections.Generic;
using System.Linq;

using BeerViewer.Network;
using BeerViewer.Models.Enums;
using BeerViewer.Models.Wrapper;
using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models
{
	public class Organization : Notifier
	{
		private readonly Homeport homeport;

		private readonly List<int> evacuatedShipsIds = new List<int>();
		private readonly List<int> towShipIds = new List<int>();

		#region Ships Property
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

		#region Fleets Property
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

		#region Combined Property
		private bool _Combined;
		public bool Combined
		{
			get { return this._Combined; }
			set
			{
				if (this._Combined != value)
				{
					this._Combined = value;
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

			proxy.Register<kcsapi_ship2[]>(Proxy.api_get_member_ship, x => this.Update(x.Data));
			proxy.Register<kcsapi_ship2[]>(Proxy.api_get_member_ship2, x =>
			{
				this.Update(x.Data);
				this.Update(x.Fleets);
			});

			proxy.Register<kcsapi_ship3>(Proxy.api_get_member_ship3, x =>
			{
				this.Update(x.Data.api_ship_data);
				this.Update(x.Data.api_deck_data);
			});

			proxy.Register<kcsapi_deck[]>(Proxy.api_get_member_deck, x => this.Update(x.Data));
			proxy.Register<kcsapi_deck[]>(Proxy.api_get_member_deck_port, x => this.Update(x.Data));
			proxy.Register<kcsapi_ship_deck>(Proxy.api_get_member_ship_deck, x => this.Update(x.Data));

			proxy.Register<kcsapi_deck>(Proxy.api_req_hensei_preset_select, x => this.Update(x.Data));

			proxy.Register(Proxy.api_req_hensei_change, e =>
			{
				var x = e.TryParse();
				if (x == null) return;

				this.Change(x);
			});
			proxy.Register<kcsapi_charge>(Proxy.api_req_hokyu_charge, x => this.Charge(x.Data));
			proxy.Register<kcsapi_powerup>(Proxy.api_req_kaisou_powerup, x => this.Powerup(x));
			proxy.Register<kcsapi_slot_exchange_index>(Proxy.api_req_kaisou_slot_exchange_index, x => this.ExchangeSlot(x));
			proxy.Register<kcsapi_slot_deprive>(Proxy.api_req_kaisou_slot_deprive, x => this.DepriveSlotItem(x.Data));

			proxy.Register<kcsapi_kdock_getship>(Proxy.api_req_kousyou_getship, x => this.GetShip(x.Data));
			proxy.Register<kcsapi_destroyship>(Proxy.api_req_kousyou_destroyship, x => this.DestoryShip(x));
			proxy.Register(Proxy.api_req_member_updatedeckname, e =>
			{
				var x = e.TryParse();
				if (x == null) return;

				this.UpdateFleetName(x);
			});
			proxy.Register<kcsapi_hensei_combined>(Proxy.api_req_hensei_combined, x => this.Combined = x.Data.api_combined != 0);

			this.SubscribeSortieSessions();
		}

		internal Fleet GetFleet(int shipId)
			=> this.Fleets.Select(x => x.Value).SingleOrDefault(x => x.Ships.Any(s => s.Id == shipId));

		private void UpdateFleetName(SvData data)
		{
			try
			{
				var fleet = this.Fleets[int.Parse(data.Request["api_deck_id"])];
				var name = data.Request["api_name"];

				fleet.Name = name;
			}
			catch { }
		}

		private void RaiseShipsChanged()
		{
			this.RaisePropertyChanged("Ships");
		}


		#region Update / Change
		internal void Update(kcsapi_ship2[] source)
		{
			if (source.Length <= 1)
			{
				foreach (var ship in source)
				{
					var target = this.Ships[ship.api_id];
					if (target == null) continue;

					target.Update(ship);
				}
			}
			else
			{
				this.Ships = new MemberTable<Ship>(source.Select(x => new Ship(this.homeport, x)));

				foreach (var id in this.evacuatedShipsIds)
					this.Ships[id].Situation |= ShipSituation.Evacuation;

				foreach (var id in this.towShipIds)
					this.Ships[id].Situation |= ShipSituation.Tow;
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
			try
			{
				var fleet = this.Fleets[int.Parse(data.Request["api_id"])];

				var index = int.Parse(data.Request["api_ship_idx"]);
				if (index == -1)
				{
					fleet.UnsetAll();
					return;
				}

				var ship = this.Ships[int.Parse(data.Request["api_ship_id"])];
				if (ship == null)
				{
					fleet.Unset(index);
					return;
				}

				var currentFleet = this.GetFleet(ship.Id);
				if (currentFleet == null)
				{
					fleet.Change(index, ship);
					return;
				}

				var currentIndex = Array.IndexOf(currentFleet.Ships, ship);
				var old = fleet.Change(index, ship);
				currentFleet.Change(currentIndex, old);

				fleet.RaiseShipsUpdated();
			}
			catch { }
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
			}
			catch { }
		}
		#endregion

		#region Charge / Powerup
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

				foreach (var x in items)
				{
					this.homeport.Itemyard.RemoveFromShip(x);
					this.Ships.Remove(x);
				}

				this.RaiseShipsChanged();
				this.Update(svd.Data.api_deck);
			}
			catch { }
		}
		#endregion

		#region DepriveSlotItem
		private void DepriveSlotItem(kcsapi_slot_deprive source)
		{
			this.Ships[source.api_ship_data.api_unset_ship.api_id]?.Update(source.api_ship_data.api_unset_ship);
			this.Ships[source.api_ship_data.api_set_ship.api_id]?.Update(source.api_ship_data.api_set_ship);
		}
		#endregion

		#region Get / Destroy
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
			catch { }
		}
		#endregion

		#region Sortie / Homing / Escape
		private void SubscribeSortieSessions()
		{
			var proxy = Proxy.Instance;

			proxy.Register(Proxy.api_req_map_start, e =>
			{
				var x = e.TryParse();
				if (x == null) return;

				this.Sortie(x);
			});
			proxy.Register(Proxy.api_port, e =>
			{
				var x = e.TryParse();
				if (x == null) return;

				this.Homing();
			});

			int[] evacuationOfferedShipIds = null;
			int[] towOfferedShipIds = null;

			proxy.Register< kcsapi_combined_battle_battleresult>(Proxy.api_req_combined_battle_battleresult, x => {
				if (x.Data.api_escape == null) return;

				var ships = this.Fleets.Where(y => y.Key == 1 || y.Key == 2)
					.SelectMany(f => f.Value.Ships).ToArray();

				evacuationOfferedShipIds = x.Data.api_escape.api_escape_idx.Select(idx => ships[idx - 1].Id).ToArray();
				towOfferedShipIds = x.Data.api_escape.api_tow_idx.Select(idx => ships[idx - 1].Id).ToArray();
			});
			proxy.Register(Proxy.api_req_combined_battle_goback_port, e => {
				var x = e.TryParse();
				if (x == null) return;

				if (evacuationOfferedShipIds != null && evacuationOfferedShipIds.Length >= 1
					&& towOfferedShipIds != null && towOfferedShipIds.Length >= 1)
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
			try
			{
				var id = int.Parse(data.Request["api_deck_id"]);
				var fleet = this.Fleets[id];

				fleet.Sortie();
				if (this.Combined && id == 1) this.Fleets[2].Sortie();
			}
			catch { }
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
				target.Homing();
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
