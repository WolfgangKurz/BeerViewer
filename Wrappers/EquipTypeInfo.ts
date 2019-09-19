import { IIdentifiable } from "System/Base/Interfaces/IIdentifiable";
import { KanColleWrapperBase } from "System/Base/Wrapper";
import { kcsapi_mst_slotitem_equiptype } from "System/Interfaces/Master/kcsapi_mst_slotitem";

export class EquipTypeInfo extends KanColleWrapperBase<kcsapi_mst_slotitem_equiptype> implements IIdentifiable {
	public get Id(): number { return this.raw.api_id }
	public get Name(): string { return this.raw.api_name }

	constructor(api_data: kcsapi_mst_slotitem_equiptype) {
		super(api_data);
	}

	public toString(): string {
		return `{"Id": ${this.Id}, "Name": "${this.Name}"}`;
	}

	public static readonly Empty: EquipTypeInfo = new EquipTypeInfo({
		api_id: 0,
		api_name: "???",
		api_show_flg: 0
	});
}