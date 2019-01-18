import { HTTPRequest } from "../Exports/API";

export interface kcsapi_deck {
    api_member_id: number;
    api_id: number;
    api_name: string;
    api_name_id: string;
    api_mission: [number, number, number, number];
    api_flagship: string;
    api_ship: number[];
}

export interface kcsapi_req_member_updatedeckname extends HTTPRequest {
    api_deck_id: number;
    api_name: string;
}

export interface kcsapi_req_hensei_combined extends HTTPRequest {
    api_combined_type: number;
}