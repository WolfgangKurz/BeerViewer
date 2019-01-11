interface KcsApiCallback<T, U extends HTTPRequest> {
    (Response: T, Request: U): void;
}

/** Subscribe HTTP the url starts with "/kcsapi/" */
function SubscribeKcsapi<T, U extends HTTPRequest = HTTPRequest>(url: string, callback: KcsApiCallback<T, U>) {
    window.API.SubscribeHTTP("/kcsapi/" + url, (x: String, y) => {
        try {
            const svdata: String = x.startsWith("svdata=") ? x.substr(7) : x;
            const json = JSON.parse(svdata.toString());
            if (json.api_result === 1 && callback)
                callback(json.api_data, y as U);
        } catch (e) {
            console.warn("Expected json, but not.", e);
        }
    });
};