import { DisposableContainer, IDisposable } from "./Interfaces/IDisposable";
import { fns } from "./Base";

/** Observable object class.
 * 
 * Register callback with `Observe` function, `ObservableCallback` will be called when property has set.
 * - Global scope observer will be call `callback` when any property has set.
 * - Property scope observer will be call `callback` when specified property has set.
 * 
 * `Observable` is `IDisposable`, can use with `using` statement.
 */
export class Observable implements IDisposable {
	private _proxy: { proxy: Observable, revoke: () => void };
	private _callbacks: {
		[name: string]: ObservableCallback[],
		[name: number]: ObservableCallback[]
	}

	/** When set value with `$`, Property changed event will be called */
	public get $(): this { return <this>this._proxy.proxy }

	constructor() {
		const _this = this;
		this._callbacks = {};
		this._proxy = Proxy.revocable(this, {
			set(target: any, name, value) {
				const oldValue = target[name];
				target[name] = value;

				if (typeof name === "symbol") // Extract symbol
					name = name.description || "";
				else if (typeof name === "number") // Convert number to string
					name = name.toString();

				// maybe empty Symbol or undefined Symbol
				if (name.length !== 0) {
					if (name[0] !== '_') { // Name not starts with `_`
						_this.PropertyChanged(name, value, oldValue);
					} else if (name[1] !== '_') { // Name not starts with `__` (double underscore), When single underscore
						const key = name.substr(1);
						if (key in target)
							_this.PropertyChanged(key, value, oldValue); // Call original name if exists
					}
				}
				return true;
			}
		});
	}
	Dispose(): void {
		this._proxy.revoke();
	}

	/** @description Register observer to object, call callback when property has set.
	 * @param {ObservableCallback} callback Callback that be called when property has set.
	 * @param {string | number | undefined} name If set, register `Property scope observer`. If not, register `Global scope observer`.
	 * @param {boolean} CallSetup If set as true, `callback` will be called after register observer. Default is `true`.
	 */
	public Observe(callback: ObservableCallback, name?: string | number, CallSetup: boolean = true): Observable {
		if (!name) name = "*";

		if (!(name in this._callbacks)) this._callbacks[name] = [];
		this._callbacks[name].push(callback);

		if (CallSetup) callback(name.toString(), (<any>this)[name], undefined);
		return this;
	}

	/** @description Remove observer from object, registered by `Observe` function.
	 * @param {ObservableCallback} callback Callback that registered.
	 * @param {string | number | undefined} name Name of registered callback.
	 */
	public Deobserve(callback: ObservableCallback, name?: string | number): Observable {
		if (!name) name = "*";
		this._callbacks[name] = this._callbacks[name].filter(x => x !== callback);
		return this;
	}

	protected RaisePropertyChanged(name: string | number) {
		if (name in this._callbacks)
			fns(this._callbacks[name], name, (<any>this)[name], undefined);
	}

	private PropertyChanged(name: string | number, value: any, oldValue: any): void {
		if (name in this._callbacks)
			fns(this._callbacks[name], name, value, oldValue);
	}
}

/** Observable contains `ManagedDisposable: DisposableContainer` */
export class DisposableObservable extends Observable implements IDisposable {
	protected ManagedDisposable: DisposableContainer;

	constructor() {
		super();
		this.ManagedDisposable = new DisposableContainer();
	}
	public Dispose(): void {
		this.ManagedDisposable.Dispose();
		super.Dispose();
	}
}

/** Observable contains `Tick()` called every 1sec */
export class TickObservable extends DisposableObservable implements IDisposable {
	private timer: number;

	constructor() {
		super();
		this.timer = setInterval(() => this.Tick(), 1000);
	}

	protected Tick(): void { }

	public Dispose(): void {
		clearInterval(this.timer);
		this.timer = 0;

		super.Dispose();
	}
}

/** Will be called when registered property has set.
 * 
 * `ObservableCallback(name: string, value: any, oldValue: any): void;`
 */
export interface ObservableCallback {
	(name: string, value: any, oldValue: any): void;
}