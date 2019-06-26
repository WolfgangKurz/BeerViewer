import cluster from "cluster";
import crypto from "crypto";
import os from "os"
import { Session, remote, ipcRenderer, ipcMain, BrowserWindow } from "electron";
import Storage from "../System/Storage";

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
	}

	public SetupEndpoint(session: Session | undefined) {
		if (!session) throw "[ProxyBase:Setup] Session is undefined";
		session.setProxy({
			pacScript: "",
			proxyRules: `http=localhost:${Proxy.Port}`,
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
}