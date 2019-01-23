import { IIdentifiable } from "System/Base/Interfaces/IIdentifiable";
import { RawDataWrapper } from "System/Base/Wrapper";
import { kcsapi_mst_useitem } from "System/Interfaces/Master/kcsapi_mst_useitem";
import { UseItemType, UseItemCategory } from "System/Enums/UseItemEnums";

export class UseItemInfo extends RawDataWrapper<kcsapi_mst_useitem> implements IIdentifiable {
    public get Id(): number { return this.raw.api_id }
    public get Name(): string { return this.raw.api_name }

    public get Description(): string { return this.raw.api_description[0] }
    public get ExtraDescription(): string { return this.raw.api_description[1] }

    // Deprecated
    // public get Price(): number { return this.raw.api_price }

    public get ItemType(): UseItemType { return this.raw.api_usetype }
    public get Category(): UseItemCategory { return this.raw.api_category }

    constructor(api_data: kcsapi_mst_useitem) {
        super(api_data);
    }

}