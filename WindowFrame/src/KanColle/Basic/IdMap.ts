import IIdentifiable from "@KC/Interfaces/IIdentifiable";

/**
 * Object interface that identifiable with `number` type key contains `T` type content.
 */
export default interface IdMap<T> {
	[key: number]: T;
}

export function ConvertToIdMap<T extends IIdentifiable>(data: T[]): IdMap<T>;
export function ConvertToIdMap<T>(data: T[], keyGetter?: (i: T) => number): IdMap<T>;

/**
 * Convert `T[]` to `IdMap<T>`
 * @param data Array to convert to IdMap object
 * @param keyGetter If data is not `IIdentifiable`, need to set custom `key`. If not set, default is index of array.
 * @returns Converted IdMap
 */
export function ConvertToIdMap<T>(data: T[], keyGetter?: (i: T) => number): IdMap<T> {
	const ret: IdMap<T> = {};

	if (keyGetter)
		data.forEach((x) => ret[keyGetter(x)] = x);
	else if (data.length > 0) {
		let identifiable: boolean = false;
		for (const i of data) {
			if ("api_id" in i) identifiable = true;
			break;
		}
		if (identifiable)
			data.forEach((x) => ret[(x as unknown as IIdentifiable).api_id] = x);
		else
			data.forEach((x, i) => ret[i] = x);
	}
	return ret;
}

/**
 * Get length of `IdMap<T>`
 * @param map IdMap object to measure length
 * @returns Length of map
 */
export function IdMapLength<T>(map: IdMap<T>): number {
	return Object.keys(map).length;
}

/**
 * Convert `IdMap<T>` to `T[]`
 * @param map IdMap object to convert to array
 * @returns Converted array
 */
export function IdMapArray<T>(map: IdMap<T>): T[] {
	return Object.keys(map).map((x) => (map as { [key: string]: T })[x]);
}
