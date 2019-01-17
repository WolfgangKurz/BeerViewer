class MasterWrapper<T> implements ReadonlyMap<number, T>{
    forEach(callbackfn: (value: T, key: number, map: ReadonlyMap<number, T>) => void, thisArg?: any): void {
        throw new Error("Method not implemented.");
    } get(key: number): T | undefined {
        throw new Error("Method not implemented.");
    }
    has(key: number): boolean {
        throw new Error("Method not implemented.");
    }
    public get size(): number { return 0 }
    [Symbol.iterator](): IterableIterator<[number, T]> {
        throw new Error("Method not implemented.");
    }
    entries(): IterableIterator<[number, T]> {
        throw new Error("Method not implemented.");
    }
    keys(): IterableIterator<number> {
        throw new Error("Method not implemented.");
    }
    values(): IterableIterator<T> {
        throw new Error("Method not implemented.");
    }

    valueArray(): T[] {
        const output: T[] = [];
        this.forEach(x => output.push(x));
        return output;
    }
}
export { // Alias
    MasterWrapper as MasterWrapper,
    MasterWrapper as MasterTable
}

class IdentifiableWrapper<T> implements Map<number, T>{
    clear(): void {
        throw new Error("Method not implemented.");
    }
    delete(key: number): boolean {
        throw new Error("Method not implemented.");
    }
    forEach(callbackfn: (value: T, key: number, map: Map<number, T>) => void, thisArg?: any): void {
        throw new Error("Method not implemented.");
    }
    get(key: number): T | undefined {
        throw new Error("Method not implemented.");
    }
    has(key: number): boolean {
        throw new Error("Method not implemented.");
    }
    set(key: number, value: T): this {
        throw new Error("Method not implemented.");
    }
    public get size(): number { return 0 }
    [Symbol.iterator](): IterableIterator<[number, T]> {
        throw new Error("Method not implemented.");
    }
    entries(): IterableIterator<[number, T]> {
        throw new Error("Method not implemented.");
    }
    keys(): IterableIterator<number> {
        throw new Error("Method not implemented.");
    }
    values(): IterableIterator<T> {
        throw new Error("Method not implemented.");
    }
    [Symbol.toStringTag]: string;

    valueArray(): T[] {
        const output: T[] = [];
        this.forEach(x => output.push(x));
        return output;
    }
}
export {
    IdentifiableWrapper as IdentifiableWrapper,
    IdentifiableWrapper as IdentifiableTable
}