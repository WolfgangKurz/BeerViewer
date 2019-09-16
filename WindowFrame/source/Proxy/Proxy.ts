import cluster from "cluster";
import uuid from "uuid/v1";
import { Session, remote, ipcRenderer, ipcMain, BrowserWindow } from "electron";
import Storage from "../System/Storage";
import IDisposable from "../Base/IDisposable";

/**
 * @param {ProxyRequest} request Request data
 * @param {Buffer} response Response data, as binary
 */
type ProxyCallback = (request: ProxyRequest, response: Buffer) => void;

/**
 * @param {ProxyRequest} request Request data
 * @param {Buffer} response Response data, as binary
 * @returns {Buffer} Modified response data, as binary
 */
type ProxyModifiableCallback = (request: ProxyRequest, response: Buffer) => Buffer;

/**
 * Request information
 */
export interface ProxyRequest {
	/** HTTP request method */
	method: "GET" | "POST";

	/** Host of request, `hostname:port` */
	host: string;

	/** Hostname of request */
	hostname: string;
	/** Port of request */
	port: number | string;

	/** Path of request, `pathname?query` */
	path: string;
	/** Pathname of request */
	pathname: string;
	/** Query string of request */
	query: {
		[key: string]: string | string[];
	};
}

/**
 * Response of Proxy, contains base64-encoded body.
 */
interface ProxyResponse {
	/** Base64 encoded string */
	response: string;
}

/**
 * ProxyWorker packet interface contains information to process.
 */
export interface ProxyWorkerPacket {
	sender?: string;
	callbackId?: string;
	response: ProxyResponse;
}
/**
 * ProxyWorker message packet, sent from Worker.
 */
export interface ProxyWorkerMessage extends ProxyWorkerPacket {
	sender?: "ProxyWorker";

	type: "BeforeResponse" | "AfterResponse";
	request: ProxyRequest;
}
/**
 * ProxyWorker response packet, sent from Master.
 */
export interface ProxyWorkerResponse extends ProxyWorkerPacket {
	sender?: "Proxy";
	callbackId: string;
}

/**
 * Local proxy server to intercept game packets.
 */
export default class Proxy implements IDisposable {
	/** Is disposed? */
	Disposed: boolean = false;

	/** Single-tone instance */
	public static get Instance(): Proxy {
		if (!this._Instnace) this._Instnace = new this();
		return this._Instnace;
	}
	private static _Instnace?: Proxy;

	/** Port of local proxy server */
	public static get Port(): number { return 49217 }

	private constructor() { }

	/** Is worker created? */
	private _WorkerCreated: boolean = false;
	/** Cluster instance */
	private _Cluster?: cluster.Worker;

	/**
	 * Setup and start proxy server
	 */
	public SetupProxyServer() {
		if (this.Disposed) throw "[Proxy:SetupProxyServer] Disposed";
		if (cluster.isWorker) throw "[Proxy:SetupProxyServer] Cannot setup proxy server on worker";
		if (this._WorkerCreated) throw "[Proxy:SetupProxyServer] Proxy server already running";

		/** Register event handler on Register event received from game-system */
		ipcMain.on("ProxyWorker.Register", this.ipcHandler);

		/* // Create clusters as many as CPUs
		os.cpus().forEach(x => {
			this._Cluster = cluster.fork({
				ClusterType: "ProxyWorker"
			});
		});
		*/
		this._Cluster = cluster.fork({
			ClusterType: "ProxyWorker"
		});

		// Register handler for received packet from worker
		this._Cluster.on("message", this.ClusterHandler);

		// set Worker created
		this._WorkerCreated = true;
	}

	/**
	 * Registers network packet received handler.
	 * @param $event Event of handler, unused
	 * @param url URL of request
	 * @param ipc_id IPC id between Worker and Master.
	 */
	private ipcHandler = ($event: Event, url: string | string[], ipc_id: string) => {
		console.debug(`[ProxyWorker:Register] <Main> "ProxyWorker.${ipc_id}" received, call register, for ${url}`);

		if (Array.isArray(url)) { // Type guard
			this.InternalRegister(url, () => { }, ipc_id);
		} else {
			this.InternalRegister(url, () => { }, ipc_id);
		}
	};

	/**
	 * Handler for received packet from worker.
	 *
	 * Worker sent, Master received.
	 * @param message Received message data
	 */
	private ClusterHandler = (message: ProxyWorkerMessage) => {
		if (message.sender !== "ProxyWorker") return; // Not from Proxy worker
		if (message.type !== "BeforeResponse") return; // For before response, modifiable only
		if (!message.callbackId) return; // Not callbackable

		// Response is base64-encoded, so decode
		let responseBuffer = Buffer.from(message.response.response, "base64");

		// Call modifiable handlers
		this.ModifiableMap.forEach(x => {
			try {
				if (message.request.pathname !== x.url) return; // Target URL not matches

				// Call modifier and apply to response
				responseBuffer = x.callback(message.request, responseBuffer);
			} catch (e) {
				// Dismiss all exceptions, just print to console
				console.error(e);
			}
		});

		// Escape when Cluster not available
		if (!this._Cluster || this._Cluster.isDead()) return;

		// Pass back to Worker, to pass original requester(browser)
		this._Cluster.send(<ProxyWorkerResponse>{
			sender: "Proxy",
			callbackId: message.callbackId,
			response: { response: responseBuffer.toString("base64") } // Encode to Base64
		});
	};

