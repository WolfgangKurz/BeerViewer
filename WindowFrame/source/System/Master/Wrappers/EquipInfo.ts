import { RawDataWrapper } from "../../Base/Wrapper";
import { kcsapi_mst_slotitem } from "../../Interfaces/Master/kcsapi_mst_slotitem";
import { IIdentifiable } from "../../Base/Interfaces/IIdentifiable";
import { EquipIcon, EquipCategory } from "../../Enums/EquipType";
import { Master } from "../Master";
import { EquipTypeInfo } from "./EquipTypeInfo";

export class EquipInfo extends RawDataWrapper<kcsapi_mst_slotitem> implements IIdentifiable {
    public get Id(): number { return this.raw.api_id }
    public get Name(): string { return this.raw.api_name }

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

    private _EquipType: EquipTypeInfo;
    public get EquipType(): EquipTypeInfo {
        return this._EquipType;
    }

    constructor(api_data: kcsapi_mst_slotitem) {
        super(api_data);

        this._EquipType = Master.Instance.EquipTypes!.get(this.Category) || EquipTypeInfo.Empty;
    }
}