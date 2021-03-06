import { HTTPRequest } from "System/Exports/API";

export interface kcsapi_mapinfo_airbase {
	api_air_base: kcsapi_airbase_corps[];
}

export interface kcsapi_airbase_corps {
	api_area_id: number;
	api_rid: number;
	api_name: string;
	api_distance: number;
	api_action_kind: number;
	api_plane_info: kcsapi_plane_info[];
}

export interface kcsapi_airbase_corps_supply {
	api_after_fuel: number;
	api_after_bauxite: number;
	api_distance: number;
	api_plane_info: kcsapi_plane_info[];
}

export interface kcsapi_airbase_corps_set_plane {
	api_after_bauxite: number;
	api_distance: number;
	api_plane_info: kcsapi_plane_info[];
}

export interface kcsapi_plane_info {
	api_squadron_id: number;
	api_state: number; // ?
	api_slotid: number;
	api_count: number;
	api_max_count: number;
	api_cond: number;
}

export interface kcsapi_req_air_corps_set_plane extends HTTPRequest {
	api_item_id: number;
}