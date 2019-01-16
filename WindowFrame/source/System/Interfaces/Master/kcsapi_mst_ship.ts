import { ShipType, ShipSpeed } from "../../Enums/ShipEnums";

/** Master information interface for Ship */
export interface kcsapi_mst_ship {
    /** Internal ship id */
    api_id: number;

    /** Internal ship id for sort */
    api_sortno: number;

    /** Original name to show */
    api_name: string;

    /** Phonetic name of original name */
    api_yomi: string;

    /** Ship type */
    api_stype: ShipType | number;

    /** Next remodelable level */
    api_afterlv: number;

    /** Internal ship id after remodel */
    api_aftershipid: string;

    /** HP - Taikyuu */
    api_taik: [number, number];

    /** Armor - Soukou */
    api_souk: [number, number];

    /** Fire Power - Hougeki */
    api_houg: [number, number];

    /** Torpedo Power - Raigeki */
    api_raig: [number, number];

    /** Anti-Aircraft Power - Taikuu
     * 
     * `api_taik` already exists, so Tykuu */
    api_tyku: [number, number];

    /** Luck */
    api_luck: [number, number];

    /** Speed of ship - Sokudo */
    api_soku: ShipSpeed;

    /** Length of fire */
    api_leng: number;

    /** Equip slot count */
    api_slot_num: number;

    /** Maximum aircraft numbers for each equip slot */
    api_maxeq: number[];

    /** Require times for build, as seconds */
    api_buildtime: number;

    /** Earnable resources when destroy ship
     * 
     * `[Fuel, Ammo, Steel, Bauxite]`
     */
    api_broken: [number, number, number, number];

    /** Earnable stats when power-up ship used this ship
     * 
     * `[FirePower, TorpedoPower, AntiAircraft, Armor]`
     */
    api_powup: [number, number, number, number];

    /** Rarity of ship */
    api_backs: number;

    /** Message when get this ship */
    api_getmes: string;

    /** Require fuels when remodel */
    api_afterfuel: number;

    /** Require ammos when remodel */
    api_afterbull: number;

    /** Maximum fuels ship haves */
    api_fuel_max: number;

    /** Maximum ammos ship haves */
    api_bull_max: number;

    /** Voice flags as `bit`
     * 
     * - 1 : Idle voice exists
     * - 2 : O-clock voice exists
     * - 4 : Special idle voice exists
     *    * Idle voice when Condition >= 50 (Sparkle)
     */
    api_voicef: number;
}

/** Master information interface for ship type */
export interface kcsapi_mst_stype {
    /** Internal ship type id, see `ShipType` enum */
    api_id: number;

    /** Internal ship type id for sort
     * 
     * For example, BBs/FBBs will be placed before DDs.
     */
    api_sortno: number;

    /** Name */
    api_name: string;

    /** Repair multiply value.
     * 
     * Repair time follows below formula:
     * - lv ≤ 11, `(lv * 10) * MULTIPLY * LossHP + 30` secs
     * - lv ≥ 12, `(lv * 5 + MAGIC) * MULTIPLY * LossHP + 30` secs
     * 
     * `MAGIC` follows below formula:
     * - `MAGIC = ⌊√(Level - 11)⌋ * 10 + 50`, `⌊ ⌋` is floor function.
     */
    api_scnt: number;

    /** Silhouette type on construction dock */
    api_kcnt: number;
}