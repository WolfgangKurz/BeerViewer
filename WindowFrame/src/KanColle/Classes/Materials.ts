import Vue from "vue";
import { State, Getter, Action, Mutation, namespace } from "vuex-class";
import Proxy from "@/Proxy/Proxy";

import { ParseKcsApi, } from "@KC/Functions";
import { kcsapi_port } from "@KC/Raw/kcsapi_port";
import { kcsapi_material } from "@KC/Raw/kcsapi_material";
import { kcsapi_charge } from "@KC/Raw/kcsapi_charge";
import { kcsapi_destroyship } from "@KC/Raw/kcsapi_ship";
import { kcsapi_destroyitem2 } from "../Raw/kcsapi_item";
import { kcsapi_req_nyukyo_start } from "../Raw/kcsapi_repair";
import { kcsapi_airbase_corps_supply, kcsapi_airbase_corps_set_plane, kcsapi_req_air_corps_set_plane } from "../Raw/kcsapi_airbase_corps";

export default class Materials extends Vue {
	@Getter
	private Materials!: {
		fuel: number;
		ammo: number;
		steel: number;
		bauxite: number;
		bucket: number;
		construction: number;
		development: number;
		improve: number;
	};

	@Mutation
	private UpdateMaterials!: (payload: { fuel: number, ammo: number, steel: number, bauxite: number }) => void;

	@Mutation
	private UpdateResources!: (payload: { bucket: number, construction: number, development: number, improve: number }) => void;

	constructor() {
		super();

		Proxy.Instance.Register("/kcsapi/api_port/port", (req, resp) => {
			ParseKcsApi<kcsapi_port, unknown>(resp, req.body, (x, y) => {
				console.log(y);
				this.Update(x.api_material);
			});
		});
		Proxy.Instance.Register("/kcsapi/api_get_member/material", (req, resp) => {
			ParseKcsApi<kcsapi_material[]>(resp, (x) => this.Update(x));
		});
		Proxy.Instance.Register("/kcsapi/api_req_hokyu/charge", (req, resp) => {
			ParseKcsApi<kcsapi_charge>(resp, (x) => this.Update(x.api_material));
		});
		Proxy.Instance.Register("/kcsapi/api_req_kousyou/destroyship", (req, resp) => {
			ParseKcsApi<kcsapi_destroyship>(resp, (x) => this.Update(x.api_material));
		});
		Proxy.Instance.Register("/kcsapi/api_req_hokyu/charge", (req, resp) => {
			ParseKcsApi<kcsapi_charge>(resp, (x) => this.Update(x.api_material));
		});
		Proxy.Instance.Register("/kcsapi/api_req_kousyou/destroyitem2", (req, resp) => {
			ParseKcsApi<kcsapi_destroyitem2>(resp, (x) => this.Update(x.api_get_material));
		});

		// Bucket using
		Proxy.Instance.Register("/kcsapi/api_req_nyukyo/start", (req, resp) => {
			ParseKcsApi<{}, kcsapi_req_nyukyo_start>(resp, (req as any).body as Buffer, (_, x) => (x.api_highspeed === 1) && this.DecreaseBucket());
		});
		Proxy.Instance.Register("/kcsapi/api_req_nyukyo/speedchange", (req, resp) => {
			ParseKcsApi(resp, () => this.DecreaseBucket());
		});

		// Supply Airbase
		Proxy.Instance.Register("/kcsapi/api_req_air_corps/supply", (req, resp) => {
			ParseKcsApi<kcsapi_airbase_corps_supply>(resp, (x) => this.Update([
				x.api_after_fuel,
				this.Materials.ammo,
				this.Materials.steel,
				x.api_after_bauxite
			]));
		});

		// Set aircraft to AirBase
		Proxy.Instance.Register("/kcsapi/api_req_air_corps/supply", (req, resp) => {
			ParseKcsApi<kcsapi_airbase_corps_set_plane, kcsapi_req_air_corps_set_plane>(resp, (req as any).body as Buffer, (x, y) => {
				if (y.api_item_id === -1) return;
				if (x.api_plane_info.length >= 2) return;
				this.Update([
					this.Materials.fuel,
					this.Materials.ammo,
					this.Materials.steel,
					x.api_after_bauxite
				]);
			});
		});
	}

	private DecreaseBucket() {
		this.UpdateResources({
			bucket: this.Materials.bucket - 1,
			construction: this.Materials.construction,
			development: this.Materials.development,
			improve: this.Materials.improve
		});
	}

	private Update(source: number[] | kcsapi_material[]): void {
		if (!source || source.length === 0) return;
		const type = typeof (source[0] as kcsapi_material).api_value;

		if (source.length >= 4 && type === "undefined") {
			const casted = source as [number, number, number, number];
			this.UpdateMaterials({
				fuel: casted[0],
				ammo: casted[1],
				steel: casted[2],
				bauxite: casted[3],
			});
		} else if (source.length >= 8 && type !== "undefined") {
			const casted = source as kcsapi_material[];
			this.UpdateMaterials({
				fuel: casted[0].api_value,
				ammo: casted[1].api_value,
				steel: casted[2].api_value,
				bauxite: casted[3].api_value,
			});
			this.UpdateResources({
				bucket: casted[4].api_value,
				construction: casted[5].api_value,
				development: casted[6].api_value,
				improve: casted[7].api_value,
			});
		}
	}
}
