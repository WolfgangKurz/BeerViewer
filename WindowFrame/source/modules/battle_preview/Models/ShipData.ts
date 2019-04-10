import { Observable } from "System/Base/Observable";
import { ShipSpeed, ShipType } from "System/Enums/ShipEnums";
import { GuageValue } from "System/Models/GuageValue";
import { EquipCategory } from "System/Enums/EquipEnums";
import { Ship } from "System/Homeport/Ship";
import { ShipInfo } from "System/Master/Wrappers/ShipInfo";
import ShipEquipData from "./ShipEquipData";

export default class ShipData extends Observable {
	protected _Id: number = 0;
	public get Id(): number { return this._Id }

	protected _MasterId: number = 0;
	public get MasterId(): number { return this._MasterId }

	protected _Name: string = "";
	public get Name(): string { return this._Name }

	protected _AdditionalName: string = "";
	public get AdditionalName(): string { return this._AdditionalName }

	protected _ShipSpeed: ShipSpeed = ShipSpeed.None;
	public get ShipSpeed(): ShipSpeed { return this._ShipSpeed }

	protected _ShipType: ShipType = ShipType.None;
	public get ShipType(): ShipType { return this._ShipType }

	protected _TypeName: string = "";
	public get TypeName(): string { return this._TypeName }

	protected _Level: number = 0;
	public get Level(): number { return this._Level }

	protected _State: Ship.State = Ship.State.None;
	public get State(): Ship.State { return this._State }

	protected _HP: GuageValue = new GuageValue();
	public get HP(): GuageValue { return this._HP }
	public set HP(value: GuageValue) {
		this.$._HP = value;

		if (this.IsHeavilyDamaged)
			this.$._State |= Ship.State.HeavilyDamaged;
		else
			this.$._State &= ~Ship.State.HeavilyDamaged;
	}

	protected _BeforeHP: number = 0;
	public get BeforeHP(): number { return this._BeforeHP }

	protected _Firepower: number = 0;
	public get Firepower(): number { return this._Firepower }

	protected _Torpedo: number = 0;
	public get Torpedo(): number { return this._Torpedo }

	protected _AA: number = 0;
	public get AA(): number { return this._AA }

	protected _Armor: number = 0;
	public get Armor(): number { return this._Armor }

	protected _Luck: number = 0;
	public get Luck(): number { return this._Luck }

	protected _ASW: number = 0;
	public get ASW(): number { return this._ASW }

	protected _Evation: number = 0;
	public get Evation(): number { return this._Evation }

	protected _Equips: ShipEquipData[] = [];
	public get Equips(): ShipEquipData[] { return this._Equips }

	protected _ExtraEquip?: ShipEquipData;
	public get ExtraEquip(): ShipEquipData | undefined { return this._ExtraEquip }

	protected _IsUsedDamecon: boolean = false;
	public get IsUsedDamecon(): boolean { return this._IsUsedDamecon }

	protected _Condition: number = 0;
	public get Condition(): number { return this._Condition }

	protected _IsMvp: boolean = false;
	public get IsMvp(): boolean { return this._IsMvp }

	//#region Equipment stats Sum
	public get EquipFirePower(): number { return this.Equips.reduce((a, c) => a + c.Firepower, 0) + (this.ExtraEquip ? this.ExtraEquip.Firepower : 0) }
	public get EquipTorpedo(): number { return this.Equips.reduce((a, c) => a + c.Torpedo, 0) + (this.ExtraEquip ? this.ExtraEquip.Torpedo : 0) }
	public get EquipAA(): number { return this.Equips.reduce((a, c) => a + c.AA, 0) + (this.ExtraEquip ? this.ExtraEquip.AA : 0) }
	public get EquipArmor(): number { return this.Equips.reduce((a, c) => a + c.Armor, 0) + (this.ExtraEquip ? this.ExtraEquip.Armor : 0) }
	public get EquipASW(): number { return this.Equips.reduce((a, c) => a + c.ASW, 0) + (this.ExtraEquip ? this.ExtraEquip.ASW : 0) }
	public get EquipAccuracy(): number { return this.Equips.reduce((a, c) => a + c.Accuracy, 0) + (this.ExtraEquip ? this.ExtraEquip.Accuracy : 0) }
	public get EquipEvation(): number { return this.Equips.reduce((a, c) => a + c.Evation, 0) + (this.ExtraEquip ? this.ExtraEquip.Evation : 0) }
	//#endregion

