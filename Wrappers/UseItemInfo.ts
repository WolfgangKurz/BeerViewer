import IIdentifiable from "../Interfaces/IIdentifiable"
import KanColleWrapperBase from "../Classes/KanColleWrapperBase"

import { kcsapi_mst_useitem } from "../Raw/Master/kcsapi_mst_useitem"
import { UseItemType, UseItemCategory } from "../Enums/UseItemEnums"

export class UseItemInfo extends KanColleWrapperBase<kcsapi_mst_useitem> implements IIdentifiable {
	public get Id(): number { return this.RawData.api_id; }
	public get Name(): string { return this.RawData.api_name; }

	public get Description(): string { return this.RawData.api_description[0]; }
	public get ExtraDescription(): string { return this.RawData.api_description[1]; }

	// Deprecated
	// public get Price(): number { return this.RawData.api_price }

	public get ItemType(): UseItemType { return this.RawData.api_usetype; }
	public get Category(): UseItemCategory { return this.RawData.api_category; }

	constructor(apiData: kcsapi_mst_useitem) {
		super(apiData);
	}
}
