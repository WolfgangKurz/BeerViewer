// tslint:disable:class-name
import { kcsapi_ship2 } from "@KC/Raw/kcsapi_ship";
import { kcsapi_slotitem } from "@KC/Raw/kcsapi_item";

export interface kcsapi_ndock {
	api_member_id: number;
	api_id: number;
	api_state: number;
	api_ship_id: number;
	api_complete_time: number;
	api_complete_time_str: string;
	api_item1: number;
	api_item2: number;
	api_item3: number;
	api_item4: number;
}
export interface kcsapi_kdock {
	api_member_id: number;
	api_id: number;
	api_state: number;
	api_created_ship_id: number;
	api_complete_time: number;
	api_complete_time_str: string;
	api_item1: number;
	api_item2: number;
	api_item3: number;
	api_item4: number;
	api_item5: number;
}
export interface kcsapi_kdock_getship {
	api_id: number;
	api_ship_id: number;
	api_kdock: kcsapi_kdock[];
	api_ship: kcsapi_ship2;
	api_slotitem: kcsapi_slotitem[];
}
export interface kcsapi_req_kousyou_createship_speedchange {
	api_kdock_id: number;
	api_highspeed: number;
}
