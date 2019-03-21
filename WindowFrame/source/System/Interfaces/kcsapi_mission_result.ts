import { ExpeditionResultType, ExpeditionResultItemKind } from "System/Enums/ExpeditionEnums";

export interface kcsapi_mission_result {
	api_ship_id: number[];
	api_clear_result: ExpeditionResultType;
	api_get_exp: number;
	api_member_lv: number;
	api_member_exp: number;
	api_get_ship_exp: number[];
	api_get_exp_lvup: number[][];
	api_maparea_name: string;
	api_detail: string;
	api_quest_name: string;
	api_quest_level: number;
	api_get_material: [number, number, number, number];
	api_useitem_flag: [ExpeditionResultItemKind, ExpeditionResultItemKind];

	api_get_item1: kcsapi_mission_result_item | null;
	api_get_item2: kcsapi_mission_result_item | null;
}

export interface kcsapi_mission_result_item {
	api_useitem_id: number;
	api_useitem_name: string;
	api_useitem_count: number;
}