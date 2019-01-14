class Ticker implements IDisposable {
    private handler: number;

    constructor() {
        this.handler = setInterval(this.Tick, 1000);
    }

    protected Tick(): void { }

    Dispose(): void {
        clearInterval(this.handler);
    }
}