	/**
	 * Setup endpoint.
	 *
	 * Set Electron proxy configuration.
	 * @param session Electron session
	 * @param callback Callback function after Endpoint has set, can be `undefined`
	 */
	public SetupEndpoint(session: Session | undefined, callback?: Function) {
		if (this.Disposed) throw "[Proxy:SetupEndpoint] Disposed";
		if (!session) throw "[Proxy:SetupEndpoint] Session is undefined";

		session.setProxy({
			pacScript: "",
			proxyRules: `http=127.0.0.1:${Proxy.Port}`,
			proxyBypassRules: ""
		}, () => (callback && callback()));
	}

	/**
	 * Dispose Proxy.
	 * 
	 * Shutdown worker cluster, remove all handlers, remove single-tone instance.
	 */
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

	/**
	 * Registers handler for received packet of specific url.
	 * @param url URL to observe
	 * @param callback Handler for received packet
	 */
	public Register(url: string, callback: ProxyCallback): void;
	/**
	 * Registers handler for received packet of specific urls.
	 * @param urls URL array to observe
	 * @param callback Handler for received packet
	 */
	public Register(urls: string[], callback: ProxyCallback): void;
	public Register(url: string | string[], callback: ProxyCallback | null): void {
		// Call inner register function, to hide IPCId parameter (internal use only parameter)
		this.InternalRegister(url, callback, null);
	}

	/**
	 * Registers handler for received packet of specific urls.
	 * @param url URL string or array to observe
	 * @param callback Handler for received packet
	 * @param IPCId IPC id between Master and Worker, internal only
	 */
	private InternalRegister(url: string | string[], callback: ProxyCallback | null, IPCId: string | null = null): void {
		if (this.Disposed) throw "[Proxy:Register] Disposed";

		// Is in Renderer process?
		if (remote) {
			/*
			 * If in Renderer process, pass register information to Main process.
			 * Worker pass packet to Main process, Main process pass packet to Renderer process handler,
			 *  and Renderer process handler pass back to Register caller.
			 */

			let ipc_id = uuid(); // Unique IPC id
			console.debug(`[ProxyWorker:Register] <Renderer> "ProxyWorker.${ipc_id}" listened, send to main process, for ${url}`);

			// Register renderer process handler
			ipcRenderer.on(`ProxyWorker.${ipc_id}`, (e: Event, req: ProxyRequest, resp: ProxyResponse) => {
				// Main process has sent packet data to Renderer process handler.
				// This renderer process handler pass back to Request caller.
				console.debug(`[ProxyWorker:Register] <Renderer> "ProxyWorker.${ipc_id}" callback received, for ${url}`);

				// Response was encoded as Base64, so decode
				const _resp = Buffer.from(resp.response, "base64");
				callback && callback(req, _resp); // Pass back
			});

			// Register renderer process handler
			ipcRenderer.send("ProxyWorker.Register", url, ipc_id);

		} else {
			// Is in Main process?

			if (!this._Cluster) throw "[Proxy:Register] Cluster not ready yet";

			// Packet received
			this._Cluster.on("message", (message: ProxyWorkerMessage) => {
				if (message.sender !== "ProxyWorker") return; // Not from Proxy worker
				if (message.type !== "AfterResponse") return; // For after response (not-modifiable, after modify) only
				if (message.request.pathname !== url) return; // Not matched url

				// Processed response, encoded as Base64
				const respBase64 = message.response.response;
				const respBuffer = Buffer.from(respBase64, "base64"); // Decode

				try {
					// Is IPC id available? (Is registerer in Renderer process?)
					if (IPCId !== null) {
						console.debug(`[ProxyWorker:Register] <Main> "ProxyWorker.${IPCId}" callback fire, for ${url}`);

						// Pass response to Renderer process
						Storage.get<BrowserWindow>("AppMainWindow")
							.webContents.send(`ProxyWorker.${IPCId}`, message.request, respBase64);
					} else {
						if (!callback) throw "[Proxy:Register] callback is not callable";

						// Internally registered callback, call callback directly
						callback(message.request, respBuffer);
					}
				} catch (e) {
					// Dismiss all exceptions, just print to console
					console.error(e);
				}
			});
		}
	}

	/** Modifiable handlers array */
	private ModifiableMap: {
		url: string;
		callback: ProxyModifiableCallback;
	}[] = [];

	/**
	 * Registers handler for received packet of specific url, available to modify.
	 * @param url URL to observe
	 * @param callback Handler for received packet, returns modified response body
	 */
	public RegisterModifiable(url: string, callback: ProxyModifiableCallback): void;
	/**
	 * Registers handler for received packet of specific urls, available to modify.
	 * @param urls URL array to observe
	 * @param callback Handler for received packet, returns modified response body
	 */
	public RegisterModifiable(urls: string[], callback: ProxyModifiableCallback): void;
	/**
	 * Registers handler for received packet of specific url, available to modify.
	 * @param url URL string or array to observe
	 * @param callback Handler for received packet, returns modified response body
	 */
	public RegisterModifiable(url: string | string[], callback: ProxyModifiableCallback): void {
		if (this.Disposed) throw "[Proxy:RegisterModifiable] Disposed";
		if (cluster.isWorker) throw "[Proxy:RegisterModifiable] Modifiable can register on main process only";
		if (!this._Cluster) throw "[Proxy:RegisterModifiable] Cluster not ready yet";

		if (Array.isArray(url)) {
			// Register all urls
			url.forEach(x => this.ModifiableMap.push({
				url: x,
				callback: callback
			}));
		} else {
			// Register url
			this.ModifiableMap.push({
				url: url,
				callback: callback
			});
		}
	}
}