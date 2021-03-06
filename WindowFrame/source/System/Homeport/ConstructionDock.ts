import { Observable, TickObservable } from "System/Base/Observable";
import { SubscribeKcsapi } from "System/Base/KcsApi";
import { kcsapi_kdock, kcsapi_kdock_getship, kcsapi_req_kousyou_createship_speedchange } from "System/Interfaces/kcsapi_dock";

import { Homeport } from "./Homeport";
import Settings from "System/Settings";
import { fns } from "System/Base/Base";
import { IdentifiableTable } from "System/Models/TableWrapper";
import { Master } from "System/Master/Master";
import { ShipInfo } from "System/Master/Wrappers/ShipInfo";

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
		console.log(source);
		if (this.Docks.size === source.length)
			source.forEach(raw => this.Docks.get(raw.api_id)!.Update(raw));

		else {
			this.Docks.forEach(dock => dock.Dispose());
			this.$.Docks = new IdentifiableTable<ConstructionDock.Dock>(source.map(x => new ConstructionDock.Dock(x)));
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
		(Dock: Dock, Id: number, Ship: ShipInfo): void;
	}
	export enum DockState {
		Locked = -1,
		Free = 0,
		Building = 2,
		Done = 3
	}
	export class Dock extends TickObservable {
		private notified: boolean = false;

		public Completed: DockComplete | DockComplete[] | null = null;

		//#region Id
		private _Id: number = -1;
		public get Id() { return this._Id }
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
		private _Ship: ShipInfo | null = null;
		public get Ship(): ShipInfo | null { return this._Ship; }
		//#endregion

		//#region CompleteTime
		private _CompleteTime: number = 0;
		public get CompleteTime(): number { return this._CompleteTime }
		//#endregion

		//#region Remaining
		private _Remaining: number = 0;
		public get Remaining(): number { return this._Remaining }
		//#endregion

		constructor(dock: kcsapi_kdock) {
			super();
			this.Update(dock);
		}
		public Update(ndock: kcsapi_kdock): void {
			this.$._Id = ndock.api_id;
			this.$._State = ndock.api_state;

			this.$._ShipId = ndock.api_created_ship_id;
			this.$._Ship =
				(this.State === DockState.Building || this.State === DockState.Done)
					? Master.Instance.Ships!.get(this.ShipId) || null
					: null;

			this.$._CompleteTime =
				this.State === DockState.Building
					? ndock.api_complete_time
					: 0;
			this.$._Remaining = this.CompleteTime - Date.now();
		}
		public Finish(): void {
			this.$._State = ConstructionDock.DockState.Done;
			this.$._CompleteTime = 0;
		}

		protected Tick(): void {
			if (this.CompleteTime !== 0) {
				let remaining = this.CompleteTime - Date.now();
				if (remaining < 0) remaining = 0;
				this.$._Remaining = remaining;

				if (!this.notified && this.Completed && remaining <= <number>Settings.Notification.NotificationTime.Value * 1000) {
					fns(this.Completed, this, this.Id, this.Ship as ShipInfo);
					this.$.notified = true;
				}
			}
			else this.$._Remaining = 0;
		}
	}
}
