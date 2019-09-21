import Proxy from "@/Proxy/Proxy";
import IdMap, { ConvertToIdMap } from "./Basic/IdMap";

import { ParseKcsApi } from "@KC/Functions";

import { kcsapi_start2 } from "@KC/Raw/Master/kcsapi_start2";
import { kcsapi_mst_stype, kcsapi_mst_ship } from "@KC/Raw/Master/kcsapi_mst_ship";
import { kcsapi_mst_slotitem_equiptype, kcsapi_mst_slotitem } from "@KC/Raw/Master/kcsapi_mst_slotitem";
import { kcsapi_mst_useitem } from "@KC/Raw/Master/kcsapi_mst_useitem";
import { kcsapi_mst_mission } from "@KC/Raw/kcsapi_mission";
import { kcsapi_mst_maparea, kcsapi_mst_mapinfo } from "@KC/Raw/Master/kcsapi_map";

/**
 * Class that contains parsed `api_start2` data objects.
 */
export default class Master {
	private ShipTypes: IdMap<kcsapi_mst_stype> = {};
	private Ships: IdMap<kcsapi_mst_ship> = {};

	private EquipTypes: IdMap<kcsapi_mst_slotitem_equiptype> = {};
	private Equips: IdMap<kcsapi_mst_slotitem> = {};
	private UseItems: IdMap<kcsapi_mst_useitem> = {};

	private Expeditions: IdMap<kcsapi_mst_mission> = {};

	private MapWorlds: IdMap<kcsapi_mst_maparea> = {};
	private MapAreas: IdMap<kcsapi_mst_mapinfo> = {};


	public constructor() {
		Proxy.Instance.Register("/kcsapi/api_start2/getData", (req, respBuffer) => {
			ParseKcsApi<kcsapi_start2>(respBuffer, (resp) => {
				this.ShipTypes = ConvertToIdMap(resp.api_mst_stype);
				this.Ships = ConvertToIdMap(resp.api_mst_ship);

				this.EquipTypes = ConvertToIdMap(resp.api_mst_slotitem_equiptype);
				this.Equips = ConvertToIdMap(resp.api_mst_slotitem);

				this.Expeditions = ConvertToIdMap(resp.api_mst_mission);

				this.MapWorlds = ConvertToIdMap(resp.api_mst_maparea);
				this.MapAreas = ConvertToIdMap(resp.api_mst_mapinfo);
			});
		});
	}
}
