import { Observable } from "System/Base/Observable";
import { Ship } from "System/Homeport/Ship";
import { Formation, FleetType } from "../Enums/Battle";
import ShipData from "./ShipData";

export default class FleetData extends Observable {
	private _FleetType: FleetType = FleetType.None;
	public get FleetType(): FleetType { return this._FleetType }

	private _Name: string = "";
	public get Name(): string { return this._Name }

	private _IsCritical: boolean = false;
	public get IsCritical(): boolean { return this._IsCritical }

	private _Ships: ShipData[] = [];
	public get Ships(): ShipData[] { return this._Ships }

	private _Formation: Formation = Formation.None;
	public get Formation(): Formation { return this._Formation }

	constructor();
	constructor(ships: ShipData[], formation: Formation, name: string, type: FleetType);
	constructor(ships?: ShipData[], formation?: Formation, name?: string, type?: FleetType) {
		super();

		if (!ships) ships = [];
		if (!formation) formation = Formation.None;
		if (!name) name = "";
		if (!type) type = FleetType.EnemyFirst;

		this.$._Ships = ships;
		this.$._Formation = formation;
		this.$._Name = name;
		this.$._FleetType = type;

		if (type === FleetType.EnemyFirst || type === FleetType.EnemySecond) return;
		this.$._IsCritical =
			this.Ships
				? this.Ships.some(x => (x.HP.Current / x.HP.Maximum <= 0.25) && (x.HP.Current > 0))
				: false;
	}
	public UpdateFormation(formation: Formation): void {
		this.$._Formation = formation;
	}
	public CriticalCheck(ExceptFlagship: boolean = false): boolean {
		const y = ExceptFlagship ? this.Ships.filter((_, x) => x > 0) : this.Ships;

		return y
			.filter(x => (x.State & Ship.State.DamageControlled) === 0)
			.filter(x => (x.State & Ship.State.Evacuation) === 0)
			.filter(x => (x.State & Ship.State.Tow) === 0)
			.some(x => (x.State & Ship.State.HeavilyDamaged) === 0);
	}
}
