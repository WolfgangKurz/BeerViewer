import { RawDataWrapper } from "System/Base/Wrapper";
import { kcsapi_mst_maparea, kcsapi_mst_mapinfo } from "System/Interfaces/Master/kcsapi_map";
import { IIdentifiable } from "System/Base/Interfaces/IIdentifiable";

export class MapWorldInfo extends RawDataWrapper<kcsapi_mst_maparea> implements IIdentifiable {
    public get Id(): number { return this.raw.api_id }
    public get Name(): string { return this.raw.api_name }

    public get IsEventmap(): boolean { return this.raw.api_type === 1 }

    constructor(api_data: kcsapi_mst_maparea) {
        super(api_data);
    }
}
export class MapAreaInfo extends RawDataWrapper<kcsapi_mst_mapinfo> implements IIdentifiable {
    public get Id(): number { return this.raw.api_id }
    public get Name(): string { return this.raw.api_name }

    public get WorldId(): number { return this.raw.api_maparea_id }
    public get AreaId(): number { return this.raw.api_no }

    public get Level(): number { return this.raw.api_level }

    public get OperationName(): string { return this.raw.api_opetext }
    public get OperationDescription(): string { return this.raw.api_infotext }

    /** [HP, Count] */
    public get Gauge(): [number, number] {
        return [
            this.raw.api_max_maphp || 0,
            this.raw.api_required_defeat_count || 0
        ];
    }

    constructor(api_data: kcsapi_mst_mapinfo) {
        super(api_data);
    }
}