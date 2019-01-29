import { HTTPRequest } from "System/Exports/API";

export interface kcsapi_req_nyukyo_start extends HTTPRequest {
	api_ship_id: number;
}
export interface kcsapi_req_nyukyo_speedchange extends HTTPRequest {
	api_ndock_id: number;
	api_highspeed: number;
}