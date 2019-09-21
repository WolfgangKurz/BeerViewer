import { Component } from "vue-property-decorator";
import Proxy from "@/Proxy/Proxy";
import { ParseKcsApi } from "@KC/Functions";
import KanColleStoreClient, { StoreInterface } from "@KC/Store/KanColleStoreClient";

import { kcsapi_port } from "@KC/Raw/kcsapi_port";
import { kcsapi_member_updatecomment } from "@KC/Raw/kcsapi_member";
import { kcsapi_require_info } from "@KC/Raw/kcsapi_require_info";
import { kcsapi_basic } from "@KC/Raw/kcsapi_basic";

@Component
export default class Admiral extends KanColleStoreClient {
	constructor() {
		super();

		Proxy.Instance.Register("/kcsapi/api_get_member/require_info", (req, resp) => {
			ParseKcsApi<kcsapi_require_info>(resp, (x) => this.Update(x.api_basic));
		});
		Proxy.Instance.Register("/kcsapi/api_get_member/basic", (req, resp) => {
			ParseKcsApi<kcsapi_basic>(resp, (x) => this.Update(x));
		});
		Proxy.Instance.Register("/kcsapi/api_port/port", (req, resp) => {
			ParseKcsApi<kcsapi_port>(resp, (x) => this.Update(x.api_basic));
		});
		Proxy.Instance.Register("/kcsapi/api_req_member/updatecomment", (req, resp) => {
			ParseKcsApi<unknown, kcsapi_member_updatecomment>(resp, req.body, (x, y) => {
				this.StoreAdmiralCommentUpdate(y.api_cmt);
			});
		});
	}

	private Update(data: kcsapi_basic) {
		this.StoreAdmiralUpdate({
			MemberId: parseInt(data.api_member_id, 10),
			Rank: data.api_rank,

			Level: data.api_level,
			Experience: data.api_experience,

			Nickname: data.api_nickname,
			Comment: data.api_comment,

			Sortie: {
				Wins: data.api_st_win,
				Loses: data.api_st_lose,
			},

			Maximum: {
				Ships: data.api_max_chara,
				Equips: data.api_max_slotitem,
			},
		});
	}
}
