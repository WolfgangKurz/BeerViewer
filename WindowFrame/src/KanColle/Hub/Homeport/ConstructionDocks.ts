import { Component } from "vue-property-decorator";
import Proxy from "@/Proxy/Proxy";
import { ParseKcsApi } from "@KC/Functions";
import KanColleStoreClient, { StoreInterface } from "@KC/Store/KanColleStoreClient";

import { kcsapi_kdock, kcsapi_kdock_getship, kcsapi_req_kousyou_createship_speedchange } from "@KC/Raw/kcsapi_dock";
import { kcsapi_require_info } from "@KC/Raw/kcsapi_require_info";

@Component
export default class ConstructionDock extends KanColleStoreClient {
	constructor() {
		super();

		Proxy.Instance.Register("/kcsapi/api_get_member/kdock", (req, resp) => {
			ParseKcsApi<kcsapi_kdock[]>(resp, (x) => this.Update(x));
		});
		Proxy.Instance.Register("/kcsapi/api_req_kousyou/getship", (req, resp) => {
			ParseKcsApi<kcsapi_kdock_getship>(resp, (x) => this.GetShip(x));
		});

		Proxy.Instance.Register("/kcsapi/api_get_member/require_info", (req, resp) => {
			ParseKcsApi<kcsapi_require_info>(resp, (x) => this.Update(x.api_kdock));
		});

		// Construction material used while normal repair
		Proxy.Instance.Register("/kcsapi/api_req_kousyou/createship_speedchange", (req, resp) => {
			ParseKcsApi<unknown, kcsapi_req_kousyou_createship_speedchange>(resp, req.body, (x, y) => this.ChangeSpeed(y));
		});
	}

	private Update(source: kcsapi_kdock[]) {
		this.StoreConstructionDocksUpdate(
			source.map<StoreInterface.ConstructionDock>((x) => ({
				Id: x.api_id,
				State: x.api_state,

				ShipId: x.api_created_ship_id,

				CompleteTime: x.api_complete_time
			}))
		);
	}
	private GetShip(source: kcsapi_kdock_getship) {
		this.Update(source.api_kdock);
	}

	private ChangeSpeed(req: kcsapi_req_kousyou_createship_speedchange) {
		if (req.api_highspeed !== 1) return;
		this.StoreConstructionDockClear(req.api_kdock_id);
	}
}