	//#region Final stats
	public get TotalFirePower(): number { return 0 < this.Firepower ? this.Firepower + this.EquipFirePower : this.Firepower }
	public get TotalTorpedo(): number { return 0 < this.Torpedo ? this.Torpedo + this.EquipTorpedo : this.Torpedo }
	public get TotalAA(): number { return 0 < this.AA ? this.AA + this.EquipAA : this.AA }
	public get TotalArmor(): number { return 0 < this.Armor ? this.Armor + this.EquipArmor : this.Armor }
	public get TotalASW(): number { return this.ASW + this.EquipASW }
	public get TotalEvation(): number { return this.Evation }
	//#endregion

	// Evation is already Total value
	public get ShipEvation(): number { return this.Evation - this.EquipEvation }

	constructor() {
		super();

		this.$._Name = "???";
		this.$._AdditionalName = "";
		this.$._TypeName = "???";
		this.$._ShipType = 0;
		this.$._State = Ship.State.None;
		this.$._Equips = [];
		this.$._ShipSpeed = ShipSpeed.Immovable;
	}

	public Update({ Level, FirePower, Torpedo, AA, Armor, Equips }: { Level: number; FirePower: number; Torpedo: number; AA: Number; Armor: number; Equips: ShipEquipData[]; }): void {

	}

	public Count(type2: EquipCategory): number {
		return this.Equips.reduce((a, c) => a + (c.CategoryType == type2 ? 1 : 0), 0)
			+ (this.ExtraEquip && this.ExtraEquip.CategoryType == type2 ? 1 : 0);
	}

	public HasScout(): boolean {
		return this.Equips
			.filter(x => x.Source && (x.Source.Category === EquipCategory.SeaplaneBomber || x.Source.Category === EquipCategory.SeaplaneReconnaissance))
			.some(x => x.Current > 0);
	}

	public get IsHeavilyDamaged(): boolean { return (this.HP.Current / this.HP.Maximum) <= 0.25 }
}

export class MembersShipData extends ShipData {
	constructor();
	constructor(ship: Ship);
	constructor(ship?: Ship) {
		super();
		if (!ship) return;

		this.$._Id = ship.Id;
		this.$._MasterId = ship.Info.Id;

		this.$._Name = ship.Info.Name;
		this.$._Level = ship.Level;
		this.$._State = ship.State;

		this.$._HP = new GuageValue(ship.HP.Current, ship.HP.Maximum);

		this.$._ShipSpeed = ship.Speed;
		this.$._ShipType = ship.Info.ShipType ? ship.Info.ShipType.Id : 0;

		this.$._Equips = ship.Equips
			.filter(s => s != null && s.Equipped)
			.map(s => new ShipEquipData(s));
		this.$._ExtraEquip =
			ship.ExtraEquip && ship.ExtraEquip.Equipped
				? new ShipEquipData(ship.ExtraEquip)
				: undefined;

		this.$._Condition = ship.Condition;

		this.$._Firepower = ship.FirePower.Current;
		this.$._Torpedo = ship.Torpedo.Current;
		this.$._AA = ship.AA.Current;
		this.$._Armor = ship.Armor.Current;
		this.$._Luck = ship.Luck.Current;
		this.$._ASW = ship.ASW.Current;
		this.$._Evation = ship.raw.api_kaihi[0];
	}
}
export class MastersShipData extends ShipData {
	constructor();
	constructor(info: ShipInfo);
	constructor(info?: ShipInfo) {
		super();
		if (!info) return;

		this.$._Id = info.Id;
		this.$._Name = info.Name;
		this.$._AdditionalName = info && info.Id > 1500 ? info.raw.api_yomi : "";

		this.$._Condition = -1;

		this.$._ShipSpeed = info ? info.Speed : ShipSpeed.Immovable;
	}
}