export class GuageValue {
    public readonly Current: number;
    public readonly Maximum: number;
    public readonly Minimum: number;

    public get Percentage(): number {
        return (this.Maximum - this.Minimum) !== 0
            ? (this.Current - this.Minimum) / (this.Maximum - this.Minimum)
            : 0;
    }

    constructor(Current: number = 0, Maximum: number = 0, Minimum: number = 0) {
        // Invalid, swap
        if (Maximum < Minimum) {
            const temp = Maximum;
            Maximum = Minimum;
            Minimum = temp;
        }

        // Adjust value
        this.Current = Math.max(Minimum, Math.min(Maximum, Current));
        this.Maximum = Maximum;
        this.Minimum = Minimum;
    }

    public Update(Current: number): GuageValue {
        return new GuageValue(Current, this.Maximum, this.Minimum);
    }
}
export class GenericGuageValue<T> {
    public readonly Current: T;
    public readonly Maximum: T;
    public readonly Minimum: T;

    constructor(Current: T, Maximum: T, Minimum: T) {
        this.Current = Current;
        this.Maximum = Maximum;
        this.Minimum = Minimum;
    }

    public Update(Current: T): GenericGuageValue<T> {
        return new GenericGuageValue<T>(Current, this.Maximum, this.Minimum);
    }
}