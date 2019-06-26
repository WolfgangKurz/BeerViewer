import http from "http";
import url from "url";
import Proxy from "./Proxy";

const Emit = (message: any) => {
	if (!process.send) throw "[ProxyWorker:Emit] process.send is undefined!";

	let data: any = {};
	if (message) Object.assign(data, message);
	data.sender = "ProxyWorker";
	process.send!(data);
}

http.createServer(function (client_req, client_res) {
	if (!client_req.url) return;
	const parsed = url.parse(client_req.url, true, true);

	console.log(parsed);

	const options: http.ClientRequestArgs = {
		method: client_req.method,
		host: parsed.host, // hostname + port
		path: parsed.path // path + query string
	};

	var proxy = http.request(options, function (res) {
		let data = "";
		res.on("data", chunk => {
			data += chunk;
		})
		res.on("end", () => {
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
					response: data
				}
			});

			client_res.write(data);
			client_res.end();
		});
	});

	client_req.pipe(proxy, {
		end: true
	});
}).listen(Proxy.Port);