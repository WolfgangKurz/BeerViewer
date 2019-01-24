import { IIdentifiable } from "System/Base/Interfaces/IIdentifiable";
import { Homeport } from "./Homeport";
import { kcsapi_deck, kcsapi_req_member_updatedeckname } from "System/Interfaces/kcsapi_deck";
import { TickObservable } from "System/Base/Observable";
import { Ship } from "./Ship";
import { ShipSpeed, ShipType } from "System/Enums/ShipEnums";
import Const from "System/Const/System";
import { FleetState } from "System/Enums/FleetEnums";
import { Expedition } from "./Expedition/Expedition";
import { AirSupremacy } from "System/Models/AirSupremacy";
import { LoSCalculator } from "System/Models/LoSCalculator/LoSCalculator";
import { SubscribeKcsapi } from "System/Base/KcsApi";
import { HTTPRequest } from "System/Exports/API";
import { Settings } from "System/Settings";

export class Fleet extends TickObservable implements IIdentifiable {
    //#region Id
    private _Id: number = 0;
    public get Id(): number { return this._Id }
    //#endregion

    //#region Name
    private _Name: string = "???";
    public get Name(): string { return this._Name }
    //#endregion

    //#region Ships
    private SourceShips: (Ship | null)[] = [];
    private _Ships: Ship[] = [];
    public get Ships(): Ship[] { return this._Ships }
    //#endregion

    public readonly Expedition: Expedition;

    public get FleetSpeed(): ShipSpeed {
        return this.Ships.length > 0
            ? this.Ships.reduce((a, c) => Math.min(a, c.Speed), ShipSpeed.Fastest)
            : ShipSpeed.None;
    }

    //#region State
    private _State: FleetState = FleetState.Empty;
    public get State(): FleetState { return this._State }
    //#endregion

    //#region IsSailing
    private _IsSailing: boolean = false;
    public get IsSailing(): boolean { return this._IsSailing }
    //#endregion

    //#region ConditionRestoreTime, IsConditionRestoring
    private _ConditionRestoreTime: number = 0;
    public get ConditionRestoreTime(): number { return this._ConditionRestoreTime }
    public get IsConditionRestoring(): boolean { return this.ConditionRestoreTime > 0 }
    //#endregion

    //#region AirSupremacy
    private _AirSupremacy: AirSupremacy = new AirSupremacy();
    public get AirSupremacy(): AirSupremacy { return this._AirSupremacy }
    //#endregion

    //#region LoS
    private _LoS: number = 0;
    public get LoS(): number { return this._LoS }
    ////#endregion


    private homeport: Homeport;
    constructor(owner: Homeport, data: kcsapi_deck) {
        super();

        this.homeport = owner;
        this.ManagedDisposable.Add(this.Expedition = new Expedition(this));
        this.ManagedDisposable.Add(SubscribeKcsapi<{}, kcsapi_req_member_updatedeckname>(
            "api_req_member/updatedeckname", (x, y) => this.UpdateFleetName(y))
        );

        this.Update(data);
    }

    private UpdateFleetName(data: kcsapi_req_member_updatedeckname): void {
        const fleetId = data.api_deck_id;;
        if (this.Id !== fleetId) return;

        this.$._Name = data.api_name.toString(); // Could be numeric name like "1234"
    }

    public Update(data: kcsapi_deck): void {
        this.$._Id = data.api_id;
        this.$._Name = data.api_name;

        this.Expedition.Update(data.api_mission);
        this.UpdateShips(
            (<Ship[]>data.api_ship.map(x => this.homeport.Ships.get(x)).filter(x => x))
        );

        this.UpdateCondition();
    }

    private UpdateShips(ships: Ship[]): void {
        this.SourceShips = ships;
        this.$._Ships = ships.filter(x => x !== null);

        this.Calculate();
        this.UpdateState();
        this.ShipUpdated();
    }

