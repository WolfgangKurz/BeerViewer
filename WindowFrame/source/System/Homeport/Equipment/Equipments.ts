import { Observable } from "../../Base/Observable";
import { Equipment } from "./Equipment";
import { IdentifiableTable } from "../../Models/TableWrapper";
import { Ship } from "../Ship";

export class Equipments extends Observable {
    //#region Equips
    private _Equips: IdentifiableTable<Equipment> = new IdentifiableTable<Equipment>();
    public get Equips(): IdentifiableTable<Equipment> { return this._Equips }
    //#endregion

    constructor() {
        super();
    }

    public RemoveAllFromShip(ship: Ship): void {

    }
}