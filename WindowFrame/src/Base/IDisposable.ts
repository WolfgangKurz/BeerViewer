/**
 * Define class as disposable
 */
export default interface IDisposable {
	/** Has class disposed? */
	Disposed: boolean;

	/** Dispose class */
	Dispose(): void;
}
