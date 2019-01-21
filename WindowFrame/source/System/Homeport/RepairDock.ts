import { Observable, TickObservable } from "../Base/Observable";
import { SubscribeKcsapi } from "../Base/KcsApi";
import { kcsapi_ndock } from "../Interfaces/kcsapi_dock";
import { kcsapi_req_nyukyo_start, kcsapi_req_nyukyo_speedchange } from "../Interfaces/kcsapi_repair";

import { Homeport } from "./Homeport";
import { Ship } from "./Ship";
import { Fleet } from "./Fleet";
import { Settings } from "../Settings";
import { fns } from "../Base/Base";

export class RepairDock extends Observable {
    public Docks: RepairDock.Dock[];
    private homeport: Homeport;

    constructor(homeport: Homeport) {
        super();
        this.homeport = homeport;
        this.Docks = [];

        SubscribeKcsapi<kcsapi_ndock[]>(
            "api_get_member/ndock",
            x => this.Update(x)
        );
        SubscribeKcsapi<{}, kcsapi_req_nyukyo_start>(
            "api_req_nyukyo/start",
            (x, y) => this.Start(y)
        );

        // Used bucket while normal repair
        SubscribeKcsapi<{}, kcsapi_req_nyukyo_speedchange>(
            "api_req_nyukyo/speedchange",
            (x, y) => this.ChangeSpeed(y)
        );
    }

    public CheckRepairing(shipId_or_fleet: number | Fleet): boolean {
        if (shipId_or_fleet instanceof Fleet) {
            const fleet = shipId_or_fleet;
            const repairing = this.Docks.filter(x => x.ShipId != -1).map(x => x.ShipId);
            return fleet.Ships.some(x => repairing.indexOf(x.Id) >= 0);
        } else {
            const shipId = shipId_or_fleet;
            return this.Docks.some(x => x.ShipId === shipId);
        }
    }

    public Update(source: kcsapi_ndock[]): void {
        if (this.Docks.length === source.length)
            source.forEach(raw => this.Docks[raw.api_id].Update(raw));

        else {
            this.Docks.forEach(dock => dock.Dispose());
            this.$.Docks = source.map(x => new RepairDock.Dock(this.homeport, x));
        }
    }

    private Start(request: kcsapi_req_nyukyo_start): void {
        const ship = this.homeport.Ships.get(request.api_ship_id);
        if (!ship) return;

        if (request.api_highspeed === 1)
            ship.Repair();

        // If bucket not used, ndock api will be delivered.
        // So, not need to process ndock data.
    }
    private ChangeSpeed(request: kcsapi_req_nyukyo_speedchange): void {
        try {
            const dock = this.Docks[request.api_ndock_id];
            const ship = dock.Ship;

            dock.Finish();
            if (ship) ship.Repair();
        }
        catch { }
    }
}
export namespace RepairDock {
    interface DockComplete {
        (Dock: Dock, Id: number, Ship: Ship): void;
    }
    export enum DockState {
        Locked = -1,
        Free = 0,
        Repairing = 1,
    }
    export class Dock extends TickObservable {
        private homeport: Homeport;
        private notified: boolean = false;

        public Completed: DockComplete | DockComplete[] | null = null;

        public Id: number = -1;
        public State: DockState = DockState.Locked;

        public ShipId: number = -1;

        private _Ship: Ship | null = null;
        public get Ship(): Ship | null { return this._Ship; }
        public set Ship(ship: Ship | null) {
            if (this.Ship) this.Ship.Repair();
            if (ship) ship.Repairing();
            this.$._Ship = ship;
        }

        public CompleteTime: number = 0;
        public Remaining: number = 0;

        constructor(homeport: Homeport, dock: kcsapi_ndock) {
            super();
            this.homeport = homeport;

            this.Update(dock);
        }
        public Update(ndock: kcsapi_ndock): void {
            this.$.Id = ndock.api_id;
            this.$.State = ndock.api_state;

            this.$.ShipId = ndock.api_ship_id;
            this.Ship =
                this.State === DockState.Repairing
                    ? this.homeport.Ships.get(this.ShipId) || null
                    : null;

            this.CompleteTime =
                this.State === DockState.Repairing
                    ? ndock.api_complete_time
                    : 0;
            this.$.Remaining = this.CompleteTime;
        }
        public Finish(): void {
            this.$.State = RepairDock.DockState.Free;
            this.$.ShipId = -1;
            this.$.Ship = null;
            this.$.CompleteTime = 0;
            this.$.Remaining = 0;
        }

        protected Tick(): void {
            if (this.CompleteTime !== 0) {
                let remaining = this.CompleteTime - Date.now();
                if (remaining < 0) remaining = 0;
                this.$.Remaining = remaining;

                if (!this.notified && this.Completed && remaining <= Settings.Instance.NotificationTime * 1000) {
                    fns(this.Completed, this, this.Id, this.Ship as Ship);
                    this.$.notified = true;
                }
            }
            else this.$.Remaining = 0;
        }
    }
}
