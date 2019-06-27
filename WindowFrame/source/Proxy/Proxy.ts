import cluster from "cluster";
import uuid from "uuid/v1";
import { Session, remote, ipcRenderer, ipcMain, BrowserWindow } from "electron";
import Storage from "../System/Storage";
import IDisposable from "../Base/IDisposable";

/**
 * @param {string} response Binary
 */
type ProxyCallback = (request: ProxyRequest, response: Buffer) => void;

/**
 * @param {string} response Binary
 * @returns {string} Binary
 */
type ProxyModifiableCallback = (request: ProxyRequest, response: Buffer) => Buffer;

interface ProxyRequest {
	method: string; // "GET" or "POST"

	host: string;
	hostname: string;
	port: number | string;

	path: string;
	pathname: string;
	query: {
		[key: string]: string | string[];
	};
}
/**
 * @property {string} response Base64 encoded string
 */
interface ProxyResponse {
	/** Base64 encoded string */
	response: string;
}
export interface ProxyWorkerMessage {
	sender?: "ProxyWorker";
	callbackId?: string;

	type: "BeforeResponse" | "AfterResponse";
	request: ProxyRequest;
	response: ProxyResponse;
}
export interface ProxyWorkerResponse {
	sender?: "Proxy";
	callbackId: string;
	response: ProxyResponse;
}

export default class Proxy implements IDisposable {
	Disposed: boolean = false;

	private static _Instnace?: Proxy;
	public static get Instance(): Proxy {
		if (!this._Instnace) this._Instnace = new this();
		return this._Instnace;
	}

	public static get Port(): number { return 49217 }

	private constructor() {
	}

	private _WorkerCreated: boolean = false;
	private _Cluster?: cluster.Worker;


	public SetupProxyServer() {
		if (this.Disposed) throw "[Proxy:SetupProxyServer] Disposed";
		if (cluster.isWorker) throw "[Proxy:SetupProxyServer] Cannot setup proxy server on worker";
		if (this._WorkerCreated) throw "[Proxy:SetupProxyServer] Proxy server already running";

		this._WorkerCreated = true;

		ipcMain.on("ProxyWorker.Register", this.ipcHandler);

		//os.cpus().forEach(x => {
		this._Cluster = cluster.fork({
			ClusterType: "ProxyWorker"
		});

		//});

		this._Cluster.on("message", this.ClusterHandler);
	}
	private ipcHandler = (_: Event, url: string | string[], ipc_id: string) => {
		console.debug(`[ProxyWorker:Register] <Main> "ProxyWorker.${ipc_id}" received, call register, for ${url}`);
		if (Array.isArray(url)) {
			this.Register(url, () => { }, ipc_id);
		} else {
			this.Register(url, () => { }, ipc_id);
		}
	};
	private ClusterHandler = (message: ProxyWorkerMessage) => {
		/*  Worker -> Master  */
		if (message.sender !== "ProxyWorker") return; // Not from Proxy worker
		if (message.type !== "BeforeResponse") return; // For before response, modifiable only
		if (!message.callbackId) return; // Not callbackable

		let responseBuffer = Buffer.from(message.response.response, "base64");
		this.ModifiableMap.forEach(x => {
			try {
				if (message.request.pathname !== x.url) return; // Not matched url

				responseBuffer = x.callback(message.request, responseBuffer);
			} catch (e) { } // Dismiss
		});

		if (!this._Cluster || this._Cluster.isDead()) return;

		// Master -> Worker
		this._Cluster.send(<ProxyWorkerResponse>{
			sender: "Proxy",
			callbackId: message.callbackId,
			response: { response: responseBuffer.toString("base64") } // Encode to Base64
		});
	};

	public SetupEndpoint(session: Session | undefined, callback?: Function) {
		if (this.Disposed) throw "[Proxy:SetupEndpoint] Disposed";
		if (!session) throw "[Proxy:SetupEndpoint] Session is undefined";

		session.setProxy({
			pacScript: "",
			proxyRules: `http=127.0.0.1:${Proxy.Port}`,
			proxyBypassRules: ""
		}, () => (callback && callback()));
	}

	public Dispose() {
		if (this.Disposed) throw "[Proxy:Dispose] Disposed";
		this.Disposed = true;

		if (this._WorkerCreated) {
			ipcMain.removeListener("ProxyWorker.Register", this.ipcHandler);
		}

		if (this._Cluster) {
			this._Cluster.off("message", this.ClusterHandler);
			this._Cluster.kill();
			this._Cluster = undefined;
		}

		Proxy._Instnace = undefined;
	}

	public Register(url: string, callback: ProxyCallback, IPCId?: string): void;
	public Register(urls: string[], callback: ProxyCallback, IPCId?: string): void;
	public Register(url: string | string[], callback: ProxyCallback | null, IPCId: string | null = null): void {
		if (this.Disposed) throw "[Proxy:Register] Disposed";

		if (remote) { // in Renderer
			let ipc_id = uuid();
			console.debug(`[ProxyWorker:Register] <Renderer> "ProxyWorker.${ipc_id}" listened, send to main process, for ${url}`);

			ipcRenderer.on(`ProxyWorker.${ipc_id}`, (e: Event, req: ProxyRequest, resp: ProxyResponse) => {
				// [Worker -> Master] Register
				// [Master -> Worker] Callback << Here
				console.debug(`[ProxyWorker:Register] <Renderer> "ProxyWorker.${ipc_id}" callback received, for ${url}`);

				const _resp = Buffer.from(resp.response, "base64");
				callback && callback(req, _resp);
			});

			// [Worker -> Master] Register << Here
			// [Master -> Worker] Callback
			ipcRenderer.send("ProxyWorker.Register", url, ipc_id);
		} else {
			if (!this._Cluster) throw "[Proxy:Register] Cluster not ready yet";

			this._Cluster.on("message", (message: ProxyWorkerMessage) => {
				// Worker -> Master
				if (message.sender !== "ProxyWorker") return; // Not from Proxy worker
				if (message.type !== "AfterResponse") return; // For after response (not-modifiable, after modify) only
				if (message.request.pathname !== url) return; // Not matched url

				const respBase64 = message.response.response; // From message, base64 data
				const respBuffer = Buffer.from(respBase64, "base64");

				try {
					if (IPCId !== null) {
						console.debug(`[ProxyWorker:Register] <Main> "ProxyWorker.${IPCId}" callback fire, for ${url}`);

						Storage.get<BrowserWindow>("AppMainWindow")
							.webContents.send(`ProxyWorker.${IPCId}`, message.request, respBase64);
					} else {
						if (!callback) throw "[Proxy:Register] callback is not callable";
						callback(message.request, respBuffer);
					}
				} catch (e) { } // Dismiss
			})
		}
	}

	private ModifiableMap: {
		url: string;
		callback: ProxyModifiableCallback;
	}[] = [];
	public RegisterModifiable(url: string, callback: ProxyModifiableCallback): void;
	public RegisterModifiable(urls: string[], callback: ProxyModifiableCallback): void;
	public RegisterModifiable(url: string | string[], callback: ProxyModifiableCallback): void {
		if (this.Disposed) throw "[Proxy:RegisterModifiable] Disposed";
		if (cluster.isWorker) throw "[Proxy:RegisterModifiable] Modifiable can register on main process only";
		if (!this._Cluster) throw "[Proxy:RegisterModifiable] Cluster not ready yet";

		if (Array.isArray(url)) {
			url.forEach(x => this.ModifiableMap.push({
				url: x,
				callback: callback
			}));
		} else {
			this.ModifiableMap.push({
				url: url,
				callback: callback
			});
		}
	}
}