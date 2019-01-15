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

    constructor() {
        const _this = this;
        this._callbacks = {};
        this._proxy = Proxy.revocable(this, {
            set(target: any, name, value) {
                const oldValue = target[name];
                target[name] = value;

                if (typeof name != "symbol") // Symbol cannot be used on PropertyChanged, because of callbacks.
                    _this.PropertyChanged(name, value, oldValue);
                return true;
            }
        });
    }
    Dispose(): void {
        this._proxy.revoke();
    }

    /** @description Register observer to object, call callback when property has set.
     * @param {ObservableCallback} callback Callback that be called when property has set.
     * @param {string | number} name If set, register `Property scope observer`. If not, register `Global scope observer`.
     * @param {boolean} CallSetup If set as true, `callback` will be called after register observer. Default is `true`.
     */
    public Observe(callback: ObservableCallback, name?: string | number, CallSetup: boolean = true): Observable {
        if (!name) name = "*";

        if (!(name in this._callbacks)) this._callbacks[name] = [];
        this._callbacks[name].push(callback);
        return this;
    }

    private PropertyChanged(name: string | number, value: any, oldValue: any): void {
        if (name in this._callbacks)
            fns(this._callbacks[name], name, value, oldValue);
    }
}
/** Will be called when registered property has set. */
export interface ObservableCallback {
    (name: string, value: any): void;
}