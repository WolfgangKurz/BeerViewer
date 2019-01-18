import { IDisposable } from "../Base/Interfaces/IDisposable";

/** Subscribe information */
export class SubscribeInfo implements IDisposable {
    private _Id: number;
    public get Id(): number { return this._Id }

    constructor(Id: number) {
        this._Id = Id;
    }

    Dispose(): void {
        if (this.Id >= 0)
            window.API.UnsubscribeHTTP(this.Id);
    }
}
