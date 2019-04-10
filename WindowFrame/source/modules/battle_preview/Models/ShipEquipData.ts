import { Observable } from "System/Base/Observable";
import { EquipInfo } from "System/Master/Wrappers/EquipInfo";
import { EquipCategory } from "System/Enums/EquipEnums";
import { ShipEquip } from "System/Homeport/Equipment/ShipEquip";

export default class ShipEquipData extends Observable {
	private _Source: EquipInfo | null = null;
	public get Source(): EquipInfo | null { return this._Source }

	public get Equipped(): boolean { return this.Source !== null }

	private _Maximum: number = 0;
	public get Maximum(): number { return this._Maximum }

	private _Current: number = 0;
	public get Current(): number { return this._Current }

	public get Lost(): number { return this.Maximum - this.Current }

	private _Level: number = 0;
	public get Level(): number { return this._Level }

	private _Proficiency: number = 0;
	public get Proficiency(): number { return this._Proficiency }

	public get Firepower(): number { return this.Source ? this.Source.FirePower : 0 }
	public get Torpedo(): number { return this.Source ? this.Source.Torpedo : 0 }
	public get AA(): number { return this.Source ? this.Source.AA : 0 }
	public get Armor(): number { return this.Source ? this.Source.Armor : 0 }
	public get Bomb(): number { return this.Source ? this.Source.Bombing : 0 }
	public get ASW(): number { return this.Source ? this.Source.ASW : 0 }
	public get Accuracy(): number { return this.Source ? this.Source.Accuracy : 0 }
	public get Evation(): number { return this.Source ? this.Source.Evation : 0 }
	public get LoS(): number { return this.Source ? this.Source.LoS : 0 }

	public get CategoryType(): EquipCategory { return this.Source ? this.Source.Category : EquipCategory.None }

	constructor(equip: ShipEquip);
	constructor(item: EquipInfo | null, maximum?: number, current?: number, level?: number, proficiency?: number);
	constructor(equip: EquipInfo | ShipEquip | null, maximum?: number, current?: number, level?: number, proficiency?: number) {
		super();

		if (equip && "Item" in equip) {
			const _equip = <ShipEquip>equip;

			equip = _equip.Item ? _equip.Item.Info : null;
			maximum = _equip.MaximumAircraft;
			current = _equip.CurrentAircraft;
			level = _equip.Item ? _equip.Item.Level : 0;
			proficiency = _equip.Item ? _equip.Item.Proficiency : 0;
		}
		this.$._Source = equip;
		this.$._Maximum = maximum || -1;
		this.$._Current = current || -1;
		this.$._Level = level || 0;
		this.$._Proficiency = proficiency || 0;
	}
}