import { GuageValue } from "System/Models/GuageValue";
import { IIdentifiable } from "System/Base/Interfaces/IIdentifiable";
import { ShipInfo } from "System/Master/Wrappers/ShipInfo";
import { Homeport } from "./Homeport";
import { kcsapi_ship2 } from "System/Interfaces/kcsapi_ship";
import { Master } from "System/Master/Master";
import { ShipSpeed } from "System/Enums/ShipEnums";
import { AirSupremacy } from "System/Models/AirSupremacy";
import { ShipEquip } from "./Equipment/ShipEquip";
import { ObservableDataWrapper } from "System/Base/Wrapper";
import { EquipCategory } from "System/Enums/EquipEnums";
import { UpgradeStatus } from "System/Models/UpgradeStatus";

export class Ship extends ObservableDataWrapper<kcsapi_ship2> implements IIdentifiable {
	public get Id(): number { return this.raw.api_id }

	private _Info!: ShipInfo;
	public get Info(): ShipInfo { return this._Info }

	public get Level(): number { return this.raw.api_lv }
	public get Experience(): number { return this.raw.api_exp[0] }
	public get ExperienceRemain(): number { return this.raw.api_exp[1] }
	public get ExperienceNext(): number { return this.ExperienceRemain + this.Experience }

	public get HP(): GuageValue { return new GuageValue(this.raw.api_nowhp, this.raw.api_maxhp) }

	private _Fuel: GuageValue = new GuageValue();
	public get Fuel(): GuageValue { return this._Fuel }

	private _Ammo: GuageValue = new GuageValue();
	public get Ammo(): GuageValue { return this._Ammo }

	public get UsedFuel(): number { return (this.Fuel.Maximum - this.Fuel.Current) * (this.Level > 99 ? 0.85 : 1.0) }
	public get UsedAmmo(): number { return (this.Ammo.Maximum - this.Ammo.Current) * (this.Level > 99 ? 0.85 : 1.0) }
	public get UsedBauxite(): number { return this.EquippedItems.reduce((a, c) => a + c.LostAircraft, 0) * 5 }

	//#region State Property
	private _State: Ship.State = Ship.State.None;
	public get State(): Ship.State { return this._State }
	//#endregion

	public get Condition(): number { return this.raw.api_cond }

	public get Speed(): ShipSpeed { return this.raw.api_soku }

	public get AirSupremacy(): AirSupremacy { return AirSupremacy.Sum(this.EquippedItems.map(x => x.AirSupremacy)) }
	public get LoS(): number { return this.raw.api_sakuteki[0] }


	//#region Equips Property
	private _Equips: ShipEquip[] = [];
	public get Equips(): ShipEquip[] { return this._Equips }
	//#endregion

	//#region ExtraEquip Property
	private _ExtraEquip: ShipEquip | null = null;
	public get ExtraEquip(): ShipEquip | null { return this._ExtraEquip }
	//#endregion

	/** Equipped equipment items includes Extra slot */
	public get EquippedItems(): ShipEquip[] {
		return this.Equips
			.concat(this.ExtraEquip ? [this.ExtraEquip] : [])
			.filter(x => x.Equipped);
	}

	//#region FirePower
	private _FirePower: UpgradeStatus = new UpgradeStatus();
	public get FirePower(): UpgradeStatus { return this._FirePower }
	//#endregion

	//#region Torpedo
	private _Torpedo: UpgradeStatus = new UpgradeStatus();
	public get Torpedo(): UpgradeStatus { return this._Torpedo }
	//#endregion

	//#region AA
	private _AA: UpgradeStatus = new UpgradeStatus();
	public get AA(): UpgradeStatus { return this._AA }
	//#endregion

	//#region Armor
	private _Armor: UpgradeStatus = new UpgradeStatus();
	public get Armor(): UpgradeStatus { return this._Armor }
	//#endregion

	//#region Luck
	private _Luck: UpgradeStatus = new UpgradeStatus();
	public get Luck(): UpgradeStatus { return this._Luck }
	//#endregion

