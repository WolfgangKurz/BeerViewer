import cluster from "cluster";
import child_process from "child_process";

import fs from "fs";
import path from "path";

import uuid from "uuid/v1";
import { Session, remote, ipcRenderer, ipcMain, BrowserWindow } from "electron";

import IDisposable from "@/Base/IDisposable";

import Storage from "@/System/Storage";
import { ProxyModifiableCallback, ProxyCallback, ProxyWorkerMessage, ProxyWorkerResponse, ProxyRequest, ProxyResponse, ProxyPort } from "./Proxy.Define";

/**
 * Local proxy server to intercept game packets.
 */
export default class Proxy implements IDisposable {
	/** Single-tone instance */
	public static get Instance(): Proxy {
		if (!this.pInstance) this.pInstance = new this();
		return this.pInstance;
	}

	/** Instance object */
	private static pInstance?: Proxy;

	/** Is disposed? */
	public Disposed: boolean = false;

	/** Is worker created? */
	private pWorkerCreated: boolean = false;
	/** Cluster instance */
	// private pCluster?: cluster.Worker;
	private pCluster?: child_process.ChildProcess;

	/** Modifiable handlers array */
	private ModifiableMap: Array<{
		url: string;
		callback: ProxyModifiableCallback;
	}> = [];

	private constructor() { }

	/** Empty function to initialize instance */
	public Empty(): void {
		return void(0);
	}

	/**
	 * Setup and start proxy server
	 */
	public SetupProxyServer() {
		if (this.Disposed) throw new Error("[Proxy:SetupProxyServer] Disposed");
		if (cluster.isWorker) throw new Error("[Proxy:SetupProxyServer] Cannot setup proxy server on worker");
		if (this.pWorkerCreated) throw new Error("[Proxy:SetupProxyServer] Proxy server already running");

		/** Register event handler on Register event received from game-system */
		ipcMain.on("ProxyWorker.Register", this.ipcHandler);

		/** Pork new child process with ProxyWorker script */
		this.pCluster = child_process.fork(
			path.join(__static, "ProxyWorker.js")
		);

		// Register handler for received packet from worker
		this.pCluster.on("message", this.ClusterHandler);

		// set Worker created
		this.pWorkerCreated = true;
	}

	/**
	 * Setup endpoint.
	 *
	 * Set Electron proxy configuration.
	 * @param session Electron session
	 * @param callback Callback function after Endpoint has set, can be `undefined`
	 */
	public SetupEndpoint(session: Session | undefined, callback?: () => void) {
		if (this.Disposed) throw new Error("[Proxy:SetupEndpoint] Disposed");
		if (!session) throw new Error("[Proxy:SetupEndpoint] Session is undefined");

		session.setProxy(
			{
				pacScript: "",
				proxyRules: `http=127.0.0.1:${ProxyPort}`,
				proxyBypassRules: ""
			},
			() => {
				if (callback)
					callback();
			}
		);
	}

	/**
	 * Dispose Proxy.
	 *
	 * Shutdown worker cluster, remove all handlers, remove single-tone instance.
	 */
	public Dispose() {
		if (this.Disposed) throw new Error("[Proxy:Dispose] Disposed");
		this.Disposed = true;

		if (this.pWorkerCreated) {
			ipcMain.removeListener("ProxyWorker.Register", this.ipcHandler);
		}

		if (this.pCluster) {
			this.pCluster.off("message", this.ClusterHandler);
			this.pCluster.kill();
			this.pCluster = undefined;
		}

		Proxy.pInstance = undefined;
	}

	/**
	 * Registers handler for received packet of specific url.
	 * @param url URL string or array to observe
	 * @param callback Handler for received packet
	 */
	public Register(url: string | string[], callback: ProxyCallback | null): void {
		// Call inner register function, to hide IPCId parameter (internal use only parameter)
		this.InternalRegister(url, callback, null);
	}

