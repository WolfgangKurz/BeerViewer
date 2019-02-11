import { Fleet } from "System/Homeport/Fleet";
import { TickObservable } from "System/Base/Observable";
import { ExpeditionInfo } from "System/Master/Wrappers/ExpeditionInfo";
import { Progress } from "System/Models/GuageValue";
import { ExpeditionResult } from "./ExpeditionResult";
import { Master } from "System/Master/Master";
import { SubscribeKcsapi } from "System/Base/KcsApi";
import { kcsapi_mission_result } from "System/Interfaces/kcsapi_mission_result";
import Settings from "System/Settings";
import { fns } from "System/Base/Base";

export class Expedition extends TickObservable {
	private readonly fleet: Fleet;
	private notificated: boolean = false;

	private _Id: number = -1;
	public get Id(): number { return this._Id }

	private _Expedition: ExpeditionInfo | null = null;
	public get Expedition(): ExpeditionInfo | null { return this._Expedition }

	//#region ReturnTime
	// set _ReturnTime -> Observable will call "ReturnTime" PropertyChanged callbacks
	private __ReturnTime: number = 0;
	private get _ReturnTime(): number { return this.__ReturnTime }
	private set _ReturnTime(value: number) {
		if (this.__ReturnTime != value) {
			this.__ReturnTime = value;
			this.notificated = false;
			this.RaisePropertyChanged(nameof(this.Remaining));
			this.RaisePropertyChanged(nameof(this.IsInExecution));
		}
	}
	public get ReturnTime(): number { return this._ReturnTime }
	//#endregion

	/** Is this expedition executing? */
	public get IsInExecution(): boolean { return this.Id > 0 && this.ReturnTime != 0 }

	/** Remaining time in msecs that return from expedition */
	public get Remaining(): number { return Math.max(0, this.ReturnTime - Date.now()) }

	/** Progress (seconds value) */
	public get Progress(): Progress {
		if (this.ReturnTime === 0) return new Progress();
		if (this.Expedition == null) return new Progress();

		var start = this.ReturnTime - (this.Expedition.raw.api_time * 60 * 1000);
		var value = Math.floor((Date.now() - start) / 1000); // as secs
		return new Progress(value, this.Expedition.raw.api_time * 60, 0);
	}

	public Returned: Expedition.ExpeditionComplete | Expedition.ExpeditionComplete[] | null = null;

	public ExpeditionResult: ExpeditionResult | null = null;

	constructor(fleet: Fleet) {
		super();
		this.fleet = fleet;

		this.ManagedDisposable.Add(
			SubscribeKcsapi<kcsapi_mission_result>("api_req_mission/result", x => {
				const mission = Master.Instance.Expeditions!.values()
					.find(y => y.Title === x.api_quest_name);

				if (mission && mission.Id === this.Id) this.Done(x);
			})
		);
	}

	public Done(mission: kcsapi_mission_result) {
		this.ExpeditionResult = new ExpeditionResult(mission);
	}

	public Update(rawData: [number, number, number, number]): void {
		if (rawData.length != 4 || rawData.filter(x => x === 0).length == 4) {
			this._Id = -1;
			this._Expedition = null;
			this._ReturnTime = 0;
		} else {
			this._Id = rawData[1];
			this._Expedition = Master.Instance.Expeditions!.get(this.Id) || null;
			this._ReturnTime = rawData[2];
		}
	}

	protected Tick(): void {
		this.RaisePropertyChanged(nameof(this.Remaining));
		this.RaisePropertyChanged(nameof(this.Progress));

		if (!this.notificated && this.Returned != null && this.Remaining <= <number>Settings.Notifictaion.NotificationTime.Value * 1000) {
			fns(this.Returned, this.fleet.Name);
			this.notificated = true;
		}
	}
}
export namespace Expedition {
	export interface ExpeditionComplete {
		(Fleet: string): void;
	}
}