	//#region ASW
	private _ASW: UpgradeStatus = new UpgradeStatus();
	public get ASW(): UpgradeStatus { return this._ASW }
	//#endregion


	private homeport: Homeport;
	constructor(homeport: Homeport, api_data: kcsapi_ship2) {
		super(api_data);

		this.homeport = homeport;
		this.Update(api_data);
	}

	public Repair(): void {
		this.$._State &= ~Ship.State.Repairing;
		this.raw.api_nowhp = this.HP.Maximum;
		this.RaisePropertyChanged(nameof(this.HP));

		this.raw.api_cond = Math.max(40, this.Condition);
		this.RaisePropertyChanged(nameof(this.Condition));
	}
	public Repairing(): void {
		this.$._State |= Ship.State.Repairing;
	}

	public Supply(fuel: number, ammo: number, aircrafts: number[]) {
		this.$._Fuel = this.Fuel.Update(fuel);
		this.$._Ammo = this.Ammo.Update(ammo);

		for (let i = 0; i < this.Equips.length; i++)
			this.Equips[i].UpdateAircrafts(aircrafts[i] || 0);
	}

	public Update(api_data: kcsapi_ship2): void {
		this.UpdateData(api_data);
		this.RaisePropertyChanged(nameof(this.Condition));

		this.$._Info = Master.Instance.Ships!.get(api_data.api_ship_id) || ShipInfo.Empty;
		this.RaisePropertyChanged(nameof(this.Id));
		this.RaisePropertyChanged(nameof(this.HP));

		this.$._Fuel = new GuageValue(this.raw.api_fuel, this.Info.Fuel, 0);
		this.$._Ammo = new GuageValue(this.raw.api_bull, this.Info.Ammo, 0);

		this.$._ASW = new UpgradeStatus(0, this.raw.api_taisen[1], this.raw.api_taisen[0]);

		if (this.raw.api_kyouka.length >= 5) {
			this.$._FirePower = new UpgradeStatus(this.Info.FirePower, this.raw.api_kyouka[0]);
			this.$._Torpedo = new UpgradeStatus(this.Info.Torpedo, this.raw.api_kyouka[1]);
			this.$._AA = new UpgradeStatus(this.Info.AA, this.raw.api_kyouka[2]);
			this.$._Armor = new UpgradeStatus(this.Info.Armor, this.raw.api_kyouka[3]);
			this.$._Luck = new UpgradeStatus(this.Info.Luck, this.raw.api_kyouka[4]);
		}
		this.UpdateEquipSlots();
	}

	public UpdateEquipSlots(): void {
		this.$._Equips = this.raw.api_slot
			.map(id => this.homeport.Equipments.Equips.get(id)).filter(x => x)
			.map((t, i) => new ShipEquip(this, t, this._Info.raw.api_maxeq[i] || 0, this.raw.api_onslot[i] || 0));

		this.$._ExtraEquip = new ShipEquip(this, this.homeport.Equipments.Equips.get(this.raw.api_slot_ex), 0, 0);

		if (this.Equips.some(x => x.Item.Info.Category === EquipCategory.DamageController))
			this.$._State |= Ship.State.DamageControlled;
		else
			this.$._State &= ~Ship.State.DamageControlled;

		this.UpdateASW();

		this.RaisePropertyChanged(nameof(this.UsedBauxite));
		this.RaisePropertyChanged(nameof(this.AirSupremacy));
	}

	private UpdateASW(): void {

	}

	public Evacuate(): void {
		this.$._State |= Ship.State.Evacuation;
	}
	public Tow(): void {
		this.$._State |= Ship.State.Tow;
	}
}
export namespace Ship {
	export enum State {
		/** Nothing special */
		None = 0,

		/** Repairing */
		Repairing = 1,

		/** Damaged ship */
		Evacuation = 1 << 1,

		/** Escort ship */
		Tow = 1 << 2,

		/** Damaged, not evacuated */
		HeavilyDamaged = 1 << 3,

		/** Damaged, and Damage control used */
		DamageControlled = 1 << 4
	}
}