import { kcsapi_slot_data } from "./kcsapi_slot";
import { kcsapi_deck } from "./kcsapi_deck";
import { HTTPRequest } from "System/Exports/API";

export interface kcsapi_ship2 {
	api_id: number;
	api_sortno: number;
	api_ship_id: number;
	api_lv: number;
	api_exp: [number, number];
	api_nowhp: number;
	api_maxhp: number;
	api_soku: number;
	api_leng: number;
	api_slot: [number, number, number, number, number];
	api_onslot: [number, number, number, number, number];
	api_kyouka: [number, number, number, number, number];
	api_backs: number;
	api_fuel: number;
	api_bull: number;
	api_slotnum: number;
	api_ndock_time: number;
	api_ndock_item: [number, number];
	api_srate: number;
	api_cond: number;
	api_karyoku: [number, number];
	api_raisou: [number, number];
	api_taiku: [number, number];
	api_soukou: [number, number];
	api_kaihi: [number, number];
	api_taisen: [number, number];
	api_sakuteki: [number, number];
	api_lucky: [number, number];
	api_locked: number;
	api_locked_equip: number;
	api_sally_area: number;
	api_slot_ex: number;
}
export interface kcsapi_ship3 {
	api_ship_data: kcsapi_ship2[];
	api_deck_data: kcsapi_deck[];

	/** Unused at this project */
	api_slot_data: kcsapi_slot_data;
}

export interface kcsapi_ship_deck {
	api_ship_data: kcsapi_ship2[];
	api_deck_data: kcsapi_deck[];
}

export interface kcsapi_destroyship {
	api_material: number[];
}

export interface kcsapi_req_kousyou_destroyship extends HTTPRequest {
	/** "," separated string or single number */
	api_ship_id: number | string;
}