import { IIdentifiable } from "System/Base/Interfaces/IIdentifiable";

class MasterWrapper<T extends IIdentifiable> {
    protected _map: Map<number, T> = new Map<number, T>();

    get size(): number { return this._map.size }

    constructor(data?: T[]) {
        if (data)
            data.forEach(x => this._map.set(x.Id, x));
    }

    forEach(callbackfn: (value: T, key: number, map: MasterWrapper<T>) => void, thisArg?: any): void {
        this._map.forEach((v, k) => callbackfn.call(thisArg, v, k, this));
    }
    get(key: number): T | undefined {
        return this._map.get(key);
    }
    has(key: number): boolean {
        return this._map.has(key);
    }
    *[Symbol.iterator](): IterableIterator<[number, T]> {
        for (const entry of this._map)
            yield entry;
    }
    entries(): [number, T][] {
        const output: [number, T][] = [];
        for (const entry of this) output.push(entry);
        return output;
    }
    keys(): number[] {
        const output: number[] = [];
        for (const entry of this) output.push(entry[0]);
        return output;
    }
    values(): T[] {
        const output: T[] = [];
        for (const entry of this) output.push(entry[1]);
        return output;
    }
    get [Symbol.toStringTag](): string { return "MasterWrapper" }
}
export { // Alias
    MasterWrapper as MasterWrapper,
    MasterWrapper as MasterTable
}

class IdentifiableWrapper<T extends IIdentifiable> extends MasterWrapper<T> {
    constructor(data?: T[]) {
        super(data);
    }

    clear(): void {
        this._map.clear();
    }
    delete(key: number): boolean {
        return this._map.delete(key);
    }
    set(key: number, value: T): this {
        this._map.set(key, value);
        return this;
    }
    get [Symbol.toStringTag](): string { return "IdentifiableWrapper" }
}
export {
    IdentifiableWrapper as IdentifiableWrapper,
    IdentifiableWrapper as IdentifiableTable
}