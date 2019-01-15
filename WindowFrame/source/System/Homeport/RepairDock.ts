import { Observable } from "../Base/Observable";
import { SubscribeKcsapi } from "../Base/KcsApi";
import { kcsapi_ndock } from "../Interfaces/kcsapi_dock";
import { kcsapi_nyukyo_start, kcsapi_nyukyo_speedchange } from "../Interfaces/kcsapi_repair";

import { Homeport } from "./Homeport";
import { Ship } from "./Ship";

export class RepairDock extends Observable {
    public Docks: RepairDock.Dock[];
    private Owner: Homeport;

    constructor(Owner: Homeport) {
        super();
        this.Owner = Owner;
        this.Docks = [];

        SubscribeKcsapi<kcsapi_ndock[]>(
            "api_get_member/ndock",
            x => this.Update(x)
        );
        SubscribeKcsapi<{}, kcsapi_nyukyo_start>(
            "api_req_nyukyo/start",
            (x, y) => this.Start(y)
        );

        // Used bucket while normal repair
        SubscribeKcsapi<{}, kcsapi_nyukyo_speedchange>(
            "api_req_nyukyo/speedchange",
            (x, y) => this.ChangeSpeed(y)
        );
    }

    public Update(source: kcsapi_ndock[]): void {
        if (this.Docks.length === source.length)
            source.forEach(raw => this.Docks[raw.api_id].Update(raw));

        else {
            this.Docks.forEach(dock => dock.Dispose());

            this.Docks = source.map(x => new RepairDock.Dock(x));
        }
    }

    private Start(request: kcsapi_nyukyo_start): void {
        const ship = this.Owner.Ships[request.api_ship_id];
        if (!ship) return;

        if (request.api_highspeed === 1)
            ship.Repair();

        // If bucket not used, ndock api will be delivered.
        // So, not need to process ndock data.
    }
    private ChangeSpeed(request: kcsapi_nyukyo_speedchange): void {
        try {
            const dock = this.Docks[request.api_ndock_id];
            const ship = dock.Ship;

            dock.Finish();
            if(ship) ship.Repair();
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
    export class Dock extends TickNotifier {
        private notified: boolean = false;

        public Completed: DockComplete | DockComplete[] | null = null;

        public Id: number = -1;
        public State: DockState = DockState.Locked;

        public ShipId: number = -1;

        private _Ship: Ship | null = null;
        public get Ship(): Ship | null { return this._Ship; }
        public set Ship(ship: Ship | null) {
            if (this._Ship) this._Ship.Situation &= ~Ship.Situation.Repairing;
            if (ship) ship.Situation |= Ship.Situation.Repairing;
            this._Ship = ship;
        }

        public CompleteTime: number = 0;
        public Remaining: number = 0;

        constructor(dock: kcsapi_ndock) {
            super();

            this.Update(dock);
        }
        public Update(ndock: kcsapi_ndock): void {
            this.Id = 0;
        }
        public Finish(): void {
            this.State = RepairDock.DockState.Free;
            this.ShipId = -1;
            this.Ship = null;
            this.CompleteTime = 0;
        }

        protected Tick(): void {
			if (this.CompleteTime !== 0)
			{
				let remaining = this.CompleteTime - Date.now();
				if (remaining < 0) remaining = 0;
				this.Remaining = remaining;

				if (!this.notified && this.Completed && remaining <= Settings.NotificationTime * 1000)
				{
                    fns(this.Completed, this, this.Id, this.Ship as Ship);
					this.notified = true;
				}
			}
			else this.Remaining = 0;
        }
    }
}
