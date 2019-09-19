// tslint:disable:class-name
import { kcsapi_mst_ship, kcsapi_mst_stype } from "@KC/Raw/Master/kcsapi_mst_ship";
import { kcsapi_mst_slotitem_equiptype, kcsapi_mst_slotitem } from "@KC/Raw/Master/kcsapi_mst_slotitem";
import { kcsapi_mst_useitem } from "@KC/Raw/Master/kcsapi_mst_useitem";
import { kcsapi_mst_maparea, kcsapi_mst_mapinfo } from "@KC/Raw/Master/kcsapi_map";
import { kcsapi_mst_mission } from "@KC/Raw/kcsapi_mission";

export interface kcsapi_start2 {
	/** Ship master information table */
	api_mst_ship: kcsapi_mst_ship[];

	/** Ship type master information table */
	api_mst_stype: kcsapi_mst_stype[];

	/** Equipment master information table */
	api_mst_slotitem: kcsapi_mst_slotitem[];

	/** Equipment type master information table */
	api_mst_slotitem_equiptype: kcsapi_mst_slotitem_equiptype[];

	/** Account item master information table */
	api_mst_useitem: kcsapi_mst_useitem[];

	/**
	 * Map world (Big category) master information table
	 *
	 * `World-Area-Node` for map.
	 * > ex) _**1**_-2-A
	 */
	api_mst_maparea: kcsapi_mst_maparea[];

	/**
	 * Map area (Medium category) master information table
	 *
	 * `World-Area-Node` for map.
	 * > ex) 1-_**2**_-A
	 */
	api_mst_mapinfo: kcsapi_mst_mapinfo[];

	/** Expedition master information table */
	api_mst_mission: kcsapi_mst_mission[];
}
