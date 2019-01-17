import { Fleet } from "../Fleet";
import { TickObservable } from "../../Base/Observable";
import { ExpeditionInfo } from "../../Master/Wrappers/ExpeditionInfo";

export class Expedition extends TickObservable {
    private readonly fleet: Fleet;
    private notificated: boolean = false;

    public  Id:number;
    public Mission :ExpeditionInfo;

    private _ReturnTime:number = 0;
    public get ReturnTime():number{return this._ReturnTime}
    public set ReturnTime(value:number) {
        this._ReturnTime = value;
        if (this._ReturnTime != value)
        {
            this.notificated = false;
            this.RaisePropertyChanged(nameof(this.Remaining));
            this.RaisePropertyChanged(nameof(this.IsInExecution));
        }
    }

    /** Is this expedition executing? */
    public get IsInExecution(): boolean { return this.ReturnTime != 0 }

    /** Remaining time in msecs that return from expedition */
    public get Remaining(): number { return Math.max(0, this.ReturnTime - Date.now()) }


    constructor(fleet: Fleet) {
        super();
        this.fleet = fleet;
    }
}