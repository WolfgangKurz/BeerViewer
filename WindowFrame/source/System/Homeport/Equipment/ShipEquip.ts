import { Observable } from "../../Base/Observable";
import { ShipInfo } from "../../Master/Wrappers/ShipInfo";
import { Ship } from "../Ship";
import { Equipment } from "./Equipment";

export class ShipEquip extends Observable {
    public readonly Owner: ShipInfo;
    public readonly Item: Equipment;

    public readonly MaximumAircraft: number;
    public readonly CurrentAircraft: number;
    public readonly LostAircraft: number;

    public get IsAircraft(): boolean { return this.Item.Info.Type.IsNumerable() }

    public get Tooltip(): string { return this.Item.Info.ToolTipData }
    public get Equipped(): boolean { return this.Item != null && this.Item != Equipment.Empty }

    constructor(Owner: Ship | undefined | null, Item: Equipment | undefined | null, MaximumAircraft: number, CurrentAircraft: number) {
        super();

        this.Owner = (Owner && Owner.Info) || ShipInfo.Empty;
        this.Item = Item || Equipment.Empty;

        this.MaximumAircraft = MaximumAircraft;
        this.CurrentAircraft = CurrentAircraft;
        this.LostAircraft = MaximumAircraft - CurrentAircraft;
    }
}