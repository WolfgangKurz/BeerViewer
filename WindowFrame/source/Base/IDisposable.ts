export default interface IDisposable {
	Disposed: boolean;
	Dispose(): void;
}