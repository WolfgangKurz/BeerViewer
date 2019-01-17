/** Master information interface for Expedition */
interface kcsapi_mission {
    /** Internal expedition id */
    api_id: number;

    /** Id when display to screen */
    api_disp_no: string;

    /** World tab id */
    api_maparea_id: number;

    /** Expedition name */
    api_name: string;

    /** Expedition description */
    api_details: string;

    /** Seconds to complete */
    api_time: number;

    /** Difficulty, display as `E` ~ `S` */
    api_difficulty: number;

    /** Lost fuels ratio after return (0.0 ~ 1.0) */
    api_use_fuel: number;

    /** Lost ammos ratio after return (0.0 ~ 1.0) */
    api_use_bull: number;

    /** Earnable item `[Id, Count]` */
    api_win_item1: [number, number];

    /** Earnable item `[Id, Count]` */
    api_win_item2: [number, number];

    /** Can return after start? */
    api_return_flag: number;
}
export { // Alias
    kcsapi_mission as kcsapi_mission,
    kcsapi_mission as kcsapi_mst_mission
}