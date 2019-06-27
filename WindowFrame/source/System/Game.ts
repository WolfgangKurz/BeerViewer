import { WebviewTag } from "electron";
import $ from "jquery";
import Constants from "./Constants";
import Proxy from "../Proxy/Proxy";
import i18n from "../i18n/i18n";
import Unicode from "../Base/Unicode";

export default class Game {
	public static Initialize() {
		const _game = $("#GAME");
		if (_game.length !== 1) throw "Fatal Error, Game frame not found on main frame.";

		const game = <WebviewTag>_game.get(0);
		Proxy.Instance.Register("/kcsapi/api_start2/getData", (req, resp) => {
			const ko = i18n.get("ko");
			if (!ko) return;

			const list = ko.replace(/\r/g, "").split("\n")
				.filter(x => x.length > 0)
				.map(x => <[string, string]>x.split("\t"))
				.filter(x => x.length == 2)
				.sort((a, b) => a[0].length < b[0].length ? -1 : a[0].length > b[0].length ? 1 : 0);

			let body = resp.toString();
			list.forEach(x => body = body.replace(
				new RegExp(`"${Unicode.Escape(x[0])}"`, "g"),
				`"${Unicode.Escape(x[1])}"`
			));
			return Buffer.from(body);
		});

		const prepareGameFrame = () => {
			game.removeEventListener("dom-ready", prepareGameFrame); // Only once

			const session = game.getWebContents().session;
			Proxy.Instance.SetupEndpoint(session, () => {
				Constants.RequireCookies.forEach(cookie => {
					session.cookies.set({
						name: cookie.name,
						value: cookie.value,
						domain: cookie.domain,
						path: cookie.path,
						url: "http://" + cookie.domain + cookie.path
					});
				});
				game.loadURL(Constants.GameURL);
			});
		};
		game.addEventListener("dom-ready", prepareGameFrame);
		game.addEventListener("dom-ready", () => {
			game.setZoomFactor(2 / 3);
			if (game.getURL() === Constants.GameURL) game.insertCSS(Constants.GameCSS);
		});
	};
}