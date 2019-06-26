import http from "http";
import url from "url";
import fs from "fs";
import crypto from "crypto";
import Proxy from "./Proxy";

const Emit = (message: any) => {
	if (!process.send) throw "[ProxyWorker:Emit] process.send is undefined!";

	let data: any = {};
	if (message) Object.assign(data, message);
	data.sender = "ProxyWorker";
	process.send!(data);
}

http.createServer((client_req, client_res) => {
	if (!client_req.url) {
		return;
	}
	const _id = crypto.randomBytes(8).toString("hex");

	const parsed = url.parse(client_req.url, true, true);
	const options: http.ClientRequestArgs = {
		method: client_req.method,
		host: parsed.host, // hostname + port
		path: parsed.path, // path + query string
	};

	options.headers = client_req.headers;
	if ("accept-encoding" in options.headers) {
		delete options.headers["accept-encoding"];
	}

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
		client_res.writeHead(403, {
			connection: "close"
		});
		client_res.end();
		return;
	}

	fs.appendFile("http-log.txt", `${_id} > ${client_req.url}\n`, () => null);

	var proxy = http.request(options, req_res => {
		let data: string = "";

		req_res.on("data", chunk => {
			data += chunk;
		})
		req_res.on("end", () => {
			const body = data;
			Emit({
				request: {
					method: client_req.method,

					host: parsed.host,
					hostname: parsed.hostname,
					port: parsed.port,

					path: parsed.path,
					pathname: parsed.pathname,
					query: parsed.query
				},
				response: {
					response: body
				}
			});


			for (let i = 0; i < req_res.rawHeaders.length; i += 2) {
				fs.appendFile("http-log.txt", `${_id} > ${req_res.rawHeaders[i]}: ${req_res.rawHeaders[i + 1]}\n`, () => null);
			}
			/*
						console.log("-----------------------------------------------");
						console.log(`Target: ${parsed.protocol}://${parsed.host}${parsed.path}`);
						console.log("");
						console.log(`HTTP/${req_res.httpVersion} ${req_res.statusCode} ${req_res.statusMessage}`);
						for (let i = 0; i < req_res.rawHeaders.length; i += 2) {
							console.log(`${req_res.rawHeaders[i]}: ${req_res.rawHeaders[i + 1]}`);
						}
						console.log("");
						console.log(body);
						console.log("-----------------------------------------------");
			*/
			const headers = req_res.headers;
			headers["cache-control"] = "must-revalidate, no-cache";
			headers.connection = "close";
			client_res.writeHead(req_res.statusCode!, headers);
			client_res.end(body);
		});
		req_res.on("error", e => {
			console.error(e);
		});
		req_res.on("close", () => {
			console.log("close");
		});
	});

	client_req.pipe(proxy, {
		end: true
	});
}).listen(Proxy.Port);