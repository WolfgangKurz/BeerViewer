import { RawDataWrapper } from "System/Base/Wrapper";
import { kcsapi_mst_mission } from "System/Interfaces/kcsapi_mission";
import { IIdentifiable } from "System/Base/Interfaces/IIdentifiable";

export class ExpeditionInfo extends RawDataWrapper<kcsapi_mst_mission> implements IIdentifiable {
	public get Id(): number { return this.raw.api_id }
	public get DisplayNo(): string { return this.raw.api_disp_no }
	public get Title(): string { return this.raw.api_name }
	public get Detail(): string { return this.raw.api_details }

	constructor(api_data: kcsapi_mst_mission) {
		super(api_data);
	}
}