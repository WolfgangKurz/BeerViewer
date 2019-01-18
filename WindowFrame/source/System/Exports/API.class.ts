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

export class HTTPRequest {
    [key: string]: string[] | string | number;

    constructor(data: any) {
        if (data === undefined || data === null) return;

        for (let key in data) {
            const value = data[key], nvalue = Number(value);
            if (nvalue.toString() === value)
                this[key] = nvalue; // Real numeric
            else
                this[key] = value;
        }
    }
}
