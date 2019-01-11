interface PropertyChanged {
    (propertyName: string | null): void;
}
interface INotifyPropertyChanged {
    PropertyChanged: PropertyChanged | PropertyChanged[] | null;
}

class Notifier implements INotifyPropertyChanged {
    public PropertyChanged: PropertyChanged | PropertyChanged[] | null = null;

    protected RaisePropertyChanged(propertyName: string | null = null): void {
        if (this.PropertyChanged === null) return;

        if ((<PropertyChanged[]>this.PropertyChanged).length) {
            const list = this.PropertyChanged as PropertyChanged[];
            list.forEach(x => x(propertyName));
            return;
        }
        (<PropertyChanged>this.PropertyChanged)(propertyName);
    }

    public PropertyEvent(PropertyName: string, Handler: Function, RaiseRegistered: boolean = false): void {
        if (this.PropertyChanged === null) {
            this.PropertyChanged = x => {
                if (x === PropertyName && Handler)
                    Handler();
            };
        }

        if (RaiseRegistered && Handler) Handler();
    }
}
class TickNotifier extends Notifier implements IDisposable {
    private handler: number;

    constructor() {
        super();
        this.handler = setInterval(this.Tick, 1000);
    }

    protected Tick(): void { }

    Dispose(): void {
        clearInterval(this.handler);
    }
}