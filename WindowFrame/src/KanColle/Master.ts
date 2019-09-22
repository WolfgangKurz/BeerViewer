import { Component } from "vue-property-decorator";
import Proxy from "@/Proxy/Proxy";
import { ParseKcsApi } from "@KC/Functions";
import KanColleStoreClient from "@KC/Store/KanColleStoreClient";
import { ConvertToIdMap } from "./Basic/IdMap";

import { kcsapi_start2 } from "@KC/Raw/Master/kcsapi_start2";

/**
 * Class that contains parsed `api_start2` data objects.
 */
@Component
export default class MasterClass extends KanColleStoreClient {
	constructor() {
		super();

		Proxy.Instance.Register("/kcsapi/api_start2/getData", (req, resp) => {
			ParseKcsApi<kcsapi_start2>(resp, (x) => {
				this.StoreMasterUpdate({
					ShipTypes: ConvertToIdMap(x.api_mst_stype),
					Ships: ConvertToIdMap(x.api_mst_ship),

					EquipTypes: ConvertToIdMap(x.api_mst_slotitem_equiptype),
					Equips: ConvertToIdMap(x.api_mst_slotitem),
					UseItems: ConvertToIdMap(x.api_mst_useitem),

					Expeditions: ConvertToIdMap(x.api_mst_mission),

					MapWorlds: ConvertToIdMap(x.api_mst_maparea),
					MapAreas: ConvertToIdMap(x.api_mst_mapinfo)
				});
			});
		});
	}
}
