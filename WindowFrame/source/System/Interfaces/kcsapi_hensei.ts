import { HTTPRequest } from "System/Exports/API";

export interface kcsapi_hensei_combined {
    api_combined: number;
}
export interface kcsapi_req_hensei_change extends HTTPRequest {
    api_id: number;
    api_ship_idx: number;
    api_ship_id: number;
}