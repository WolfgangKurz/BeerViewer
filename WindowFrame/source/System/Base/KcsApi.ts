import { HTTPRequest } from "../Exports/API";
import { SubscribeInfo } from "../Exports/API";

export interface KcsApiCallback<T, U extends HTTPRequest> {
    (Response: T, Request: U, RawJson: any): void;
}

/** Subscribe HTTP the url starts with "/kcsapi/" */
export function SubscribeKcsapi<T, U extends HTTPRequest = HTTPRequest>(url: string, callback: KcsApiCallback<T, U>): SubscribeInfo {
    return new SubscribeInfo(
        window.API.SubscribeHTTP("/kcsapi/" + url, (x: String, y) => {
            let svdata: String;
            let json: any;
            try {
                svdata = x.startsWith("svdata=") ? x.substr(7) : x;
                json = JSON.parse(svdata.toString());
            } catch (e) {
                console.warn("Expected json, but not.", e);
                return;
            }

            if (json.api_result && json.api_result === 1 && callback)
                callback(json.api_data, y as U, json);
        })
    );
}
