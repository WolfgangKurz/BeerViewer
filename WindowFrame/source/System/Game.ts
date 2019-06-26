import { WebviewTag, remote } from "electron";
import $ from "jquery";
import Constants from "./Constants";
import Proxy from "../Proxy/Proxy";

export default class Game {
	public static Initialize() {
		const _game = $("#GAME");
		if (_game.length !== 1) throw "Fatal Error, Game frame not found on main frame.";

		Proxy.Instance.SetupEndpoint(remote.session.defaultSession);

		Proxy.Instance.Register("/api_start2/getData", (req, resp) => {
			console.log(resp.response);
		});

		const game = <WebviewTag>_game.get(0);
		const prepareGameFrame = () => {
			game.removeEventListener("dom-ready", prepareGameFrame); // Only once

			const session = game.getWebContents().session;
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

			game.openDevTools();
		};
		game.addEventListener("dom-ready", prepareGameFrame);
		game.addEventListener("dom-ready", () => {
			game.setZoomFactor(2 / 3);
			if (game.getURL() === Constants.GameURL) game.insertCSS(Constants.GameCSS);
		});
	};
}