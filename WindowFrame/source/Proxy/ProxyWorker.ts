import http from "http";
import httpProxy from "http-proxy";
import crypto from "crypto";
import url from "url";
import fs from "fs";
import Proxy, { ProxyWorkerMessage, ProxyWorkerResponse } from "./Proxy";

const Emit = (message: any) => {
	if (!process.send) throw "[ProxyWorker:Emit] process.send is undefined!";

	let data: any = {};
	if (message) Object.assign(data, message);
	data.sender = "ProxyWorker";
	process.send!(data);
};

const logFile = "log.txt";
if (fs.existsSync(logFile)) fs.unlinkSync(logFile);
const Log = (...message: any[]) => {
	console.log(...message);
	fs.appendFileSync(logFile, message.map(x => x.toString()).join("\t") + "\r\n");
};

var proxy = httpProxy.createProxyServer({
	selfHandleResponse: true
});
proxy.on("error", (e, req, res, target) => {
	console.error(e);
	console.log(e);
	res.writeHead(500);
	res.end();
});

proxy.on("proxyReq", (proxyReq, req, res, opts) => {
	proxyReq.setHeader("cache-control", "must-revalidate, no-cache");
});
proxy.on("proxyRes", (proxyRes, req, res) => {
	const parsed = url.parse(req.url!, true, true);

	let body = Buffer.alloc(0);
	proxyRes.on("data", (data) => body = Buffer.concat([body, data]));
	proxyRes.on("end", () => {
		const result = body.toString();
		const callbackId = crypto.randomBytes(16).toString("hex");
		const callback = (message: ProxyWorkerResponse) => {
			if (message.callbackId !== callbackId) return;
			process.off("message", callback);

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
					response: message.response.response
				}
			});

			const headers = proxyRes.headers;
			// delete headers["content-length"];
			delete headers["transfer-encoding"];
			// headers.connection = "close";

			let __headers: string[] = [];
			for (let i = 0; i < proxyRes.rawHeaders.length; i += 2) {
				__headers.push(`${proxyRes.rawHeaders[i]}: ${proxyRes.rawHeaders[i + 1]}`);
			}

			const head = `HTTP/${proxyRes.httpVersion} ${proxyRes.statusCode} ${proxyRes.statusMessage}
		${__headers.join("\n")}

		`;

			fs.writeFile(
				"cache/" + Date.now() + "] " + (parsed.path!.replace(/[\<\>\:\"\/\\\|\?\*]/g, "_")),
				head + message.response.response,
				() => { }
			);

			res.writeHead(proxyRes.statusCode!, headers);
			res.end(message.response.response);
		};
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
				response: result
			}
		});
	});
});

const server = http.createServer((req, res) => {
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
	proxy.web(req, res, {
		target: target
	});
}).listen(Proxy.Port);