	/**
	 * Registers handler for received packet of specific url, available to modify.
	 * @param url URL string or array to observe
	 * @param callback Handler for received packet, returns modified response body
	 */
	public RegisterModifiable(url: string | string[], callback: ProxyModifiableCallback): void {
		if (this.Disposed) throw new Error("[Proxy:RegisterModifiable] Disposed");
		if (cluster.isWorker) throw new Error("[Proxy:RegisterModifiable] Modifiable can register on main process only");
		if (!this.pCluster) throw new Error("[Proxy:RegisterModifiable] Cluster not ready yet");

		if (Array.isArray(url)) {
			// Register all urls
			url.forEach((x) => this.ModifiableMap.push({
				url: x,
				callback
			}));
		} else {
			// Register url
			this.ModifiableMap.push({
				url,
				callback
			});
		}
	}

	/**
	 * Registers network packet received handler.
	 * @param $event Event of handler, unused
	 * @param url URL of request
	 * @param ipdId IPC id between Worker and Master.
	 */
	private ipcHandler = ($event: Event, url: string | string[], ipdId: string) => {
		console.debug(`[ProxyWorker:Register] <Main> "ProxyWorker.${ipdId}" received, call register, for ${url}`);

		if (Array.isArray(url)) { // Type guard
			this.InternalRegister(url, () => void (0), ipdId);
		} else {
			this.InternalRegister(url, () => void (0), ipdId);
		}
	}

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
		this.ModifiableMap.forEach((x) => {
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
		if (!this.pCluster || this.pCluster.killed/*isDead()*/) return;

		// Pass back to Worker, to pass original requester(browser)
		this.pCluster.send({
			sender: "Proxy",
			callbackId: message.callbackId,
			response: { response: responseBuffer.toString("base64") } // Encode to Base64
		} as ProxyWorkerResponse);
	}

	/**
	 * Registers handler for received packet of specific urls.
	 * @param url URL string or array to observe
	 * @param callback Handler for received packet
	 * @param IPCId IPC id between Master and Worker, internal only
	 */
	private InternalRegister(url: string | string[], callback: ProxyCallback | null, IPCId: string | null = null): void {
		if (this.Disposed) throw new Error("[Proxy:Register] Disposed");

		// Is in Renderer process?
		if (remote) {
			/*
			 * If in Renderer process, pass register information to Main process.
			 * Worker pass packet to Main process, Main process pass packet to Renderer process handler,
			 *  and Renderer process handler pass back to Register caller.
			 */

			const ipcId = uuid(); // Unique IPC id
			console.debug(`[ProxyWorker:Register] <Renderer> "ProxyWorker.${ipcId}" listened, send to main process, for ${url}`);

			// Register renderer process handler
			ipcRenderer.on(`ProxyWorker.${ipcId}`, (e: Event, req: ProxyRequest, resp: ProxyResponse) => {
				// Main process has sent packet data to Renderer process handler.
				// This renderer process handler pass back to Request caller.
				console.debug(`[ProxyWorker:Register] <Renderer> "ProxyWorker.${ipcId}" callback received, for ${url}`);

				// Response was encoded as Base64, so decode
				const decodedResp = Buffer.from(resp.response, "base64");
				if (callback) callback(req, decodedResp); // Pass back
			});

			// Register renderer process handler
			ipcRenderer.send("ProxyWorker.Register", url, ipcId);

		} else {
			// Is in Main process?

			if (!this.pCluster) throw new Error("[Proxy:Register] Cluster not ready yet");

			// Packet received
			this.pCluster.on("message", (message: ProxyWorkerMessage) => {
				if (message.sender !== "ProxyWorker") return; // Not from Proxy worker
				if (message.type !== "AfterResponse") return; // For after response (not-modifiable, after modify) only
				if (message.request.pathname !== url) return; // Not matched url

				try {
					// Is IPC id available? (Is registerer in Renderer process?)
					if (IPCId !== null) {
						console.debug(`[ProxyWorker:Register] <Main> "ProxyWorker.${IPCId}" callback fire, for ${url}`);

						// Pass response to Renderer process
						Storage.get<BrowserWindow>("AppMainWindow")
							.webContents.send(`ProxyWorker.${IPCId}`, message.request, message.response);
					} else {
						if (!callback) throw new Error("[Proxy:Register] callback is not callable");

						// Processed response, encoded as Base64
						const respBase64 = message.response.response;
						const respBuffer = Buffer.from(respBase64, "base64"); // Decode

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
}
