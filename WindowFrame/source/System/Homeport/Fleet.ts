import { IIdentifiable } from "../Base/Interfaces/IIdentifiable";
import { Homeport } from "./Homeport";
import { kcsapi_deck } from "../Interfaces/kcsapi_deck";
import { TickObservable } from "../Base/Observable";
import { Ship } from "./Ship";
import { ShipSpeed, ShipType } from "../Enums/ShipEnums";
import Const from "../Const/System";
import { FleetState } from "../Enums/FleetEnums";

export class Fleet extends TickObservable implements IIdentifiable {
    public Id!: number;
    public Name!: string;
    public Ships!: Ship[];

    public get FleetSpeed(): ShipSpeed {
        return this.Ships.length > 0
            ? this.Ships.reduce((a, c) => Math.min(a, c.Speed), ShipSpeed.Fastest)
            : ShipSpeed.None;
    }

    public State: FleetState = FleetState.Empty;

    public IsSailing: boolean = false;

    public ConditionRestoreTime: number = 0;
    public get IsConditionRestoring(): boolean { return this.ConditionRestoreTime > 0 }

    private homeport: Homeport;

    constructor(owner: Homeport, data: kcsapi_deck) {
        super();

        this.homeport = owner;
        this.ManagedDisposable.Add(this.Expedition = new Expedition(this));

        this.Update(data);
    }

    private Update(data: kcsapi_deck): void {
        this.Id = data.api_id;
        this.Name = data.api_name;

        this.Expedition.Update(data.api_mission);
        this.UpdateShips(data.api_ship.map(x => this.homeport.Ships[x]));

        this.UpdateCondition();
    }

    private PrevCondition!: number;
    private UpdateCondition(): void {
        const condition: number = this.Ships.reduce((a, c) => a === 0 ? c.Condition : Math.min(a, c.Condition), 0);
        const goal = Const.ConditionRestorationLimit;

        // Require time not changed
        if (this.PrevCondition === condition) return;

        this.PrevCondition = condition;
        if (condition >= goal)
            this.ConditionRestoreTime = 0;
        else {
            let restore = Date.now();

            const value = Math.floor((goal - condition + 2) / 3) * 3; // Integer dividing
            restore = restore + value * 60 * 1000;

            this.ConditionRestoreTime = restore;
        }
        this.RaisePropertyChanged(nameof(this.IsConditionRestoring));
        this.UpdateState();
    }

    private UpdateState(): void {
        let state = FleetState.Empty;

        const ships = this.Ships;
        if (ships.length > 0) {
            if (this.IsSailing)
                state |= FleetState.Sailing;
            else if (this.Expedition.IsInExecution)
                state |= FleetState.Expedition;
            else
                state |= FleetState.Homeport;
        }

        if ((state & FleetState.Homeport) !== 0) {
            // Check repairing
            if (ships.filter(x => this.homeport.RepairDock!.CheckRepairing(x.Id)).length > 0)
                state |= FleetState.Repairing;

            // Not fully supplied
            if (ships.filter(x => x.Fuel.Current < x.Fuel.Maximum || x.Ammo.Current < x.Ammo.Maximum).length > 0)
                state |= FleetState.NeedSupply;

            // Condition restoring
            if (this.IsConditionRestoring)
                state |= FleetState.ConditionRestoring;
        }

        const heavilyDamaged: boolean = ships
            .filter(x => !this.homeport.RepairDock!.CheckRepairing(x.Id))
            .filter(x => !(x.State & Ship.State.Evacuation) && !(x.State & Ship.State.Tow))
            .filter(x => !(state & FleetState.Sailing) && (x.State & Ship.State.DamageControlled))
            .filter(x => x.HP.Percentage <= 0.25)
            .length > 0;
        if (heavilyDamaged)
            state |= FleetState.HeavilyDamaged;

        if (ships.length > 0) {
            if (ships[0].Info.ShipType && (ships[0].Info.ShipType.Id == ShipType.RepairShip))
                state |= FleetState.FlagshipRepairShip;
        }

        this.State = state;
    }

    protected Tick(): void {
        super.Tick();

    }
}