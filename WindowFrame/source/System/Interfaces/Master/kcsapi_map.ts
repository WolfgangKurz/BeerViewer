/** Master information interface for Map world */
export interface kcsapi_mst_maparea {
	/** Internal world id */
	api_id: number;

	/** Name */
	api_name: string;

	/** Type
	 * 
	 * - 0 : Normal world
	 * - 1 : Limited world (Event)
	 */
	api_type: number;
}

/** Master information interface for Map area */
export interface kcsapi_mst_mapinfo {
	/** Map `{world}{area}` id */
	api_id: number;

	/** Map world id */
	api_maparea_id: number;

	/** Map area id */
	api_no: number;

	/** Name */
	api_name: string;

	/** Star count */
	api_level: number;

	/** Operation name */
	api_opetext: string;

	/** Description */
	api_infotext: string;

	/** Earnable item list */
	api_item: [number, number, number, number];

	/** Map hp if boss gauge exists (like Event bosses) */
	api_max_maphp: number | null;

	/** Map hp count if count gauge exists (like 1-5) */
	api_required_defeat_count: number | null;

	/** Sailing limit flags (bit flag)
	 * 
	 * `[SingleFleet, CombinedFleet, Unknown]`
	 * 
	 * SingleFleet
	 * - 0 : Cannot sail
	 * - 1 : Can sail
	 * 
	 * CombinedFleet
	 * - 0 : Cannot sail with combined fleet
	 * - 1 : Can sail with Surface Task Force fleet
	 * - 2 : Can sail with Carrier Task Force fleet
	 * - 4 : Can sail with Transport Escort fleet
	 */
	api_sally_flag: [number, number, number];
}