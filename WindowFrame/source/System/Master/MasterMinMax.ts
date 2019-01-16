export class MasterMinMax {
    private _min: number;
    private _max: number;

    public get Min(): number { return this._min }
    public get Max(): number { return this._max }

    constructor(value: [number, number]) {
        this._min = value[0];
        this._max = value[1];
    }

    public toString(): string {
        return `{"Min": ${this.Min}, "Max": ${this.Max}}`;
    }
}