    private PrevCondition!: number;
    private UpdateCondition(): void {
        const condition: number = this.Ships.reduce((a, c) => a === 0 ? c.Condition : Math.min(a, c.Condition), 0);
        const goal = Const.ConditionRestorationLimit;

        // Require time not changed
        if (this.PrevCondition === condition) return;

        this.PrevCondition = condition;
        if (condition >= goal)
            this.$._ConditionRestoreTime = 0;
        else {
            let restore = Date.now();

            const value = Math.floor((goal - condition + 2) / 3) * 3; // Integer dividing
            restore = restore + value * 60 * 1000;

            this.$._ConditionRestoreTime = restore;
        }
        this.RaisePropertyChanged(nameof(this.IsConditionRestoring));
        this.UpdateState();
    }

    public UpdateState(): void {
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
            if (ships.some(x => this.homeport.RepairDock!.CheckRepairing(x.Id)))
                state |= FleetState.Repairing;

            // Not fully supplied
            if (ships.some(x => x.Fuel.Current < x.Fuel.Maximum || x.Ammo.Current < x.Ammo.Maximum))
                state |= FleetState.NeedSupply;

            // Condition restoring
            if (this.IsConditionRestoring)
                state |= FleetState.ConditionRestoring;
        }

        const heavilyDamaged: boolean = ships
            .filter(x => !this.homeport.RepairDock!.CheckRepairing(x.Id))
            .filter(x => !(x.State & Ship.State.Evacuation) && !(x.State & Ship.State.Tow))
            .filter(x => !(state & FleetState.Sailing) && (x.State & Ship.State.DamageControlled))
            .some(x => x.HP.Percentage <= 0.25);
        if (heavilyDamaged)
            state |= FleetState.HeavilyDamaged;

        if (ships.length > 0) {
            if (ships[0].Info.ShipType && (ships[0].Info.ShipType.Id == ShipType.RepairShip))
                state |= FleetState.FlagshipRepairShip;
        }

        this.$._State = state;
    }
    public Calculate(): void {
        const ships = this.Ships.filter(x => // Only actually exists in fleet
            (x.State & Ship.State.Tow) === 0
            && (x.State & Ship.State.Evacuation) === 0
        );

        this.$._AirSupremacy = AirSupremacy.Sum(ships.map(x => x.AirSupremacy));

        const calculator = LoSCalculator.Instance.Get(Settings.Instance.LoSCalculator);
        if (calculator)
            this.$._LoS = calculator.Calc([this]);
        else
            this.$._LoS = 0;
    }

    public UnsetAll(): void {
        const list = [this.SourceShips[0]];
        const ships = new Array(this.SourceShips.length);
        for (let i = 0; i < list.length; i++)
            ships[i] = list[i];

        this.UpdateShips(ships);
    }
    public Unset(index: number): void {
        const list = this.SourceShips.filter((_, y) => y !== index);
        const ships = new Array(this.SourceShips.length);
        for (let i = 0; i < list.length; i++)
            ships[i] = list[i];

        this.UpdateShips(ships);
    }
    public Change(index: number, ship: Ship): Ship {
        const current = this.SourceShips[index];

        let list: Ship[] = [];
        if (index == -1) {
            list = [(<Ship>this.SourceShips.find(x => true))];
        }
        else {
            const temp = this.SourceShips;
            temp[index] = ship;
            list = <Ship[]>temp.filter(x => x);
        }

        const ships = new Array(this.SourceShips.length);
        for (let i = 0; i < list.length; i++)
            ships[i] = list[i];

        this.UpdateShips(ships);

        return <Ship>current;
    }
    public ShipUpdated(): void {
        this.RaisePropertyChanged(nameof(this.Ships));
        this.RaisePropertyChanged(nameof(this.FleetSpeed));
    }

    protected Tick(): void {
        super.Tick();

        if(this.IsConditionRestoring){
            this.RaisePropertyChanged(nameof(this.ConditionRestoreTime));
        }
    }
}