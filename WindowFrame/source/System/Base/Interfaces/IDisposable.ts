export interface IDisposable {
    Dispose(): void;
}

export class DisposableContainer implements IDisposable {
    private readonly DisposableList: IDisposable[] = [];

    public Add(disposable: IDisposable): void {
        this.DisposableList.push(disposable);
    }
    public Dispose(): void {
        this.DisposableList
            .splice(0, this.DisposableList.length)
            .forEach(x => x.Dispose());
    }
}

export function using<T extends IDisposable>(resource: T, func: (resource: T) => void) {
    try {
        func(resource);
    } finally {
        resource.Dispose();
    }
}
