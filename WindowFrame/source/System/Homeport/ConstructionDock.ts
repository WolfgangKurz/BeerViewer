import { Observable, TickObservable } from "System/Base/Observable";
import { SubscribeKcsapi } from "System/Base/KcsApi";
import { kcsapi_kdock, kcsapi_kdock_getship, kcsapi_req_kousyou_createship_speedchange } from "System/Interfaces/kcsapi_dock";

import { Homeport } from "./Homeport";
import { Ship } from "./Ship";
import { Settings } from "System/Settings";
import { fns } from "System/Base/Base";
import { IdentifiableTable } from "System/Models/TableWrapper";

export class ConstructionDock extends Observable {
	public Docks: IdentifiableTable<ConstructionDock.Dock>;
	private homeport: Homeport;

	constructor(homeport: Homeport) {
		super();
		this.homeport = homeport;
		this.Docks = new IdentifiableTable<ConstructionDock.Dock>();

		SubscribeKcsapi<kcsapi_kdock[]>(
			"api_get_member/kdock",
			x => this.Update(x)
		);
		SubscribeKcsapi<kcsapi_kdock_getship>(
			"api_req_kousyou/getship",
			x => this.GetShip(x)
		);

		// Used bucket while normal repair
		SubscribeKcsapi<{}, kcsapi_req_kousyou_createship_speedchange>(
			"api_req_kousyou/createship_speedchange",
			(x, y) => this.ChangeSpeed(y)
		);
	}

	public Update(source: kcsapi_kdock[]): void {
		if (this.Docks.size === source.length)
			source.forEach(raw => this.Docks.get(raw.api_id)!.Update(raw));

		else {
			this.Docks.forEach(dock => dock.Dispose());
			this.$.Docks = new IdentifiableTable<ConstructionDock.Dock>(source.map(x => new ConstructionDock.Dock(this.homeport, x)));
		}
	}

	private GetShip(source: kcsapi_kdock_getship): void {
		this.Update(source.api_kdock);
	}
	private ChangeSpeed(request: kcsapi_req_kousyou_createship_speedchange): void {
		try {
			const dock = this.Docks.get(request.api_kdock_id);
			if (request.api_highspeed === 1) dock!.Finish();
		}
		catch { }
	}
}
export namespace ConstructionDock {
	interface DockComplete {
		(Dock: Dock, Id: number, Ship: Ship): void;
	}
	export enum DockState {
		Locked = -1,
		Free = 0,
		Building = 2,
		Done = 3
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
			this.$._Ship = ship;
		}

		public CompleteTime: number = 0;
		public Remaining: number = 0;

		constructor(homeport: Homeport, dock: kcsapi_kdock) {
			super();
			this.homeport = homeport;

			this.Update(dock);
		}
		public Update(ndock: kcsapi_kdock): void {
			this.$.Id = ndock.api_id;
			this.$.State = ndock.api_state;

			this.$.ShipId = ndock.api_created_ship_id;
			this.$.Ship =
				this.State === DockState.Building
					? this.homeport.Ships.get(this.ShipId) || null
					: null;

			this.$.CompleteTime =
				this.State === DockState.Building
					? ndock.api_complete_time
					: 0;
			this.$.Remaining = this.CompleteTime;
		}
		public Finish(): void {
			this.$.State = ConstructionDock.DockState.Done;
			this.$.CompleteTime = 0;
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
