import { kcsapi_mst_ship, kcsapi_mst_stype } from "./kcsapi_mst_ship";

export interface kcsapi_start2 {
    api_mst_ship: kcsapi_mst_ship[];
    api_mst_slotitem_equiptype: kcsapi_mst_slotitem_equiptype[];
    api_mst_stype: kcsapi_mst_stype[];
    api_mst_slotitem: kcsapi_mst_slotitem[];
    api_mst_useitem: kcsapi_mst_useitem[];
    api_mst_maparea: kcsapi_mst_maparea[];
    api_mst_mapinfo: kcsapi_mst_mapinfo[];
    api_mst_mission: kcsapi_mission[];
}