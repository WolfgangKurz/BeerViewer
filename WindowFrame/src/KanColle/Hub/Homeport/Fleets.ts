import { Component } from "vue-property-decorator";
import Proxy from "@/Proxy/Proxy";
import { ParseKcsApi } from "@KC/Functions";
import KanColleStoreClient, { StoreInterface } from "@KC/Store/KanColleStoreClient";

import { IdMapArray } from "@KC/Basic/IdMap";

import Fleet from "@KC/Classes/Fleet";
import Ship from "@KC/Classes/Ship";

import { KcsApiDeck, KcsApiShipDeck } from "@KC/Interfaces/KcsApiDeck";
import { ShipState } from "@KC/Enums/ShipEnums";

import { kcsapi_deck } from "@KC/Raw/kcsapi_deck";
import { kcsapi_port } from "@KC/Raw/kcsapi_port";
import { kcsapi_ship2, kcsapi_ship3, kcsapi_ship_deck } from "@KC/Raw/kcsapi_ship";
import { kcsapi_req_hensei_change } from "@KC/Raw/kcsapi_hensei";
import { kcsapi_charge } from "@KC/Raw/kcsapi_charge";
import { kcsapi_powerup, kcsapi_req_kaisou_powerup } from "@KC/Raw/kcsapi_powerup";
import { kcsapi_slot_exchange_index, kcsapi_req_kaisou_slot_exchange_index, kcsapi_slot_deprive } from "@KC/Raw/kcsapi_slot";

@Component
export default class Fleets extends KanColleStoreClient {
	constructor() {
		super();

		Proxy.Instance.Register("/kcsapi/api_port/port", (req, resp) => {
			ParseKcsApi<kcsapi_port>(resp, (x) => {
				this.UpdateShips(x.api_ship);
				this.UpdateDecks(x.api_deck_port);
			});
		});

		// For fleets
		Proxy.Instance.Register("/kcsapi/api_get_member/ship", (req, resp) => {
			ParseKcsApi<kcsapi_ship2[]>(resp, (x) => this.UpdateShips(x));
		});
		Proxy.Instance.Register("/kcsapi/api_get_member/ship2", (req, resp) => {
			ParseKcsApi<kcsapi_ship2[]>(resp, (x, raw) => {
				this.UpdateShips(x);
				this.UpdateShips((raw as KcsApiShipDeck).api_deck_data);
			});
		});
		Proxy.Instance.Register("/kcsapi/api_get_member/ship3", (req, resp) => {
			ParseKcsApi<kcsapi_ship3>(resp, (x) => {
				this.UpdateShips(x.api_ship_data);
				this.UpdateDecks(x.api_deck_data);
			});
		});

		Proxy.Instance.Register("/kcsapi/api_get_member/deck", (req, resp) => {
			ParseKcsApi<kcsapi_deck[]>(resp, (x) => this.UpdateDecks(x));
		});
		Proxy.Instance.Register("/kcsapi/api_get_member/deck_port", (req, resp) => {
			ParseKcsApi<kcsapi_deck[]>(resp, (x) => this.UpdateDecks(x));
		});
		Proxy.Instance.Register("/kcsapi/api_req_hensei/preset_select", (req, resp) => {
			ParseKcsApi<kcsapi_deck>(resp, (x) => this.UpdateDeck(x));
		});
		Proxy.Instance.Register("/kcsapi/api_get_member/ship_deck", (req, resp) => {
			ParseKcsApi<kcsapi_ship_deck>(resp, (x) => this.UpdateShipDeck(x));
		});

		Proxy.Instance.Register("/kcsapi/api_req_hensei/change", (req, resp) => {
			ParseKcsApi<unknown, kcsapi_req_hensei_change>(resp, req.body, (x, y) => this.ChangeComposite(y));
		});
		Proxy.Instance.Register("/kcsapi/api_req_hokyu/charge", (req, resp) => {
			// ParseKcsApi<kcsapi_charge>(resp, (x) => this.SupplyShip(x));
		});
		Proxy.Instance.Register("/kcsapi/api_req_kaisou/powerup", (req, resp) => {
			// ParseKcsApi<kcsapi_powerup, kcsapi_req_kaisou_powerup>(resp, req.body, (x, y) => this.ImproveShip(x, y));
		});
		Proxy.Instance.Register("/kcsapi/api_req_kaisou/slot_exchange_index", (req, resp) => {
			// ParseKcsApi<kcsapi_slot_exchange_index, kcsapi_req_kaisou_slot_exchange_index>(resp, req.body, (x, y) => this.ExchangeEquip(x, y));
		});
		Proxy.Instance.Register("/kcsapi/api_req_kaisou/slot_deprive", (req, resp) => {
			// ParseKcsApi<kcsapi_slot_deprive>(resp, (x) => this.DepriveEquipment(x));
		});
	}

