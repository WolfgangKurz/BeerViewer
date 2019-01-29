import { Observable } from "System/Base/Observable";
import { IdentifiableTable } from "System/Models/TableWrapper";
import { Ship } from "System/Homeport/Ship";
import { Equipment } from "./Equipment";

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

	public Update(arg: any): void {

	}
}