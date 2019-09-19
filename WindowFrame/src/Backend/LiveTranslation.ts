import Proxy from "@/Proxy/Proxy";
import { ProxyRequest } from "@/Proxy/Proxy.Define";

import i18n from "@/i18n/i18n";
import Unicode from "@/Base/Unicode";

/**
 * Translate in-game texts via HTTP response modification.
 *
 * Beta feature.
 */
export default class LiveTranslation {
	/** Has class initialized? */
	private pInitialized: boolean = false;

	/** List of text to translate */
	private list: Array<{ key: string, value: string }>;

	/**
	 * Initialize and setup LiveTranslation feature
	 * @param locale Locale to translate
	 */
	constructor(locale: string) {
		let ko = i18n.get(locale); // Load all localization texts of given locale
		if (!ko) ko = "";

		// MAke translation text list
		this.list = ko.replace(/\r/g, "").split("\n") // Split by lines
			.filter((x) => x.length > 0) // Skip empty line
			.map((x) => x.split("\t").filter((y) => y.length > 0)) // Tab separated only (key-value pair only)
			.filter((x) => x.length === 2) // No multi-values
			.map((x) => ({ key: x[0], value: x[1] })) // Make key-value pair
			.sort((a, b) => a.key.length < b.key.length ? -1 : a.key.length > b.key.length ? 1 : 0) // Sort by length of text
			|| {}; // Or, empty list.
	}

	/** Register modificable HTTP handler */
	public Init(): void {
		if (this.pInitialized) return;
		this.pInitialized = true;

		Proxy.Instance.RegisterModifiable("/kcsapi/api_start2/getData", this.Handler);
	}

	/**
	 * Handler to modify KanColle API to translate in-game text
	 * @param req Request data
	 * @param resp Response body
	 */
	private Handler(req: ProxyRequest, resp: Buffer): Buffer {
		let body = resp.toString();

		// Find text to translate
		this.list.forEach((x) => {
			const a = Unicode.Escape(x.key);
			const b = Unicode.Escape(x.value);
			if (a === b) return;

			while (body.includes(`"${a}"`)) // Translate if found.
				body = body.replace(`"${a}"`, `"${b}"`);
		});
		return Buffer.from(body);
	}
}
