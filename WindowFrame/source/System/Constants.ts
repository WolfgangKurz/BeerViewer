export class CookieInfo {
	public name!: string;
	public value!: string;
	public domain?: string;
	public path?: string = "/";
	public expires?: string;
}

export default class Constants {
	static get GameURL(): string {
		return "http://www.dmm.com/netgame/social/-/gadgets/=/app_id=854854";
	}
	static get RequireCookies(): CookieInfo[] {
		const expires = "Fri, 31 Dec 2100 23:59:59 GMT";
		return [
			{
				name: "cklg",
				value: "ja",
				domain: "dmm.com",
				expires: expires
			},
			{
				name: "ckcy",
				value: "1",
				domain: "osapi.dmm.com",
				expires: expires
			},
			{
				name: "ckcy",
				value: "1",
				domain: "203.104.209.7",
				expires: expires
			},
			{
				name: "ckcy",
				value: "1",
				domain: "www.dmm.com",
				path: "/netgame/",
				expires: expires
			}
		];
	}
	static get GameCSS(): string {
		return "body { margin:0; overflow:hidden }"
			+ "#game_frame { position:fixed; left:50%; top:-16px; margin-left:-600px; z-index:1 }"
			+ ".area-pickupgame, .area-menu { display: none !important }";
	}
}