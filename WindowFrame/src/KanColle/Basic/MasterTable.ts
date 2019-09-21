import IIdentifiable from "@KC/Interfaces/IIdentifiable";

type KeyValuePair<T extends IIdentifiable> = [number, T];

/**
 * Readonly and `<data.Id, data>` typed `Map`.
 *
 * `data` should be extended with `IIdentifiable`.
 */
export default class MasterTable<T extends IIdentifiable> {
	public static readonly Empty = new MasterTable([]);

	protected map: Map<number, T>;

	constructor(entries: T[]) {
		this.map = new Map<number, T>(entries.map((x) => [x.api_id, x]));
	}

	public forEach(callbackfn: (value: T, key: number, map: MasterTable<T>) => void, thisArg?: any): void {
		this.map.forEach((v, k) => callbackfn.call(thisArg, v, k, this));
	}
	public get(key: number): T | undefined {
		return this.map.get(key);
	}
	public has(key: number): boolean {
		return this.map.has(key);
	}
	public *[Symbol.iterator](): IterableIterator<[number, T]> {
		for (const entry of this.map)
			yield entry;
	}
	public entries(): Array<KeyValuePair<T>> {
		const output: Array<KeyValuePair<T>> = [];
		for (const entry of this) output.push(entry);
		return output;
	}
	public keys(): number[] {
		const output: number[] = [];
		for (const entry of this) output.push(entry[0]);
		return output;
	}
	public values(): T[] {
		const output: T[] = [];
		for (const entry of this) output.push(entry[1]);
		return output;
	}
	public get [Symbol.toStringTag](): string { return "MasterTable"; }
}
