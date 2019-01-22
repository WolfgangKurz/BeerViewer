import { Fleet } from "../Fleet";
import { TickObservable } from "../../Base/Observable";
import { ExpeditionInfo } from "../../Master/Wrappers/ExpeditionInfo";
import { Progress } from "../../Models/GuageValue";
import { ExpeditionResult } from "./ExpeditionResult";
import { Master } from "../../Master/Master";
import { SubscribeKcsapi } from "../../Base/KcsApi";
import { kcsapi_mission_result } from "../../Interfaces/kcsapi_mission_result";

export class Expedition extends TickObservable {
    private readonly fleet: Fleet;
    private notificated: boolean = false;

    public Id: number = -1;
    public Expedition: ExpeditionInfo | null = null;

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
    public get IsInExecution(): boolean { return this.ReturnTime != 0 }

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
            this.Id = -1;
            this.Expedition = null;
            this._ReturnTime = 0;
        } else {
            this.Id = rawData[1];
            this.Expedition = Master.Instance.Expeditions!.get(this.Id) || null;
            this._ReturnTime = rawData[2];
        }
    }
}