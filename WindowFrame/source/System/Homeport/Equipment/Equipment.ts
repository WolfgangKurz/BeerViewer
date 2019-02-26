import { RawDataWrapper } from "System/Base/Wrapper";
import { kcsapi_slotitem } from "System/Interfaces/kcsapi_item";
import { IIdentifiable } from "System/Base/Interfaces/IIdentifiable";
import { Master } from "System/Master/Master";
import { EquipInfo } from "System/Master/Wrappers/EquipInfo";

export class Equipment extends RawDataWrapper<kcsapi_slotitem> implements IIdentifiable {
	public get Id(): number { return this.raw.api_id }

	public readonly Info: EquipInfo;

	public get Level(): number { return this.raw.api_level }
	public get LevelText(): string {
		if (this.Level <= 0) return "";
		return this.Level >= 10 ? "★max" : `★+${this.Level}`;
	}

	public get Proficiency(): number { return this.raw.api_alv || 0 }
	public get ProficiencyText(): string {
		switch (this.Proficiency) {
			case 1: return "|";
			case 2: return "||";
			case 3: return "|||";
			case 4: return "/";
			case 5: return "//";
			case 6: return "///";
			case 7: return ">>";
		}
		return "";
	}

	constructor(api_data: kcsapi_slotitem) {
		super(api_data);

		if (this.raw.api_slotitem_id >= 0 && Master.Instance.Equips!.has(this.raw.api_slotitem_id))
			this.Info = Master.Instance.Equips!.get(this.raw.api_slotitem_id) || EquipInfo.Empty;
		else
			this.Info = EquipInfo.Empty;
	}

	public Remodel(level: number, MasterId: number): Equipment {
		const raw = Object.assign({}, this.raw);
		raw.api_level = level;
		raw.api_slotitem_id = MasterId;
		return new Equipment(raw);
	}

	public ToString(): string {
		return `{"Id": ${this.Id}, "Name": "${this.Info.Name}", "Level": ${this.Level}, "Proficiency": ${this.Proficiency}}`;
	}

	public static readonly Empty: Equipment = new Equipment({
		api_id: 0,
		api_alv: 0,
		api_level: 0,
		api_locked: 0,
		api_slotitem_id: -1
	});
}