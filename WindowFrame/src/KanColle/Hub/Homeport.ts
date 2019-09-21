import { Component } from "vue-property-decorator";
import Proxy from "@/Proxy/Proxy";
import { ParseKcsApi } from "@KC/Functions";
import KanColleStoreClient, { StoreInterface } from "@KC/Store/KanColleStoreClient";

import Materials from "./Homeport/Materials";
import Admiral from "./Homeport/Admiral";
import ConstructionDocks from "./Homeport/ConstructionDocks";
import RepairDocks from "./Homeport/RepairDocks";
import Fleets from "./Homeport/Fleets";

import { CombinedFleetType } from "@KC/Enums/CombinedFleetType";

import { kcsapi_require_info } from "@KC/Raw/kcsapi_require_info";
import { kcsapi_destroyship, kcsapi_req_kousyou_destroyship } from "@KC/Raw/kcsapi_ship";
import { kcsapi_req_hensei_combined } from "@KC/Raw/kcsapi_deck";
import { kcsapi_hensei_combined } from "@KC/Raw/kcsapi_hensei";
import { kcsapi_kdock_getship } from "@KC/Raw/kcsapi_dock";

@Component
export default class Homeport extends KanColleStoreClient {
	private Materials: Materials = new Materials();
	private Admiral: Admiral = new Admiral();

	private ConstructionDocks: ConstructionDocks = new ConstructionDocks();
	private RepairDocks: RepairDocks = new RepairDocks();

	private Fleets: Fleets = new Fleets();

	constructor() {
		super();

		Proxy.Instance.Register("/kcsapi/api_get_member/require_info", (req, resp) => {
			ParseKcsApi<kcsapi_require_info>(resp, () => {
				// this.Equipments.Update(x.api_slot_item);
			});
		});

		Proxy.Instance.Register("/kcsapi/api_req_kousyou/getship", (req, resp) => {
			// ParseKcsApi<kcsapi_kdock_getship>(resp, (x) => this.GetShipFromConstruction(x));
		});
		Proxy.Instance.Register("/kcsapi/api_req_kousyou/destroyship", (req, resp) => {
			// ParseKcsApi<kcsapi_destroyship, kcsapi_req_kousyou_destroyship>(resp, req.body, (x, y) => this.DestroyShip(x, y));
		});

		// Combine fleet
		Proxy.Instance.Register("/kcsapi/api_req_hensei/combined", (req, resp) => {
			ParseKcsApi<kcsapi_hensei_combined, kcsapi_req_hensei_combined>(resp, req.body, (x, y) => {
				this.StoreFleetCombinedUpdate({
					Combined: x.api_combined !== 0,
					Type: y.api_combined_type as CombinedFleetType
				});
			});
		});
	}
}
