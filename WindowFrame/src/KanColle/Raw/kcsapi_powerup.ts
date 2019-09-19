// tslint:disable:class-name
import { kcsapi_ship2 } from "@KC/Raw/kcsapi_ship";
import { kcsapi_deck } from "@KC/Raw/kcsapi_deck";

export interface kcsapi_powerup {
	api_powerup_flag: number;
	api_ship: kcsapi_ship2;
	api_deck: kcsapi_deck[];
}

export interface kcsapi_req_kaisou_powerup {
	api_id_items: string;
}
