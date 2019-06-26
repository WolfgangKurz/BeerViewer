import cluster from "cluster";
import { Session, remote } from "electron";

type ProxyCallback = (request: ProxyRequest, response: ProxyResponse) => void;

interface ProxyRequest {
	method: string; // "GET" or "POST"

	host: string;
	hostname: string;
	port: number;

	path: string;
	pathname: string;
	query: {
		[key: string]: string | string[];
	};
}
interface ProxyResponse {
	response: string;
}
interface ProxyWorkerMessage {
	sender: "ProxyWorker",
	request: ProxyRequest,
	response: ProxyResponse;
}

export default class Proxy {
	private static _Instnace?: Proxy;
	public static get Instance(): Proxy {
		if (!this._Instnace) this._Instnace = new this();
		return this._Instnace;
	}

	public static get Port(): number { return 49217 }

	private constructor() { } // Cannot use constructor


	private _WorkerCreated: boolean = false;
	private _Cluster?: cluster.Worker;


	public SetupProxyServer() {
		if (cluster.isWorker) throw "[Proxy:SetupProxyServer] Cannot setup proxy server on worker";
		if (this._WorkerCreated) throw "[Proxy:SetupProxyServer] Proxy server already running";

		this._WorkerCreated = true;
		this._Cluster = cluster.fork({
			ClusterType: "ProxyWorker"
		});
	}

	public SetupEndpoint(session: Session | undefined) {
		if (!session) throw "[ProxyBase:Setup] Session is undefined";
		session.setProxy({
			pacScript: "",
			proxyRules: `http=localhost:${Proxy.Port}`,
			proxyBypassRules: ""
		}, () => { });
	}

	public Register(url: string, callback: ProxyCallback): void;
	public Register(urls: string[], callback: ProxyCallback): void;
	public Register(url: string | string[], callback: ProxyCallback): void {
		if (remote.getCurrentWindow) { // in Renderer

		} else {
			if (!this._Cluster) throw "[Proxy:Register] Cluster not ready yet";

			this._Cluster.on("message", (message: ProxyWorkerMessage) => {
				if(message.sender !== "ProxyWorker") return; // Not from Proxy worker

				callback(message.request, message.response);
			})
		}
	}
}