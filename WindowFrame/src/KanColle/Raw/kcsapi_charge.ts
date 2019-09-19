// tslint:disable:class-name
export interface kcsapi_charge {
	api_ship: kcsapi_charge_ship[];
	api_material: number[];
	api_use_bou: number;
}
export interface kcsapi_charge_ship {
	api_id: number;
	api_fuel: number;
	api_bull: number;
	api_onslot: number[];
}
