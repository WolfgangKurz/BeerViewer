import { GuageValue } from "../Models/GuageValue";
import { IIdentifiable } from "../Base/Interfaces/IIdentifiable";
import { ShipInfo } from "../Master/Wrappers/ShipInfo";
import { Homeport } from "./Homeport";
import { kcsapi_ship2 } from "../Interfaces/kcsapi_ship";
import { Master } from "../Master/Master";
import { ShipSpeed } from "../Enums/ShipEnums";
import { AirSupremacy } from "../Models/AirSupremacy";
import { ShipEquip } from "./Equipment/ShipEquip";
import { ObservableDataWrapper } from "../Base/Wrapper";
import { EquipCategory } from "../Enums/EquipEnums";

export class Ship extends ObservableDataWrapper<kcsapi_ship2> implements IIdentifiable {
    public readonly Id: number = 0;

    public Info!: ShipInfo;

    public get Level(): number { return this.raw.api_lv }

    public get HP(): GuageValue { return new GuageValue(this.raw.api_nowhp, this.raw.api_maxhp) }

    private _Fuel: GuageValue = new GuageValue();
    public get Fuel(): GuageValue { return this._Fuel }

    private _Ammo: GuageValue = new GuageValue();
    public get Ammo(): GuageValue { return this._Ammo }

    public get UsedFuel(): number { return (this.Fuel.Maximum - this.Fuel.Current) * (this.Level > 99 ? 0.85 : 1.0) }
    public get UsedAmmo(): number { return (this.Ammo.Maximum - this.Ammo.Current) * (this.Level > 99 ? 0.85 : 1.0) }
    public get UsedBauxite(): number { return this.EquippedItems.reduce((a, c) => a + c.LostAircraft, 0) * 5 }

    private _State: Ship.State = Ship.State.None;
    public get State(): Ship.State { return this._State }
    public Condition: number = 0;

    public Speed: ShipSpeed = ShipSpeed.None;

    public AirSupremacy: AirSupremacy = new AirSupremacy();
    public LoS: number = 0;


    //#region Equips Property
    private _Equips: ShipEquip[] = [];
    public get Equips(): ShipEquip[] { return this._Equips }
    //#endregion

    //#region ExtraEquip Property
    private _ExtraEquip: ShipEquip | null = null;
    public get ExtraEquip(): ShipEquip | null { return this._ExtraEquip }
    //#endregion

    public get EquippedItems(): ShipEquip[] {
        return this.Equips
            .concat(this.ExtraEquip ? [this.ExtraEquip] : [])
            .filter(x => x.Equipped);
    }


    private homeport: Homeport;
    constructor(homeport: Homeport, api_data: kcsapi_ship2) {
        super(api_data);

        this.homeport = homeport;
        this.Update(api_data);
    }

    public Update(api_data: kcsapi_ship2): void {
        this.Info = Master.Instance.Ships!.get(api_data.api_ship_id) || ShipInfo.Empty;
    }

    public Repair(): void {
        this._State &= ~Ship.State.Repairing;
        this.raw.api_nowhp = this.HP.Maximum;
    }
    public Repairing(): void {
        this._State |= Ship.State.Repairing;
    }

    public Supply(fuel: number, ammo: number, aircrafts: number[]) {
        this._Fuel = this.Fuel.Update(fuel);
        this._Ammo = this.Ammo.Update(ammo);

        for (let i = 0; i < this.Equips.length; i++)
            this.Equips[i].UpdateAircrafts(aircrafts[i] || 0);
    }

    public UpdateEquipSlots(equipSlots: number[]): void {
        this._Equips = this.raw.api_slot
            .map(id => this.homeport.Equipments.Equips.get(id)).filter(x => x)
            .map((t, i) => new ShipEquip(this, t, this.Info.raw.api_maxeq[i] || 0, this.raw.api_onslot[i] || 0));

        this._ExtraEquip = new ShipEquip(this, this.homeport.Equipments.Equips.get(this.raw.api_slot_ex), 0, 0);

        if (this.Equips.some(x => x.Item.Info.Category === EquipCategory.DamageController))
            this._State |= Ship.State.DamageControlled;
        else
            this._State &= ~Ship.State.DamageControlled;

        this.UpdateASW();

        this.RaisePropertyChanged(nameof(this.UsedBauxite));
    }

    private UpdateASW(): void {

    }

    public Evacuate(): void {
        this._State |= Ship.State.Evacuation;
    }
    public Tow(): void {
        this._State |= Ship.State.Tow;
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