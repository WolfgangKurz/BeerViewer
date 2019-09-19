/**
 * Contains readonly `Minimum` and `Maximum` value pair.
 *
 * If need new value pair, need to use `new` keyword.
 */
export default class MinMaxPair {
	private data: [number, number];

	constructor(data: [number, number]) {
		this.data = data;
	}

	public get Miminum(): number {
		return this.data[0];
	}
	public get Maximum(): number {
		return this.data[1];
	}
}
