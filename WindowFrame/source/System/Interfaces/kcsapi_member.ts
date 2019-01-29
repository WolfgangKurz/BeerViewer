import { HTTPRequest } from "System/Exports/API";

export interface kcsapi_member_updatecomment extends HTTPRequest {
	api_cmt: string;
}