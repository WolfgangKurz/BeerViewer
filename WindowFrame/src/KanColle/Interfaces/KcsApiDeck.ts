import IKcsApi from "./IKcsApi";
import { kcsapi_deck } from "@KC/Raw/kcsapi_deck";
import { kcsapi_ship2 } from "../Raw/kcsapi_ship";

export interface KcsApiDeck extends IKcsApi {
	api_deck_data: kcsapi_deck[];
}

export interface KcsApiShipDeck extends IKcsApi {
	api_deck_data: kcsapi_ship2[];
}
