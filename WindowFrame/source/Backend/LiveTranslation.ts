import Proxy from "../Proxy/Proxy";
import i18n from "../i18n/i18n";
import Unicode from "../Base/Unicode";

class LiveTranslation {
	private _Initialized: boolean = false;
	private list: { key: string, value: string }[];

	constructor() {
		let ko = i18n.get("ko");
		if (!ko) ko = "";

		this.list = ko.replace(/\r/g, "").split("\n")
			.filter(x => x.length > 0)
			.map(x => x.split("\t").filter(y => y.length > 0))
			.filter(x => x.length == 2)
			.map(x => ({
				key: x[0],
				value: x[1]
			}))
			.sort((a, b) => a.key.length < b.key.length ? -1 : a.key.length > b.key.length ? 1 : 0);
	}

	public Init(): void {
		if (this._Initialized) return;
		this._Initialized = true;

		Proxy.Instance.RegisterModifiable("/kcsapi/api_start2/getData", (req, resp) => {
			let body = resp.toString();
			this.list.forEach(x => {
				const a = Unicode.Escape(x.key);
				const b = Unicode.Escape(x.value);
				if (a === b) return;

				while (body.includes(`"${a}"`))
					body = body.replace(`"${a}"`, `"${b}"`);
			});
			return Buffer.from(body);
		});
	}
}
export default new LiveTranslation();