/**
 * Base of wrapper classes that wrapped `kcsapi` json data.
 */
export default class KanColleWrapperBase<T> {
	protected get RawData(): T {
		return this.internalRawData;
	}
	private internalRawData: T;

	constructor(RawData: T) {
		this.internalRawData = RawData;
	}

	protected UpdateRawData(RawData: T): void {
		this.internalRawData = RawData;
	}
}
