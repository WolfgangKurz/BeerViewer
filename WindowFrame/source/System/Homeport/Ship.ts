import { Observable } from "../Base/Observable";
import { GuageValue } from "../Models/GuageValue";
import { IIdentifiable } from "../Base/Interfaces/IIdentifiable";
import { ShipInfo } from "../Master/Wrappers/ShipInfo";
import { Homeport } from "./Homeport";
import { kcsapi_ship2 } from "../Interfaces/kcsapi_ship";
import { Master } from "../Master/Master";
import { ShipSpeed } from "../Enums/ShipEnums";
import { AirSupremacy } from "../Models/AirSupremacy";
import { ShipEquip } from "./Equipment/ShipEquip";

export class Ship extends Observable implements IIdentifiable {
    public readonly Id: number = 0;

    public Info!: ShipInfo;

    public HP: GuageValue = new GuageValue();

    public Fuel: GuageValue = new GuageValue();
    public Ammo: GuageValue = new GuageValue();

    public State: Ship.State = Ship.State.None;
    public Condition: number = 0;

    public Speed: ShipSpeed = ShipSpeed.None;

    public AirSupremacy: AirSupremacy = new AirSupremacy();
    public LoS: number = 0;

    public EquippedItems: ShipEquip[] = [];

    constructor(homeport: Homeport, api_data: kcsapi_ship2) {
        super();
        this.Update(api_data);
    }

    private Update(api_data: kcsapi_ship2): void {
        this.Info = Master.Instance.Ships!.get(api_data.api_ship_id) || ShipInfo.Empty;
    }

    public Repair() {

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