// tslint:disable:class-name
import { kcsapi_basic } from "@KC/Raw/kcsapi_basic";
import { kcsapi_kdock } from "@KC/Raw/kcsapi_dock";
import { kcsapi_slotitem, kcsapi_useitem } from "@KC/Raw/kcsapi_item";

export interface kcsapi_require_info {
	api_basic: kcsapi_basic;
	api_slot_item: kcsapi_slotitem[];
	api_kdock: kcsapi_kdock[];
	api_useitem: kcsapi_useitem[];
}
