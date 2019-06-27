import http from "http";
import httpProxy from "http-proxy";
import net from "net";
import url from "url";
import uuid from "uuid/v1";
import Proxy, { ProxyWorkerMessage, ProxyWorkerResponse } from "./Proxy";

let proxy: httpProxy | undefined;
let server: net.Server | undefined;

process.on("SIGTERM", () => {
	console.debug("[ProxyWorker:SIGTERM] Signal received, do destroy");
	proxy && proxy.close();
	server && server.close();
	process.kill(0);
})

const Emit = (message: any) => {
	if (!process.send) throw "[ProxyWorker:Emit] process.send is undefined!";

	try {
		let data: any = {};
		if (message) Object.assign(data, message);
		data.sender = "ProxyWorker";
		process.send!(data);
	} catch (e) { }
};

proxy = httpProxy.createProxyServer({
	selfHandleResponse: true
});

proxy.on("error", (e, req, res, target) => {
	console.error("[ProxyWorker] Proxy error,", e);
	res.writeHead(500);
	res.end();
});

var web_o = require("http-proxy/lib/http-proxy/passes/web-outgoing");
web_o = Object.keys(web_o).map(pass => web_o[pass]);

const proxy_endpoints = [
	/\/kcs\//, /\/kcs2\//, /\/kcsapi\//
];

proxy.on("proxyReq", (proxyReq, req, res, opts) => {
	proxyReq.setHeader("cache-control", "must-revalidate, no-cache");
});
proxy.on("proxyRes", (proxyRes, req, res) => {
	if (!res.headersSent) {
		for (var i = 0; i < web_o.length; i++) {
			if (web_o[i](req, res, proxyRes, {})) break;
		}
	}

	const parsed = url.parse(req.url!, true, true);

	let body = Buffer.alloc(0, undefined, "binary");
	proxyRes.on("data", (data) => {
		body = Buffer.concat([body, data]);
		// res.write(data);
	});
	proxyRes.on("end", () => {
		// Parse kcs resources, kcsapi only
		if (!proxy_endpoints.some(x => x.test(parsed.pathname!))) {
			res.write(body);
			res.end();
			return;
		}

		const result = body;
		const callbackId = uuid();
		const callback = (message: ProxyWorkerResponse) => {
			if (message.callbackId !== callbackId) return;
			process.off("message", callback);

			console.debug(`[ProxyWorker] BeforeResponse end, for "${callbackId}", "${req.url}"`);

			const respBase64 = message.response.response; // From message, base64 data
			const respBuffer = Buffer.from(respBase64, "base64");

			Emit(<ProxyWorkerMessage>{
				type: "AfterResponse",
				request: {
					method: req.method,

					host: parsed.host,
					hostname: parsed.hostname,
					port: parsed.port,

					path: parsed.path,
					pathname: parsed.pathname,
					query: parsed.query
				},
				response: {
					response: respBase64 // Send to Master
				}
			});

			const headers = proxyRes.headers;
			headers["content-length"] = respBuffer.byteLength.toString();
			delete headers["content-encoding"];
			delete headers["transfer-encoding"];

			res.write(respBuffer);
			res.end();
		};
		console.debug(`[ProxyWorker] BeforeResponse start, for "${callbackId}", "${req.url}"`);
		process.on("message", callback);

		Emit(<ProxyWorkerMessage>{
			type: "BeforeResponse",
			callbackId: callbackId,
			request: {
				method: req.method,

				host: parsed.host,
				hostname: parsed.hostname,
				port: parsed.port,

				path: parsed.path,
				pathname: parsed.pathname,
				query: parsed.query
			},
			response: {
				response: result.toString("base64") // Send to Master, encode to base64
			}
		});
	});
	res.writeHead(proxyRes.statusCode!, proxyRes.headers);
});

server = http.createServer((req, res) => {
	if (!req.url) return;
	const parsed = url.parse(req.url, true, true);

	// Speed booster, for http (not https)
	const allowed_hosts: string[] = [
		"^log-netgame\\.dmm\\.com$",
		"^www\\.dmm\\.com$",
		"^dmm\\.com$",
		"^p\\.dmm\\.com$",
		"^osapi\\.dmm\\.com$",
		"^[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}$", // KanColle servers has no domain, just ip
	];
	if (!allowed_hosts.some(x => new RegExp(x).test(parsed.host!))) {
		res.writeHead(403, {
			connection: "close"
		});
		res.end();
		return;
	}

	delete req.headers["accept-encoding"]; // Plain data
	req.headers.connection = "close";

	const target = parsed.protocol + "//" + parsed.host;
	proxy && proxy.web(req, res, {
		target: target,
		secure: false
	});
}).listen(Proxy.Port);

server.on("error", e => {
	console.error("[ProxyWorker] Server error,", e);
})
server.on("connect", (req, socket) => {
	const serverUrl = url.parse("http://" + req.url);
	const srvSocket = net.connect(serverUrl.port ? parseInt(serverUrl.port) : 80, serverUrl.hostname, () => {
		socket.write("HTTP/1.1 200 Connection Established\r\n" +
			"Proxy-agent: Node-Proxy\r\n" +
			"\r\n");
		srvSocket.pipe(socket);
		socket.pipe(srvSocket);
	});
});
