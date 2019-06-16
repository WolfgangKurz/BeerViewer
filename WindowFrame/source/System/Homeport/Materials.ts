import { Observable } from "System/Base/Observable";
import { SubscribeKcsapi } from "System/Base/KcsApi";
import { kcsapi_port } from "System/Interfaces/kcsapi_port";
import { kcsapi_material } from "System/Interfaces/kcsapi_material";
import { kcsapi_charge } from "System/Interfaces/kcsapi_charge";
import { kcsapi_destroyship, kcsapi_req_kousyou_destroyship } from "System/Interfaces/kcsapi_ship";
import { kcsapi_destroyitem2 } from "System/Interfaces/kcsapi_item";
import { kcsapi_airbase_corps_supply, kcsapi_airbase_corps_set_plane, kcsapi_req_air_corps_set_plane } from "System/Interfaces/kcsapi_airbase_corps";
import { kcsapi_req_nyukyo_start, kcsapi_req_nyukyo_speedchange } from "System/Interfaces/kcsapi_repair";
import LogHelper from "System/Base/LogHelper";

export class Materials extends Observable {
	private _Fuel: number;
	private _Ammo: number;
	private _Steel: number;
	private _Bauxite: number;
	private _InstantConstruction: number;
	private _RepairBucket: number;
	private _DevelopmentMaterial: number;
	private _ImprovementMaterial: number;

	public get Fuel(): number { return this._Fuel }
	public get Ammo(): number { return this._Ammo }
	public get Steel(): number { return this._Steel }
	public get Bauxite(): number { return this._Bauxite }
	public get InstantConstruction(): number { return this._InstantConstruction }
	public get RepairBucket(): number { return this._RepairBucket }
	public get DevelopmentMaterial(): number { return this._DevelopmentMaterial }
	public get ImprovementMaterial(): number { return this._ImprovementMaterial }

	public get Materials(): number[] { return [this.Fuel, this.Ammo, this.Steel, this.Bauxite] }

	constructor() {
		super();
		this._Fuel = 0;
		this._Ammo = 0;
		this._Steel = 0;
		this._Bauxite = 0;
		this._InstantConstruction = 0;
		this._RepairBucket = 0;
		this._DevelopmentMaterial = 0;
		this._ImprovementMaterial = 0;

		SubscribeKcsapi<kcsapi_port>(
			"api_port/port",
			x => this.Update(x.api_material)
		);
		SubscribeKcsapi<kcsapi_material[]>(
			"api_get_member/material",
			x => this.Update(x)
		);
		SubscribeKcsapi<kcsapi_charge>(
			"api_req_hokyu/charge",
			x => {
				window.API.Log("Ships supplied, costs {0}.", LogHelper.MaterialDiff(this.Materials, x.api_material));
				this.Update(x.api_material);
			}
		);
		SubscribeKcsapi<kcsapi_destroyship, kcsapi_req_kousyou_destroyship>(
			"api_req_kousyou/destroyship",
			(x, y) => {
				window.API.Log("Ship {0} destroyed, resource {1} returned.", LogHelper.GetShipNamesFromList(y.api_ship_id.toString()), LogHelper.MaterialDiff(this.Materials, x.api_material));
				this.Update(x.api_material);
			}
		);
		SubscribeKcsapi<kcsapi_destroyitem2>(
			"api_req_kousyou/destroyitem2",
			x => this.Update(x.api_get_material)
		);

		// Bucket using
		SubscribeKcsapi<{}, kcsapi_req_nyukyo_start>(
			"api_req_nyukyo/start",
			(x, y) => y.api_highspeed === 1 ? this.$._RepairBucket-- : 0
		);
		SubscribeKcsapi<{}, kcsapi_req_nyukyo_speedchange>(
			"api_req_nyukyo/speedchange",
			(x, y) => this.$._RepairBucket--
		);

		// Supply Airbase
		SubscribeKcsapi<kcsapi_airbase_corps_supply>(
			"api_req_air_corps/supply",
			x => this.Update([
				x.api_after_fuel,
				this.Ammo,
				this.Steel,
				x.api_after_bauxite
			])
		);

		// Set aircraft to AirBase
		SubscribeKcsapi<kcsapi_airbase_corps_set_plane, kcsapi_req_air_corps_set_plane>(
			"api_req_air_corps/set_plane",
			(x, y) => {
				if (y.api_item_id === -1) return;
				if (x["api_plane_info"].length >= 2) return;
				this.Update([
					this.Fuel,
					this.Ammo,
					this.Steel,
					x.api_after_bauxite
				]);
			}
		);
	}

	public Update(source: number[] | kcsapi_material[]): void {
		if (!source || source.length == 0) return;
		const type = typeof (<kcsapi_material>source[0]).api_value;

		if (source.length >= 4 && type === "undefined") {
			const casted = source as number[];
			this.$._Fuel = casted[0];
			this.$._Ammo = casted[1];
			this.$._Steel = casted[2];
			this.$._Bauxite = casted[3];
		} else if (source.length >= 8 && type !== "undefined") {
			const casted = source as kcsapi_material[];
			this.$._Fuel = casted[0].api_value;
			this.$._Ammo = casted[1].api_value;
			this.$._Steel = casted[2].api_value;
			this.$._Bauxite = casted[3].api_value;
			this.$._InstantConstruction = casted[4].api_value;
			this.$._RepairBucket = casted[5].api_value;
			this.$._DevelopmentMaterial = casted[6].api_value;
			this.$._ImprovementMaterial = casted[7].api_value;
		}

		this.RaisePropertyChanged(nameof(this.Materials));
	}
}