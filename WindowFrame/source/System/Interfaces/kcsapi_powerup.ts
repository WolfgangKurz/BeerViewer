import { HTTPRequest } from "../Exports/API.class";
import { kcsapi_ship2 } from "./kcsapi_ship";
import { kcsapi_deck } from "./kcsapi_deck";

export interface kcsapi_powerup {
    api_powerup_flag: number;
    api_ship: kcsapi_ship2;
    api_deck: kcsapi_deck[];
}

export interface kcsapi_req_kaisou_powerup extends HTTPRequest {
    api_id_items: string;
}