import { RawDataWrapper } from "System/Base/Wrapper";
import { kcsapi_mst_slotitem } from "System/Interfaces/Master/kcsapi_mst_slotitem";
import { IIdentifiable } from "System/Base/Interfaces/IIdentifiable";
import { EquipIcon, EquipCategory, EquipDictCategory } from "System/Enums/EquipEnums";
import { Master } from "System/Master/Master";
import { EquipTypeInfo } from "./EquipTypeInfo";

export class EquipInfo extends RawDataWrapper<kcsapi_mst_slotitem> implements IIdentifiable {
	public get Id(): number { return this.raw.api_id }
	public get Name(): string { return this.raw.api_name }

	public get Type(): EquipDictCategory { return this.raw.api_type[1] || EquipDictCategory.None }
	public get Category(): EquipCategory { return this.raw.api_type[2] || EquipCategory.None }
	public get Icon(): EquipIcon { return this.raw.api_type[3] || EquipIcon.None }

	public get FirePower(): number { return this.raw.api_houg }
	public get Torpedo(): number { return this.raw.api_raig }
	public get AA(): number { return this.raw.api_tyku }
	public get Bombing(): number { return this.raw.api_baku }
	public get ASW(): number { return this.raw.api_tais }
	public get Armor(): number { return this.raw.api_souk }
	public get Accuracy(): number { return this.raw.api_houm }
	public get Evation(): number { return this.raw.api_houk }
	public get LoS(): number { return this.raw.api_saku }

	/** Is Aircraft? */
	public get IsNumerable(): boolean {
		switch (this.Category) {
			case EquipCategory.CarrierBasedRecon:
			case EquipCategory.CarrierBasedRecon_II:
			case EquipCategory.CarrierBasedFighter:
			case EquipCategory.CarrierBasedTorpedoBomber:
			case EquipCategory.CarrierBasedDiveBomber:
			case EquipCategory.SeaplaneRecon:
			case EquipCategory.SeaplaneBomber:
			case EquipCategory.SeaplaneFighter:
			case EquipCategory.Autogyro:
			case EquipCategory.AntiSubmarinePatrolAircraft:
			case EquipCategory.LargeFlyingBoat:
			case EquipCategory.LandBasedAttackAircraft:
			case EquipCategory.InterceptorFighter:
			case EquipCategory.JetPoweredFighter:
			case EquipCategory.JetPoweredFighterBomber:
			case EquipCategory.JetPoweredAttacker:
			case EquipCategory.JetPoweredRecon:
				return true;

			default:
				return false;
		}
	}

	public get Encounterable(): boolean[] {
		const first = this.Category === EquipCategory.CarrierRecon || this.Category === EquipCategory.SeaplaneRecon;
		return [first, first || this.Category === EquipCategory.CarrierBasedTorpedoBomber];
	}

	public readonly EquipType: EquipTypeInfo;

	constructor(api_data: kcsapi_mst_slotitem) {
		super(api_data);

		if (this.Category === -1)
			this.EquipType = EquipTypeInfo.Empty;
		else
			this.EquipType = Master.Instance.EquipTypes!.get(this.Category) || EquipTypeInfo.Empty;
	}

	public static readonly Empty: EquipInfo = new EquipInfo({
		api_id: -1,
		api_sortno: 0,
		api_name: "???",
		api_type: [-1, -1, -1, -1, -1],
		api_taik: 0,
		api_souk: 0,
		api_houg: 0,
		api_raig: 0,
		api_soku: 0,
		api_baku: 0,
		api_tyku: 0,
		api_tais: 0,
		api_atap: 0,
		api_houm: 0,
		api_raim: 0,
		api_houk: 0,
		api_raik: 0,
		api_bakk: 0,
		api_saku: 0,
		api_sakb: 0,
		api_luck: 0,
		api_leng: 0,
		api_rare: 0,
		api_broken: [0, 0, 0, 0],
		api_info: "",
		api_usebull: "",
		api_cost: 0,
		api_distance: 0,
	});
}