	private UpdateDecks(source: kcsapi_deck[]) {
		this.StoreFleetsUpdate(source.map((x) => new Fleet(x)));
	}
	private UpdateDeck(source: kcsapi_deck) {
		this.StoreFleets[source.api_id].Update(source);
	}

	private UpdateShips(source: kcsapi_ship2[]) {
		if (source.length <= 1) {
			const ship = source[0];
			this.StoreShips[ship.api_id].Update(ship);
		} else {
			const shipsEvacuated: number[] = IdMapArray(this.StoreShips)
				.filter((x) => x.State & ShipState.Evacuation)
				.map((x) => x.Id);
			const shipsTow: number[] = IdMapArray(this.StoreShips)
				.filter((x) => x.State & ShipState.Tow)
				.map((x) => x.Id);

			this.StoreShipsUpdate(source.map((x) => new Ship().Update(x)));

			for (const id of shipsEvacuated) this.StoreShipEvacuate(id);
			for (const id of shipsTow) this.StoreShipTow(id);
		}
	}
	private UpdateShipDeck(source: kcsapi_ship_deck) {
		if (source.api_ship_data) {
			const shipsEvacuated: number[] = IdMapArray(this.StoreShips)
				.filter((x) => x.State & ShipState.Evacuation)
				.map((x) => x.Id);
			const shipsTow: number[] = IdMapArray(this.StoreShips)
				.filter((x) => x.State & ShipState.Tow)
				.map((x) => x.Id);

			for (const ship of source.api_ship_data) {
				this.StoreShipUpdate(ship);

				if (shipsEvacuated.includes(ship.api_id)) this.StoreShipEvacuate(ship.api_id);
				if (shipsTow.includes(ship.api_id)) this.StoreShipTow(ship.api_id);
			}
		}

		if (source.api_deck_data) {
			for (const deck of source.api_deck_data)
				this.StoreFleetUpdate(deck);
		}
	}

	private ChangeComposite(data: kcsapi_req_hensei_change): void {
		const id = data.api_id;
		if (!id) return;

		const fleet = this.StoreFleets[id];
		if (!fleet) return;

		const index = data.api_ship_idx;
		const shipId = data.api_ship_id;
		if (index === 0 && shipId === -2) {
			// Unset all except flagship
			this.StoreFleetUnsetAll(id);
			return;
		}

		const ship = this.StoreShips[shipId];
		if (!ship) {
			// Unset single ship
			this.StoreFleetUnset({
				fleet: id,
				ship: index
			});
			return;
		}

		const currentFleet: Fleet | undefined = this.GetFleetFromShipId(ship.Id);
		if (!currentFleet) {
			// Not in fleet, set to fleet
			this.StoreFleetChangeShip({
				fleet: id,
				index,
				ship: ship.Id
			});
			return;
		}

		// Change ship position
		this.StoreFleetChangeShip({
			fleet: currentFleet.Id,
			index,
			ship: ship.Id
		});
	}

	private GetFleetFromShipId(shipId: number): Fleet | undefined {
		return IdMapArray(this.StoreFleets)
			.find((x) => x.Ships.some((s) => s === shipId));
	}
}
