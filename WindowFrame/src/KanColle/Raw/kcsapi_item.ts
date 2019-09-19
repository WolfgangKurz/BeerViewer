// tslint:disable:class-name

export interface kcsapi_slotitem {
	api_id: number;
	api_slotitem_id: number;
	api_level: number;
	api_locked: number;
	api_alv: number;
}
export interface kcsapi_createitem {
	api_id: number;
	api_slotitem_id: number;
	api_create_flag: number;
	api_shizai_flag: number;
	api_slot_item: kcsapi_slotitem;
	api_material: number[];
	api_type3: number;
	api_unsetslot: number[];
	api_fdata: string;
}
export interface kcsapi_destroyitem2 {
	api_get_material: number[];
}
export interface kcsapi_remodel_slot {
	api_remodel_flag: number;
	api_remodel_id: number[];
	api_after_material: number[];
	api_voice_ship_id: number;
	api_voice_id: number;
	api_after_slot: kcsapi_remodel_after_slot;
	api_use_slot_id: number[];
}
export interface kcsapi_remodel_after_slot {
	api_id: number;
	api_slotitem_id: number;
	api_locked: number;
	api_level: number;
}

export interface kcsapi_req_destroyitem2 {
	api_slotitem_ids: string;
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
