// tslint:disable:class-name
import { kcsapi_material } from "@KC/Raw/kcsapi_material";
import { kcsapi_deck } from "@KC/Raw/kcsapi_deck";
import { kcsapi_ndock } from "@KC/Raw/kcsapi_dock";
import { kcsapi_basic } from "@KC/Raw/kcsapi_basic";
import { kcsapi_ship2 } from "@KC/Raw/kcsapi_ship";

export interface kcsapi_port {
	api_material: kcsapi_material[];
	api_deck_port: kcsapi_deck[];
	api_ndock: kcsapi_ndock[];
	api_ship: kcsapi_ship2[];
	api_basic: kcsapi_basic;
	api_combined_flag: number;
}
