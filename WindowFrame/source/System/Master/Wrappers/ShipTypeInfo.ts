import { RawDataWrapper } from "System/Base/Wrapper";
import { kcsapi_mst_stype } from "System/Interfaces/Master/kcsapi_mst_ship";
import { IIdentifiable } from "System/Base/Interfaces/IIdentifiable";

export class ShipTypeInfo extends RawDataWrapper<kcsapi_mst_stype> implements IIdentifiable {
    public get Id(): number { return this.raw.api_id }
    public get SortId(): number { return this.raw.api_sortno }
    public get Name(): string { return this.raw.api_name }

    public get RepairMultiply(): number { return this.raw.api_scnt }

    constructor(api_data: kcsapi_mst_stype) {
        super(api_data);
    }
}