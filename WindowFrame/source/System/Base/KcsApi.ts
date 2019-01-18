import { HTTPRequest } from "../Exports/API";
import { SubscribeInfo } from "../Exports/API.class";

export interface KcsApiCallback<T, U extends HTTPRequest> {
    (Response: T, Request: U, RawJson: any): void;
}

/** Subscribe HTTP the url starts with "/kcsapi/" */
export function SubscribeKcsapi<T, U extends HTTPRequest = HTTPRequest>(url: string, callback: KcsApiCallback<T, U>): SubscribeInfo {
    return new SubscribeInfo(
        window.API.SubscribeHTTP("/kcsapi/" + url, (x: String, y) => {
            try {
                const svdata: String = x.startsWith("svdata=") ? x.substr(7) : x;
                const json = JSON.parse(svdata.toString());
                if (json.api_result === 1 && callback)
                    callback(json.api_data, y as U, json);
            } catch (e) {
                console.warn("Expected json, but not.", e);
            }
        })
    );
}
