/** Port of local proxy server */
export const ProxyPort: number = 49217;

/**
 * @param {ProxyRequest} request Request data
 * @param {Buffer} response Response data, as binary
 */
export type ProxyCallback = (request: ProxyRequest, response: Buffer) => void;

/**
 * @param {ProxyRequest} request Request data
 * @param {Buffer} response Response data, as binary
 * @returns {Buffer} Modified response data, as binary
 */
export type ProxyModifiableCallback = (request: ProxyRequest, response: Buffer) => Buffer;

/**
 * Response of Proxy, contains base64-encoded body.
 */
export interface ProxyResponse {
	/** Base64 encoded string */
	response: string;
}

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

	/** Request body */
	body?: Buffer;
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
