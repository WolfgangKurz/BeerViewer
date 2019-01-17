import { Fleet } from "../Fleet";
import { TickObservable } from "../../Base/Observable";
import { ExpeditionInfo } from "../../Master/Wrappers/ExpeditionInfo";
import { Progress } from "../../Models/GuageValue";
import { ExpeditionResult } from "./ExpeditionResult";
import { Master } from "../../Master/Master";

export class Expedition extends TickObservable {
    private readonly fleet: Fleet;
    private notificated: boolean = false;

    public Id: number = -1;
    public Expedition: ExpeditionInfo | null = null;

    private _ReturnTime: number = 0;
    public get ReturnTime(): number { return this._ReturnTime }
    public set ReturnTime(value: number) {
        this._ReturnTime = value;
        if (this._ReturnTime != value) {
            this.notificated = false;
            this.RaisePropertyChanged(nameof(this.Remaining));
            this.RaisePropertyChanged(nameof(this.IsInExecution));
        }
    }

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
    }

    public Update(rawData: [number, number, number, number]): void {
        if (rawData.length != 4 || rawData.filter(x => x === 0).length == 4) {
            this.Id = -1;
            this.Expedition = null;
            this.ReturnTime = 0;
        } else {
            this.Id = rawData[1];
            this.Expedition = Master.Instance.Expeditions!.get(this.Id) || null;
            this.ReturnTime = rawData[2];
        }
    }
}