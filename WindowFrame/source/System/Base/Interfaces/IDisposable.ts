export interface IDisposable {
	Dispose(): void;
}

export class DisposableContainer implements IDisposable {
	private readonly DisposableList: IDisposable[] = [];

	public Add(disposable: IDisposable | Promise<IDisposable>): void {
		if(disposable instanceof Promise){
			disposable.then(x => this.DisposableList.push(x));
		}else
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
