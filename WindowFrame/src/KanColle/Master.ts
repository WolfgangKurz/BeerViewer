import Proxy from "@/Proxy/Proxy";
import MasterTable from "@/System/MasterTable";

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
	public get ShipTypes(): MasterTable<kcsapi_mst_stype> { return this.internalShipTypes || MasterTable.Empty; }
	private internalShipTypes?: MasterTable<kcsapi_mst_stype>;

	public get Ships(): MasterTable<kcsapi_mst_ship> { return this.internalShips || MasterTable.Empty; }
	private internalShips?: MasterTable<kcsapi_mst_ship>;


	public get EquipTypes(): MasterTable<kcsapi_mst_slotitem_equiptype> { return this.internalEquipTypes || MasterTable.Empty; }
	private internalEquipTypes?: MasterTable<kcsapi_mst_slotitem_equiptype>;

	public get Equips(): MasterTable<kcsapi_mst_slotitem> { return this.internalEquips || MasterTable.Empty; }
	private internalEquips?: MasterTable<kcsapi_mst_slotitem>;

	public get UseItems(): MasterTable<kcsapi_mst_useitem> { return this.internalUseItems || MasterTable.Empty; }
	private internalUseItems?: MasterTable<kcsapi_mst_useitem>;


	public get Expeditions(): MasterTable<kcsapi_mst_mission> { return this.internalExpeditions || MasterTable.Empty; }
	private internalExpeditions?: MasterTable<kcsapi_mst_mission>;


	public get MapWorlds(): MasterTable<kcsapi_mst_maparea> { return this.internalMapWorlds || MasterTable.Empty; }
	private internalMapWorlds?: MasterTable<kcsapi_mst_maparea>;

	public get MapAreas(): MasterTable<kcsapi_mst_mapinfo> { return this.internalMapAreas || MasterTable.Empty; }
	private internalMapAreas?: MasterTable<kcsapi_mst_mapinfo>;


	public constructor() {
		Proxy.Instance.Register("/kcsapi/api_start2/getData", (req, respBuffer) => {
			ParseKcsApi<kcsapi_start2>(respBuffer, (resp) => {
				this.internalShipTypes = new MasterTable(resp.api_mst_stype);
				this.internalShips = new MasterTable(resp.api_mst_ship);

				this.internalEquipTypes = new MasterTable(resp.api_mst_slotitem_equiptype);
				this.internalEquips = new MasterTable(resp.api_mst_slotitem);

				this.internalExpeditions = new MasterTable(resp.api_mst_mission);

				this.internalMapWorlds = new MasterTable(resp.api_mst_maparea);
				this.internalMapAreas = new MasterTable(resp.api_mst_mapinfo);
			});
		});
	}
}
