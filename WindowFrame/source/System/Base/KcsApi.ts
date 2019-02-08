import { HTTPRequest } from "System/Exports/API";
import { IDisposable } from "./Interfaces/IDisposable";

export interface KcsApiCallback<T, U extends HTTPRequest> {
	(Response: T, Request: U, RawJson: any): void;
}

/** Subscribe information */
export class SubscribeInfo<T, U extends HTTPRequest = HTTPRequest> implements IDisposable {
private _Disposed: boolean = false;
	public get Disposed(): boolean { return this._Disposed }

	private _Id: number = -1;
	public get Id(): number { return this._Id }

	private readonly _callback: KcsApiCallback<T, U>;

	constructor(url: string, callback: KcsApiCallback<T, U>) {
		this._callback = callback;

		window.API.SubscribeHTTP("/kcsapi/" + url, (x: string, y) => {
			if (!this._Disposed)
				this.Caller(x, new HTTPRequest(y));
			else // Something wrong?
				this.Dispose();
		}).then(x => this._Id = x);
	}

	private Caller(response: string, request: HTTPRequest): void {
		let svdata: string;
		let json: any;
		try {
			svdata = response.startsWith("svdata=") ? response.substr(7) : response;
			json = JSON.parse(svdata.toString());
		} catch (e) {
			console.warn("Expected json, but not.", e);
			return;
		}

		if (json.api_result && json.api_result === 1 && this._callback)
			this._callback(json.api_data, request as U, json);
	}

	Dispose(): void {
		if (this.Id >= 0) {
			window.API.UnsubscribeHTTP(this.Id)
				.then(x => !x && console.log(`Failed to Unsubscribe HTTP ${this.Id}`));
			this._Disposed = true;
		}
	}
}

/** Subscribe HTTP the url starts with "/kcsapi/" */
export function SubscribeKcsapi<T, U extends HTTPRequest = HTTPRequest>(url: string, callback: KcsApiCallback<T, U>): SubscribeInfo<T, U> {
	return new SubscribeInfo(url, callback);
}
