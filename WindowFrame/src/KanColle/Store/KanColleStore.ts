import { MutationMethod } from "./StoreBase";
import { StoreInterface } from "./KanColleStoreClient";
import IdMap, { IdMapLength } from "@KC/Basic/IdMap";

import { DockState } from "@KC/Enums/DockState";
import { CombinedFleetType } from "@KC/Enums/CombinedFleetType";
import { AdmiralRank } from "@KC/Enums/AdmiralRank";

import Master from "@KC/Master";
import Homeport from "@KC/Hub/Homeport";

import Fleet from "@KC/Classes/Fleet";
import Ship from "@KC/Classes/Ship";

import { kcsapi_ship2 } from "@KC/Raw/kcsapi_ship";
import { kcsapi_deck } from "@KC/Raw/kcsapi_deck";

let instMaster: Master;
let instHomeport: Homeport;

/**
 * Initialize store managers
 */
export function InitializeStore() {
	instMaster = new Master();
	instHomeport = new Homeport();
}

/**
 * Store for KanColle data.
 *
 * Check `KanColleStoreClient` to read JSDoc.
 */
export default class KanColleStore {
	public StoreMaterials: StoreInterface.Materials = {
		Fuel: 0,
		Ammo: 0,
		Steel: 0,
		Bauxite: 0,
		Bucket: 0,
		Construction: 0,
		Development: 0,
		Improvement: 0
	};

	public StoreAdmiral: StoreInterface.Admiral = {
		MemberId: 0,
		Nickname: "",
		Comment: "",

		Level: 0,
		Rank: AdmiralRank.Novice_Lieutenant_Commander,
		Experience: 0,

		Sortie: {
			Wins: 0,
			Loses: 0,
		},

		Maximum: {
			Ships: 0,
			Equips: 0,
		},
	};

	public StoreConstructionDocks: IdMap<StoreInterface.ConstructionDock> = {};
	public StoreRepairDocks: IdMap<StoreInterface.RepairDock> = {};

	public StoreCombinedFleet: StoreInterface.CombinedFleet = {
		Combined: false,
		Type: CombinedFleetType.None
	};

	public StoreFleets: IdMap<Fleet> = {};
	public StoreShips: IdMap<Ship> = {};

	@MutationMethod()
	public StoreMaterialsUpdate(payload: StoreInterface.MaterialsPayload) {
		if (payload.Fuel)
			this.StoreMaterials.Fuel = payload.Fuel;
		if (payload.Ammo)
			this.StoreMaterials.Ammo = payload.Ammo;
		if (payload.Steel)
			this.StoreMaterials.Steel = payload.Steel;
		if (payload.Bauxite)
			this.StoreMaterials.Bauxite = payload.Bauxite;
		if (payload.Bucket)
			this.StoreMaterials.Bucket = payload.Bucket;
		if (payload.Construction)
			this.StoreMaterials.Construction = payload.Construction;
		if (payload.Development)
			this.StoreMaterials.Development = payload.Development;
		if (payload.Improvement)
			this.StoreMaterials.Improvement = payload.Improvement;
	}

	@MutationMethod()
	public StoreAdmiralUpdate(payload: StoreInterface.Admiral) {
		this.StoreAdmiral = payload;
	}

	@MutationMethod()
	public StoreAdmiralCommentUpdate(comment: string) {
		this.StoreAdmiral.Comment = comment;
	}

	@MutationMethod()
	public StoreConstructionDocksUpdate(payload: StoreInterface.ConstructionDock[]) {
		if (IdMapLength(this.StoreConstructionDocks) !== payload.length)
			this.StoreConstructionDocks = {};

		payload.forEach((dock) => this.StoreConstructionDocks[dock.Id] = dock);
	}

	@MutationMethod()
	public StoreConstructionDockClear(id: number) {
		const dock = this.StoreConstructionDocks[id];
		dock.State = DockState.Done;
		dock.CompleteTime = 0;
	}

	@MutationMethod()
	public StoreRepairDocksUpdate(payload: StoreInterface.RepairDock[]) {
		if (IdMapLength(this.StoreRepairDocks) !== payload.length)
			this.StoreRepairDocks = {};

		payload.forEach((dock) => this.StoreRepairDocks[dock.Id] = dock);
	}

	@MutationMethod()
	public StoreRepairDockClear(id: number) {
		const dock = this.StoreRepairDocks[id];
		dock.State = DockState.Done;
		dock.CompleteTime = 0;
	}

	@MutationMethod()
	public StoreFleetCombinedUpdate(payload: StoreInterface.CombinedFleet) {
		this.StoreCombinedFleet = payload;
	}

	@MutationMethod()
	public StoreFleetsUpdate(payload: Fleet[]) {
		if (IdMapLength(this.StoreFleets) !== payload.length)
			this.StoreFleets = {};

		payload.forEach((dock) => this.StoreFleets[dock.Id] = dock);
	}

	@MutationMethod()
	public StoreFleetUpdate(fleet: kcsapi_deck) {
		this.StoreFleets[fleet.api_id].Update(fleet);
	}

	@MutationMethod()
	public StoreFleetChangeShip(payload: { fleet: number, index: number, ship: number }) {
		this.StoreFleets[payload.fleet].ChangeShip(payload.index, payload.ship);
	}

	@MutationMethod()
	public StoreShipsUpdate(payload: Ship[]) {
		if (IdMapLength(this.StoreShips) !== payload.length)
			this.StoreShips = {};

		payload.forEach((ship) => this.StoreShips[ship.Id] = ship);
	}

	@MutationMethod()
	public StoreShipUpdate(ship: kcsapi_ship2) {
		this.StoreShips[ship.api_id].Update(ship);
	}

	@MutationMethod()
	public StoreShipEvacuate(id: number) {
		this.StoreShips[id].Evacuate();
	}

	@MutationMethod()
	public StoreShipTow(id: number) {
		this.StoreShips[id].Tow();
	}
}
