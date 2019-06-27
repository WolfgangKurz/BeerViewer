import cluster from "cluster";
import crypto from "crypto";
import os from "os"
import { Session, remote, ipcRenderer, ipcMain, BrowserWindow } from "electron";
import Storage from "../System/Storage";

type ProxyCallback = (request: ProxyRequest, response: ProxyResponse) => void;
type ProxyModifiableCallback = (request: ProxyRequest, response: ProxyResponse) => string;

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
interface ProxyResponse {
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

export default class Proxy {
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
		if (cluster.isWorker) throw "[Proxy:SetupProxyServer] Cannot setup proxy server on worker";
		if (this._WorkerCreated) throw "[Proxy:SetupProxyServer] Proxy server already running";

		ipcMain.on("ProxyWorker.Register", (_: Event, url: string | string[], ipc_id: string) => {
			console.log(`[ProxyWorker:Register] <Main> "ProxyWorker.${ipc_id}" received, call register, for ${url}`);
			if (Array.isArray(url)) {
				this.Register(url, () => { }, ipc_id);
			} else {
				this.Register(url, () => { }, ipc_id);
			}
		});

		this._WorkerCreated = true;
		//os.cpus().forEach(x => {
		this._Cluster = cluster.fork({
			ClusterType: "ProxyWorker"
		});
		//});

		this._Cluster.on("message", (message: ProxyWorkerMessage) => {
			if (message.sender !== "ProxyWorker") return; // Not from Proxy worker
			if (message.type !== "BeforeResponse") return; // For after response (not-modifiable, after modify) only
			if (!message.callbackId) return; // Not callbackable

			let resp = message.response.response;
			this.ModifiableMap.forEach(x => {
				try {
					if (message.request.pathname !== x.url) return; // Not matched url

					resp = x.callback(message.request, { response: resp });
				} catch (e) { } // Dismiss
			});
			this._Cluster!.send(<ProxyWorkerResponse>{
				sender: "Proxy",
				callbackId: message.callbackId,
				response: { response: resp }
			});
		});
	}

	public SetupEndpoint(session: Session | undefined) {
		if (!session) throw "[ProxyBase:Setup] Session is undefined";
		session.setProxy({
			pacScript: "",
			proxyRules: `http=127.0.0.1:${Proxy.Port}`,
			proxyBypassRules: ""
		}, () => { });
	}

	public Register(url: string, callback: ProxyCallback, IPCId?: string): void;
	public Register(urls: string[], callback: ProxyCallback, IPCId?: string): void;
	public Register(url: string | string[], callback: ProxyCallback | null, IPCId: string | null = null): void {
		if (remote) { // in Renderer
			let ipc_id = crypto.randomBytes(16).toString("hex");

			console.log(`[ProxyWorker:Register] <Renderer> "ProxyWorker.${ipc_id}" listened, send to main process, for ${url}`);
			ipcRenderer.on(`ProxyWorker.${ipc_id}`, (e: Event, req: ProxyRequest, resp: ProxyResponse) => {
				console.log(`[ProxyWorker:Register] <Renderer> "ProxyWorker.${ipc_id}" callback received, for ${url}`);
				callback && callback(req, resp);
			});
			ipcRenderer.send("ProxyWorker.Register", url, ipc_id);
		} else {
			if (!this._Cluster) throw "[Proxy:Register] Cluster not ready yet";

			this._Cluster.on("message", (message: ProxyWorkerMessage) => {
				if (message.sender !== "ProxyWorker") return; // Not from Proxy worker
				if (message.type !== "AfterResponse") return; // For after response (not-modifiable, after modify) only
				if (message.request.pathname !== url) return; // Not matched url

				try {
					if (IPCId !== null) {
						console.log(`[ProxyWorker:Register] <Main> "ProxyWorker.${IPCId}" callback fire, for ${url}`);

						Storage.get<BrowserWindow>("AppMainWindow")
							.webContents.send(`ProxyWorker.${IPCId}`, message.request, message.response);
					} else {
						if (!callback) throw "[Proxy:Register] callback is not callable";
						callback(message.request, message.response);
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