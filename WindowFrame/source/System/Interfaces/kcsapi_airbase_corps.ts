interface kcsapi_mapinfo_airbase {
	api_air_base: kcsapi_airbase_corps[];
}

interface kcsapi_airbase_corps {
	api_area_id: number;
	api_rid: number;
	api_name: string;
	api_distance: number;
	api_action_kind: number;
	api_plane_info: kcsapi_plane_info[];
}

interface kcsapi_airbase_corps_supply {
	api_after_fuel: number;
	api_after_bauxite: number;
	api_distance: number;
	api_plane_info: kcsapi_plane_info[];
}

interface kcsapi_airbase_corps_set_plane {
	api_after_bauxite: number;
	api_distance: number;
	api_plane_info: kcsapi_plane_info[];
}

interface kcsapi_plane_info {
	api_squadron_id: number;
	api_state: number; // ?
	api_slotid: number;
	api_count: number;
	api_max_count: number;
	api_cond: number;
}
