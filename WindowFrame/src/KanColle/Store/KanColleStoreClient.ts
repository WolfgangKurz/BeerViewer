// tslint:disable:no-namespace
import Vue from "vue";
import { Mutation, State, Getter } from "vuex-class";
import IdMap from "@KC/Basic/IdMap";

import { DockState } from "@KC/Enums/DockState";
import { CombinedFleetType } from "@KC/Enums/CombinedFleetType";
import { AdmiralRank } from "@KC/Enums/AdmiralRank";

import Fleet from "@KC/Classes/Fleet";
import Ship from "@KC/Classes/Ship";

import { kcsapi_ship2 } from "@KC/Raw/kcsapi_ship";
import { kcsapi_deck } from "@KC/Raw/kcsapi_deck";
import { kcsapi_mst_stype, kcsapi_mst_ship } from "@KC/Raw/Master/kcsapi_mst_ship";
import { kcsapi_mst_slotitem_equiptype, kcsapi_mst_slotitem } from "@KC/Raw/Master/kcsapi_mst_slotitem";
import { kcsapi_mst_useitem } from "@KC/Raw/Master/kcsapi_mst_useitem";
import { kcsapi_mst_mission } from "@KC/Raw/kcsapi_mission";
import { kcsapi_mst_maparea, kcsapi_mst_mapinfo } from "@KC/Raw/Master/kcsapi_map";

/**
 * Namespace that contains interfaces of `KanColleStore`.
 */
export namespace StoreInterface {
	/** Master dataset interface */
	export interface Master {
		ShipTypes: IdMap<kcsapi_mst_stype>;
		Ships: IdMap<kcsapi_mst_ship>;

		EquipTypes: IdMap<kcsapi_mst_slotitem_equiptype>;
		Equips: IdMap<kcsapi_mst_slotitem>;
		UseItems: IdMap<kcsapi_mst_useitem>;

		Expeditions: IdMap<kcsapi_mst_mission>;

		MapWorlds: IdMap<kcsapi_mst_maparea>;
		MapAreas: IdMap<kcsapi_mst_mapinfo>;
	}

	/** Material data interface */
	export interface Materials {
		Fuel: number;
		Ammo: number;
		Steel: number;
		Bauxite: number;
		Bucket: number;
		Construction: number;
		Development: number;
		Improvement: number;
	}

	/** Material data interface for payload */
	export interface MaterialsPayload {
		Fuel?: number;
		Ammo?: number;
		Steel?: number;
		Bauxite?: number;
		Bucket?: number;
		Construction?: number;
		Development?: number;
		Improvement?: number;
	}

	/** Admiral data interface */
	export interface Admiral {
		MemberId: number;
		Nickname: string;
		Comment: string;

		Level: number;
		Rank: AdmiralRank;
		Experience: number;

		Sortie: {
			Wins: number;
			Loses: number;
		};

		Maximum: {
			Ships: number;
			Equips: number;
		};
	}

	/** Construction dock data interface */
	export interface ConstructionDock {
		Id: number;
		State: DockState;

		ShipId: number;

		CompleteTime: number;

		// private _Ship: ShipInfo | null = null;
	}

	/** Repair dock data interface */
	export interface RepairDock {
		Id: number;
		State: DockState;

		ShipId: number;

		CompleteTime: number;
	}

	/** Combined fleet data interface */
	export interface CombinedFleet {
		Combined: boolean;
		Type: CombinedFleetType;
	}
}

/**
 * KanColleStore implementation class for Vue Component/Pages
 */
export default class KanColleStoreClient extends Vue {
	//#region Master
	@Getter
	public StoreMaster!: StoreInterface.Master;

	@Mutation("StoreMasterUpdate")
	public StoreMasterUpdate!: (payload: StoreInterface.Master) => void;
	//#endregion

	//#region Materials
	/**
	 * Material data of `KanColleStore`.
	 */
	@State
	public StoreMaterials!: StoreInterface.Materials;

	/**
	 * Update new materials data to store.
	 */
	@Mutation("StoreMaterialsUpdate")
	public StoreMaterialsUpdate!: (payload: StoreInterface.MaterialsPayload) => void;
	//#endregion

	//#region Admiral
	/**
	 * Admiral data of `KanColleStore`.
	 */
	@State
	public StoreAdmiral!: StoreInterface.Admiral;

	/**
	 * Update entire admiral data to store.
	 */
	@Mutation("StoreAdmiralUpdate")
	public StoreAdmiralUpdate!: (payload: StoreInterface.Admiral) => void;

	/**
	 * Update admiral comment data to store.
	 */
	@Mutation("StoreAdmiralCommentUpdate")
	public StoreAdmiralCommentUpdate!: (comment: string) => void;
	//#endregion

	//#region Construction dock
	/**
	 * Construction dock data of `KanColleStore`.
	 */
	@State
	public StoreConstructionDocks!: IdMap<StoreInterface.ConstructionDock>;

	/**
	 * Update construction dock list to store.
	 */
	@Mutation("StoreConstructionDocksUpdate")
	public StoreConstructionDocksUpdate!: (payload: StoreInterface.ConstructionDock[]) => void;

	@Mutation("StoreConstructionDockClear")
	public StoreConstructionDockClear!: (id: number) => void;
	//#endregion

	//#region Repair dock
	/**
	 * Repair dock data of `KanColleStore`.
	 */
	@State
	public StoreRepairDocks!: IdMap<StoreInterface.RepairDock>;

	/**
	 * Update Repair dock list to store.
	 */
	@Mutation("StoreRepairDocksUpdate")
	public StoreRepairDocksUpdate!: (payload: StoreInterface.RepairDock[]) => void;

	@Mutation("StoreRepairDockClear")
	public StoreRepairDockClear!: (id: number) => void;
	//#endregion

	//#region Combined fleet
	/**
	 * Combined fleet data of `KanColleStore`.
	 */
	@State
	public StoreCombinedFleet!: StoreInterface.CombinedFleet;

	@Mutation("StoreFleetCombinedUpdate")
	public StoreFleetCombinedUpdate!: (payload: StoreInterface.CombinedFleet) => void;
	//#endregion

	//#region Fleets and Ships
	@State
	public StoreFleets!: IdMap<Fleet>;

	@State
	public StoreShips!: IdMap<Ship>;

	@Mutation("StoreFleetsUpdate")
	public StoreFleetsUpdate!: (payload: Fleet[]) => void;

	@Mutation("StoreFleetUpdate")
	public StoreFleetUpdate!: (fleet: kcsapi_deck) => void;

	@Mutation("StoreFleetUnset")
	public StoreFleetUnset!: (payload: { fleet: number, ship: number }) => void;

	@Mutation("StoreFleetUnsetAll")
	public StoreFleetUnsetAll!: (id: number) => void;

	@Mutation("StoreFleetChangeShip")
	public StoreFleetChangeShip!: (payload: { fleet: number, index: number, ship: number }) => void;


	@Mutation("StoreShipsUpdate")
	public StoreShipsUpdate!: (payload: Ship[]) => void;

	@Mutation("StoreShipUpdate")
	public StoreShipUpdate!: (ship: kcsapi_ship2) => void;

	@Mutation("StoreShipEvacuate")
	public StoreShipEvacuate!: (id: number) => void;

	@Mutation("StoreShipTow")
	public StoreShipTow!: (id: number) => void;
	//#endregion
}
