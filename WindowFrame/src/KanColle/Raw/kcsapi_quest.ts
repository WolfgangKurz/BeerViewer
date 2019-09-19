// tslint:disable:class-name

export interface kcsapi_quest {
	api_no: number;
	api_category: number;
	api_type: number;
	api_state: number;
	api_title: string;
	api_detail: string;
	api_get_material: number[];
	api_bonus_flag: number;
	api_progress_flag: number;
}
export interface kcsapi_questlist {
	api_count: number;
	api_page_count: number;
	api_disp_page: number;
	api_list: kcsapi_quest[];
	api_exec_count: number;
}
export interface kcsapi_req_quest_requests {
	api_quest_id: number;
}
