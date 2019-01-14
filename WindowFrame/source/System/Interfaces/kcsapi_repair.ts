import { HTTPRequest } from "../Exports/API";

export interface kcsapi_nyukyo_start extends HTTPRequest {
    api_ship_id: number;
}
export interface kcsapi_nyukyo_speedchange extends HTTPRequest {
    api_ndock_id: number;
    api_highspeed: number;
}