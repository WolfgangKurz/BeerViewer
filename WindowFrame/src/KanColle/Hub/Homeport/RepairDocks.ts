import { Component } from "vue-property-decorator";
import Proxy from "@/Proxy/Proxy";
import { ParseKcsApi } from "@KC/Functions";
import KanColleStoreClient, { StoreInterface } from "@KC/Store/KanColleStoreClient";

import { kcsapi_ndock } from "@KC/Raw/kcsapi_dock";
import { kcsapi_req_nyukyo_start, kcsapi_req_nyukyo_speedchange } from "@KC/Raw/kcsapi_repair";
import { kcsapi_port } from "@KC/Raw/kcsapi_port";

@Component
export default class RepairDocks extends KanColleStoreClient {
	constructor() {
		super();

		Proxy.Instance.Register("/kcsapi/api_get_member/ndock", (req, resp) => {
			ParseKcsApi<kcsapi_ndock[]>(resp, (x) => this.Update(x));
		});
		Proxy.Instance.Register("/kcsapi/api_req_nyukyo/start", (req, resp) => {
			ParseKcsApi<unknown, kcsapi_req_nyukyo_start>(resp, req.body, (x, y) => this.Start(y));
		});

		Proxy.Instance.Register("/kcsapi/api_port/port", (req, resp) => {
			ParseKcsApi<kcsapi_port>(resp, (x) => this.Update(x.api_ndock));
		});

		// Bucket used while normal repair
		Proxy.Instance.Register("/kcsapi/api_req_nyukyo/speedchange", (req, resp) => {
			ParseKcsApi<unknown, kcsapi_req_nyukyo_speedchange>(resp, req.body, (x, y) => this.ChangeSpeed(y));
		});
	}

	private Update(source: kcsapi_ndock[]): void {
		this.StoreRepairDocksUpdate(
			source.map<StoreInterface.RepairDock>((x) => ({
				Id: x.api_id,
				State: x.api_state,

				ShipId: x.api_ship_id,

				CompleteTime: x.api_complete_time
			}))
		);
	}

	private Start(req: kcsapi_req_nyukyo_start): void {
		if (req.api_highspeed)
			void (0); // this.StoreShipRepair(req.api_ship_id);

		// No need to process ndock data.
	}
	private ChangeSpeed(req: kcsapi_req_nyukyo_speedchange): void {
		const dock = this.StoreRepairDocks[req.api_ndock_id];
		// this.StoreShipRepair(dock.ShipId);
		this.StoreRepairDockClear(dock.Id);
	}
}
