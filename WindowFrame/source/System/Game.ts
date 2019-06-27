import { WebviewTag, remote } from "electron";
import $ from "jquery";
import Constants from "./Constants";
import Proxy from "../Proxy/Proxy";

export default class Game {
	public static Initialize() {
		const _game = $("#GAME");
		if (_game.length !== 1) throw "Fatal Error, Game frame not found on main frame.";

		const game = <WebviewTag>_game.get(0);
/*
		Proxy.Instance.Register("/kcsapi/api_start2/getData", (req, resp) => {
			console.log(resp.response);
		});
*/
		const prepareGameFrame = () => {
			game.removeEventListener("dom-ready", prepareGameFrame); // Only once

			const session = game.getWebContents().session;
			Proxy.Instance.SetupEndpoint(session);
			
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