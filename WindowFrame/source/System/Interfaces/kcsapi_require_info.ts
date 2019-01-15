import { kcsapi_basic } from "./kcsapi_basic";
import { kcsapi_kdock } from "./kcsapi_dock";
import { kcsapi_slotitem, kcsapi_useitem } from "./kcsapi_item";

export interface kcsapi_require_info {
    api_basic: kcsapi_basic;
    api_slot_item: kcsapi_slotitem[];
    api_kdock: kcsapi_kdock[];
    api_useitem: kcsapi_useitem[];
}