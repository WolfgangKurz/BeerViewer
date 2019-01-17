import { EquipBigCategory, EquipDictCategory, EquipCategory, EquipIcon, EquipAircraft } from "../../Enums/EquipEnums";

/** Master information interface for Equipment */
export interface kcsapi_mst_slotitem {
    /** Internal equip id */
    api_id: number;

    /** Internal equip id for sort */
    api_sortno: number;

    /** Original name to show */
    api_name: string;

    /** Equipment type.
     * 
     * `[BigCategory, DictionaryCategory, Category, Icon, AircraftCategory]`
     */
    api_type: [EquipBigCategory, EquipDictCategory, EquipCategory, EquipIcon, EquipAircraft];

    /** HP - Taikyuu */
    api_taik: number;

    /** Armor - Soukou */
    api_souk: number;

    /** Fire Power - Hougeki */
    api_houg: number;

    /** Torpedo Power - Raigeki */
    api_raig: number;

    /** Speed - Sokudo */
    api_soku: number;

    /** Bombing - Bakugeki */
    api_baku: number;

    /** Anti-Aircraft Power - Taikuu
     * 
     * `api_taik` already exists, so Tykuu */
    api_tyku: number;

    /** Anti Submarine Warfare - Taisen */
    api_tais: number;

    /** Unknown */
    api_atap: number;

    /**
     * Accuracy - unknown
     * 
     * Anti-Bombing for Land-Based Fighter/Interceptor
     */
    api_houm: number;

    /** Torpedo Accuracy - Raigeki Meichuu */
    api_raim: number;

    /**
     * Evation - unknown
     * 
     * Intercept for Land-Based Fighter/Interceptor
     */
    api_houk: number;

    /** Unknown */
    api_raik: number;

    /** Unknown */
    api_bakk: number;

    /** Length of Sight / Reconnaissance - Sakuteki */
    api_saku: number;

    /** Unknown */
    api_sakb: number;

    /** Luck */
    api_luck: number;

    /** Length of fire */
    api_leng: number;

    /** Rarity of equipment */
    api_rare: number;

    /** Earnable resources when destroy ship
     * 
     * `[Fuel, Ammo, Steel, Bauxite]`
     */
    api_broken: number[];

    /** Dictionary text */
    api_info: string;

    /** Unused */
    api_usebull: string;

    /** Cost when insert aircraft to LBAS slot */
    api_cost: number;

    /** Distance when use on LBAS */
    api_distance: number;
}

/** Master information interface for equipment type */
export interface kcsapi_mst_slotitem_equiptype {
    /** Internal id of type */
    api_id: number;

    /** Name */
    api_name: string;

    /** Unknown */
    api_show_flg: number;
}