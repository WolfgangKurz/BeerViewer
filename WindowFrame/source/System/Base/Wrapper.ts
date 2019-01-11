class RawDataWrapper<T> {
    private _raw: T;
    public get raw() {
        return this._raw;
    }

    constructor(RawData: T) {
        this._raw = RawData;
    }

    protected UpdateData(RawData: T) {
        this._raw = RawData;
    }
}