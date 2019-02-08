import { Observable, TickObservable } from "System/Base/Observable";
import { SubscribeKcsapi } from "System/Base/KcsApi";
import { kcsapi_ndock } from "System/Interfaces/kcsapi_dock";
import { kcsapi_req_nyukyo_start, kcsapi_req_nyukyo_speedchange } from "System/Interfaces/kcsapi_repair";

import { Homeport } from "./Homeport";
import { Ship } from "./Ship";
import { Fleet } from "./Fleet";
import Settings from "System/Settings";
import { fns } from "System/Base/Base";
import { IdentifiableTable } from "System/Models/TableWrapper";

export class RepairDock extends Observable {
	public Docks: IdentifiableTable<RepairDock.Dock>;
	private homeport: Homeport;

	constructor(homeport: Homeport) {
		super();
		this.homeport = homeport;
		this.Docks = new IdentifiableTable<RepairDock.Dock>();

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
			const repairing = this.Docks.values().filter(x => x.ShipId != -1).map(x => x.ShipId);
			return fleet.Ships.some(x => repairing.indexOf(x.Id) >= 0);
		} else {
			const shipId = shipId_or_fleet;
			return this.Docks.values().some(x => x.ShipId === shipId);
		}
	}

	public Update(source: kcsapi_ndock[]): void {
		if (this.Docks.size === source.length)
			source.forEach(raw => this.Docks.get(raw.api_id)!.Update(raw));

		else {
			this.Docks.forEach(dock => dock.Dispose());
			this.$.Docks = new IdentifiableTable<RepairDock.Dock>(source.map(x => new RepairDock.Dock(this.homeport, x)));
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
			const dock = this.Docks.get(request.api_ndock_id);
			const ship = dock!.Ship;

			dock!.Finish();
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

		//#region Id
		private _Id: number = -1;
		public get Id(): number { return this._Id }
		//#endregion

		//#region State
		private _State: DockState = DockState.Locked;
		public get State(): DockState { return this._State }
		//#endregion

		//#region ShipId
		private _ShipId: number = -1;
		public get ShipId(): number { return this._ShipId }
		//#endregion

		//#region Ship
		private _Ship: Ship | null = null;
		public get Ship(): Ship | null { return this._Ship; }
		public set Ship(ship: Ship | null) {
			if (this.Ship) this.Ship.Repair();
			if (ship) ship.Repairing();
			this.$._Ship = ship;
		}
		//#endregion

		//#region CompleteTime
		private _CompleteTime: number = 0;
		public get CompleteTime(): number { return this._CompleteTime }
		//#endregion

		//#region Remaining
		private _Remaining: number = 0;
		public get Remaining(): number { return this._Remaining }
		//#endregion

		constructor(homeport: Homeport, dock: kcsapi_ndock) {
			super();
			this.homeport = homeport;

			this.Update(dock);
		}
		public Update(ndock: kcsapi_ndock): void {
			this.$._Id = ndock.api_id;
			this.$._State = ndock.api_state;

			this.$._ShipId = ndock.api_ship_id;
			this.Ship =
				this.State === DockState.Repairing
					? this.homeport.Ships.get(this.ShipId) || null
					: null;

			this.$._CompleteTime =
				this.State === DockState.Repairing
					? ndock.api_complete_time
					: 0;
			this.$._Remaining = this.CompleteTime;
		}
		public Finish(): void {
			this.$._State = RepairDock.DockState.Free;
			this.$._ShipId = -1;
			this.$.Ship = null;
			this.$._CompleteTime = 0;
			this.$._Remaining = 0;
		}

		protected Tick(): void {
			if (this.CompleteTime !== 0) {
				let remaining = this.CompleteTime - Date.now();
				if (remaining < 0) remaining = 0;
				this.$._Remaining = remaining;

				if (!this.notified && this.Completed && remaining <= Settings.Instance.Notification.NotificationTime.Value * 1000) {
					fns(this.Completed, this, this.Id, this.Ship as Ship);
					this.$.notified = true;
				}
			}
			else this.$._Remaining = 0;
		}
	}
}
