// tslint:disable:variable-name

import IIdentifiable from "../Interfaces/IIdentifiable";
import KanColleWrapperBase from "../Classes/KanColleWrapperBase";
import Master from "../Master";

import { kcsapi_mst_ship } from "../Raw/Master/kcsapi_mst_ship";
import { ShipTypeInfo } from "./ShipTypeInfo";
import { ShipSpeed } from "../Enums/ShipEnums";

import MinMaxPair from "../Classes/MinMaxPair";

export class ShipInfo extends KanColleWrapperBase<kcsapi_mst_ship> implements IIdentifiable {
	//#region Wrapping
	public get Id(): number {
		return this.RawData.api_id;
	}
	public get SortId(): number {
		return this.RawData.api_sortno;
	}

	public get Name(): string {
		return this.RawData.api_name;
	}
	public get ShipType(): ShipTypeInfo | null {
		return Master.Instance.ShipTypes.get(this.RawData.api_stype) || null;
	}

	public get Slots(): number {
		return this.RawData.api_slot_num;
	}
	public get SlotAircraftCounts(): number[] {
		return this.RawData.api_maxeq;
	}
	public get Speed(): ShipSpeed {
		return this.RawData.api_soku;
	}

	public get NextRemodelLevel(): number | null {
		return this.RawData.api_afterlv || null;
	}

	public get HP(): MinMaxPair {
		return new MinMaxPair(this.RawData.api_taik);
	}
	public get Armor(): MinMaxPair {
		return new MinMaxPair(this.RawData.api_souk);
	}
	public get FirePower(): MinMaxPair {
		return new MinMaxPair(this.RawData.api_houg);
	}
	public get Torpedo(): MinMaxPair {
		return new MinMaxPair(this.RawData.api_raig);
	}
	public get AA(): MinMaxPair {
		return new MinMaxPair(this.RawData.api_tyku);
	}
	public get Luck(): MinMaxPair {
		return new MinMaxPair(this.RawData.api_luck);
	}

	public get Fuel(): number {
		return this.RawData.api_fuel_max;
	}
	public get Ammo(): number {
		return this.RawData.api_bull_max;
	}

	public static readonly Empty: ShipInfo = new ShipInfo({
		api_id: 0,
		api_sortno: 0,
		api_name: "???",
		api_yomi: "???",
		api_stype: 0,
		api_afterlv: 0,
		api_aftershipid: "",
		api_taik: [0, 0],
		api_souk: [0, 0],
		api_houg: [0, 0],
		api_raig: [0, 0],
		api_tyku: [0, 0],
		api_luck: [0, 0],
		api_soku: 0,
		api_leng: 0,
		api_slot_num: 0,
		api_maxeq: [],
		api_buildtime: 0,
		api_broken: [0, 0, 0, 0],
		api_powup: [0, 0, 0, 0],
		api_backs: 0,
		api_getmes: "",
		api_afterfuel: 0,
		api_afterbull: 0,
		api_fuel_max: 0,
		api_bull_max: 0,
		api_voicef: 0
	});

	constructor(api_data: kcsapi_mst_ship) {
		super(api_data);
	}
	//#endregion

	public IsAircraftShipType(): boolean {
		return this.SlotAircraftCounts.some((x) => x > 0);
	}
}
