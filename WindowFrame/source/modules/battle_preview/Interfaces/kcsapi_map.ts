import { EventKind, EventId } from "../Enums/Event";
import { HTTPRequest } from "System/Exports/API";

export interface kcsapi_map_start {
	api_maparea_id: number;
	api_mapinfo_no: number;
	api_no: number;

	api_event_id: EventId;
	api_event_kind: EventKind;
	api_eventmap: any;

	api_happening: MapHappening;
	api_itemget?: MapItemGet | MapItemGet[];
	api_itemget_eo_comment?: MapItemGet;

	api_destruction_battle?: MapDestructionBattle; // LBAS raid

	/** If zero, no nodes to go (End of map reached) */
	api_next: number;
}
export interface kcsapi_map_next extends kcsapi_map_start {
	api_m1?: number; // Map extended?
}

export interface MapHappening {
	api_type: number;
	api_count: number;
	api_usemst: number;
	api_mst_id: number;
	api_icon_id: number;
	api_dentan: number;
}
export interface MapItemGet {
	api_getcount: number;
	api_icon_id: number;
	api_id: number;
	api_name: string;
	api_usemst: number;
}
export interface MapDestructionBattle {
	api_f_nowhps: number[];
	api_f_maxhps: number[];

	api_lost_kind: number;
	api_m1?: number; // Map extended?
}

export interface kcsapi_req_map_start extends HTTPRequest {
	api_deck_id: number;
}
export interface kcsapi_req_map_next extends kcsapi_req_map_start {
}