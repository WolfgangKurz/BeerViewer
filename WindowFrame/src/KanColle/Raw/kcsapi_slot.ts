// tslint:disable:class-name
import { kcsapi_ship2 } from "@KC/Raw/kcsapi_ship";

export interface kcsapi_slot_deprive {
	api_ship_data: kcsapi_slot_deprive_ship_data;
	api_unset_list: kcsapi_slot_deprive_unset_list;
}

export interface kcsapi_slot_deprive_ship_data {
	api_set_ship: kcsapi_ship2;
	api_unset_ship: kcsapi_ship2;
}

export interface kcsapi_slot_deprive_unset_list {
	api_type3No: number;
	api_slot_list: number[];
}

export interface kcsapi_slot_exchange_index {
	api_slot: [number, number, number, number, number];
}

export interface kcsapi_slot_data {
	api_slottype1: number[];
	api_slottype2: number[];
	api_slottype3: number[];
	api_slottype4: number[];
	api_slottype5: number[];
	api_slottype6: number[];
	api_slottype7: number[];
	api_slottype8: number[];
	api_slottype9: number[];
	api_slottype10: number[];
	api_slottype11: number[];
	api_slottype12: number[];
	api_slottype13: number[];
	api_slottype14: number[];
	api_slottype15: number[];
	api_slottype16: number[];
	api_slottype17: number[];
	api_slottype18: number[];
	api_slottype19: number[];
	api_slottype20: number[];
	api_slottype21: number[];
	api_slottype22: number[];
	api_slottype23: number[];
}

export interface kcsapi_req_kaisou_slot_exchange_index {
	api_id: number;
}
