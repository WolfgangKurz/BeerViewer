import { Observable } from "System/Base/Observable";
import { ShipInfo } from "System/Master/Wrappers/ShipInfo";
import { Ship } from "System/Homeport/Ship";
import { Equipment } from "./Equipment";
import { AirSupremacy } from "System/Models/AirSupremacy";
import { EquipCategory } from "System/Enums/EquipEnums";
import { Proficiency, AirSupremacyOption } from "System/Models/Proficiency";

export class ShipEquip extends Observable {
    public readonly Owner: ShipInfo;
    public readonly Item: Equipment;

    public readonly MaximumAircraft: number;

    //#region CurrentAircraft
    private _CurrentAircraft: number;
    public get CurrentAircraft(): number { return this._CurrentAircraft }
    //#endregion

    //#region LostAircraft
    private _LostAircraft: number;
    public get LostAircraft(): number { return this._LostAircraft }
    //#endregion

    public get IsAircraft(): boolean { return this.Item.Info.IsNumerable }

    public get Equipped(): boolean { return this.Item != null && this.Item != Equipment.Empty }


    private readonly proficiencies: { [key: number]: Proficiency } = {
        0: new Proficiency(0, 9, 0, 0),
        1: new Proficiency(10, 24, 0, 0),
        2: new Proficiency(25, 39, 2, 1),
        3: new Proficiency(40, 54, 5, 1),
        4: new Proficiency(55, 69, 9, 1),
        5: new Proficiency(70, 84, 14, 3),
        6: new Proficiency(85, 99, 14, 3),
        7: new Proficiency(100, 120, 22, 6)
    };
    public get AirSupremacy(): AirSupremacy {
        let calculator = (value: ShipEquip, option: AirSupremacyOption): number => 0;
        const proficiency = this.proficiencies[Math.max(Math.min(this.Item.Proficiency, 7), 0)];

        switch (this.Item.Info.Category) {
            case EquipCategory.CarrierBasedFighter:
            case EquipCategory.SeaplaneFighter:
                calculator = (value: ShipEquip, option: AirSupremacyOption): number =>
                    (this.Item.Info.AA + this.Item.Level * 0.2) * Math.sqrt(this.CurrentAircraft)
                    + Math.sqrt(proficiency.GetInternalValue(option) / 10.0) + proficiency.FighterBonus;
                break;

            case EquipCategory.CarrierBasedTorpedoBomber:
            case EquipCategory.CarrierBasedDiveBomber:
                calculator = (value: ShipEquip, option: AirSupremacyOption): number =>
                    (this.Item.Info.AA + this.Item.Level * 0.25) * Math.sqrt(this.CurrentAircraft)
                    + Math.sqrt(proficiency.GetInternalValue(option) / 10.0);
                break;

            case EquipCategory.SeaplaneBomber:
                calculator = (value: ShipEquip, option: AirSupremacyOption): number =>
                    this.Item.Info.AA * Math.sqrt(this.CurrentAircraft)
                    + Math.sqrt(proficiency.GetInternalValue(option) / 10.0) + proficiency.SeaplaneBomberBonus;
                break;

            case EquipCategory.JetPoweredFighterBomber:
                calculator = (value: ShipEquip, option: AirSupremacyOption): number =>
                    this.Item.Info.AA * Math.sqrt(this.CurrentAircraft)
                    + Math.sqrt(proficiency.GetInternalValue(option) / 10.0);
                break;

            default:
                return new AirSupremacy();
        }
        return new AirSupremacy(
            calculator(this, AirSupremacyOption.MinimumValue),
            calculator(this, AirSupremacyOption.MaximumValue)
        );
    }

    constructor(Owner: Ship | undefined | null, Item: Equipment | undefined | null, MaximumAircraft: number, CurrentAircraft: number) {
        super();

        this.Owner = (Owner && Owner.Info) || ShipInfo.Empty;
        this.Item = Item || Equipment.Empty;

        this.MaximumAircraft = MaximumAircraft;
        this._CurrentAircraft = CurrentAircraft;
        this._LostAircraft = MaximumAircraft - CurrentAircraft;
    }

    public UpdateAircrafts(current: number): void {
        this.$._CurrentAircraft = current;
        this.$._LostAircraft = this.MaximumAircraft - current;
    }
}