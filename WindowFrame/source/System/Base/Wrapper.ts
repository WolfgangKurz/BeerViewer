import { Observable, ObservableCallback } from "./Observable";

export interface IRawDataWrapper<T> {
    raw: T;
}

export class RawDataWrapper<T> implements IRawDataWrapper<T> {
    private _raw: T;
    public get raw(): T {
        return this._raw;
    }

    constructor(RawData: T) {
        this._raw = RawData;
    }

    protected UpdateData(RawData: T): void {
        this._raw = RawData;
    }
}
export class ObservableDataWrapper<T> extends Observable implements IRawDataWrapper<T> {
    private _raw: T;
    public get raw(): T {
        return this.$._raw;
    }

    constructor(RawData: T) {
        super();
        this._raw = RawData;
    }

    protected UpdateData(RawData: T): void {
        this.$._raw = RawData;
    }
}