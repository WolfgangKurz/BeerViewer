import { Observable } from "../../Base/Observable";
import { ShipInfo } from "../../Master/Wrappers/ShipInfo";
import { Ship } from "../Ship";
import { Equipment } from "./Equipment";

export class ShipEquip extends Observable {
    public readonly Owner: ShipInfo;
    public readonly Item: Equipment;

    public readonly MaximumAircraft: number;

    //#region CurrentAircraft
    private _CurrentAircraft:number;
    public get CurrentAircraft(): number { return this._CurrentAircraft }
    //#endregion

    //#region LostAircraft
    private _LostAircraft: number;
    public get LostAircraft(): number { return this._LostAircraft }
    //#endregion

    public get IsAircraft(): boolean { return this.Item.Info.IsNumerable }

    public get Equipped(): boolean { return this.Item != null && this.Item != Equipment.Empty }

    constructor(Owner: Ship | undefined | null, Item: Equipment | undefined | null, MaximumAircraft: number, CurrentAircraft: number) {
        super();

        this.Owner = (Owner && Owner.Info) || ShipInfo.Empty;
        this.Item = Item || Equipment.Empty;

        this.MaximumAircraft = MaximumAircraft;
        this._CurrentAircraft = CurrentAircraft;
        this._LostAircraft = MaximumAircraft - CurrentAircraft;
    }

    public UpdateAircrafts(current:number):void{
        this._CurrentAircraft = current;
        this._LostAircraft = this.MaximumAircraft - current;
    }
}