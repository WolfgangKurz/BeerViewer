import IKcsApi from "./Interfaces/IKcsApi";
import { request } from "http";

export type ParseSingleCallback<T> = (response: T) => void;
export type ParsePairCallback<T, U> = (response: T, request: U) => void;

/**
 * Parse `kcsapi` response json string to data object.
 */
export function ParseKcsApi<T>(response: Buffer, callback: (response: T) => void): void;

/**
 * Parse `kcsapi` response json string to data object, request data to object.
 */
export function ParseKcsApi<T, U>(response: Buffer, request: Buffer | undefined, callback: ParsePairCallback<T, U>): void;

// tslint:disable-next-line:no-shadowed-variable
export function ParseKcsApi<T, U>(response: Buffer, request: Buffer | ParseSingleCallback<T> | undefined, callback?: ParsePairCallback<T, U>): void {
	// kcsapi response starts with "svdata=", but check and process for compatibility.
	const resp = (() => {
		const _ = response.toString();
		if (_.startsWith("svdata=")) return _.substr(7);
		return _;
	})();

	const kcsapi: IKcsApi = JSON.parse(resp) as IKcsApi;
	if (kcsapi.api_result !== 1) return;
	// throw new Error("Failed to parse kcsapi, result was " + kcsapi.api_result);

	if (callback) {
		const req: any = request ? JSON.parse(request.toString()) : {};
		callback(kcsapi.api_data as T, req as U);
	} else if (typeof request === "function") {
		request(kcsapi.api_data as T);
	}
}
