export interface kcsapi_ship2 {
    api_id: number;
    api_sortno: number;
    api_ship_id: number;
    api_lv: number;
    api_exp: number[];
    api_nowhp: number;
    api_maxhp: number;
    api_soku: number;
    api_leng: number;
    api_slot: number[];
    api_onslot: number[];
    api_kyouka: number[];
    api_backs: number;
    api_fuel: number;
    api_bull: number;
    api_slotnum: number;
    api_ndock_time: number;
    api_ndock_item: number[];
    api_srate: number;
    api_cond: number;
    api_karyoku: number[];
    api_raisou: number[];
    api_taiku: number[];
    api_soukou: number[];
    api_kaihi: number[];
    api_taisen: number[];
    api_sakuteki: number[];
    api_lucky: number[];
    api_locked: number;
    api_locked_equip: number;
    api_sally_area: number;
    api_slot_ex: number;
}

export interface kcsapi_destroyship {
    api_material: number[];
}