import { kcsapi_mission_result, kcsapi_mission_result_item } from "System/Interfaces/kcsapi_mission_result";
import { Master } from "System/Master/Master";
import { ExpeditionResultItemKind } from "System/Enums/ExpeditionEnums";

export class ExpeditionResult {
	public Result: number;

	public Fuel: number;
	public Ammo: number;
	public Steel: number;
	public Bauxite: number;

	public Items: ExpeditionResultItem[];

	constructor(mission: kcsapi_mission_result) {
		this.Result = mission.api_clear_result;

		this.Fuel = mission.api_get_material[0];
		this.Ammo = mission.api_get_material[1];
		this.Steel = mission.api_get_material[2];
		this.Bauxite = mission.api_get_material[3];

		var list: ExpeditionResultItem[] = [];
		if (mission.api_get_item1 != null)
			list.push(new ExpeditionResultItem(mission.api_get_item1, mission.api_useitem_flag[0]));
		if (mission.api_get_item2 != null)
			list.push(new ExpeditionResultItem(mission.api_get_item2, mission.api_useitem_flag[1]));

		this.Items = list;
	}
}

export class ExpeditionResultItem {
	private readonly source: kcsapi_mission_result_item;
	public readonly Kind: ExpeditionResultItemKind;

	public get Id(): number { return this.source.api_useitem_id }

	/** Original name of item (Account item/Use item)
	 * 
	 * **※ Need to `i18n` at front-end**
	 */
	public get Name(): string {
		switch (this.Kind) {
			case ExpeditionResultItemKind.InstantRepairBucket:
				return "高速修復材";
			case ExpeditionResultItemKind.InstantConstructionMaterial:
				return "高速建造材";
			case ExpeditionResultItemKind.DevelopmentMaterial:
				return "開発資材";
			case ExpeditionResultItemKind.FurnitureCoin:
				return "家具コイン";

			case ExpeditionResultItemKind.BasedOnId:
				const item = Master.Instance.UseItems!.get(this.Id);
				return (item && item.Name) || "???";

			default:
				return "???";
		}
	}
	public get Count(): number { return this.source.api_useitem_count }

	constructor(item: kcsapi_mission_result_item, kind: ExpeditionResultItemKind) {
		this.source = item;
		this.Kind = kind;
	}
}