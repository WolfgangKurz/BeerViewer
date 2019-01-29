export interface kcsapi_destroyitem2 {
	api_get_material: number[];
}
export interface kcsapi_slotitem {
	api_id: number;
	api_slotitem_id: number;
	api_level: number;
	api_locked: number;
	api_alv: number;
}
export interface kcsapi_useitem {
	api_member_id: number;
	api_id: number;
	api_value: number;
	api_usetype: number;
	api_category: number;
	api_name: string;
	api_description: string[];
	api_price: number;
	api_count: number;
}
