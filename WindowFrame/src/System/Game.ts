import { WebviewTag } from "electron";
import $ from "jquery";

import Constants from "./Constants";
import Proxy from "../Proxy/Proxy";

export default class Game {
	/**
	 * Initialize WindowFrame.
	 *
	 * Set initial zoom level, and stylesheet.
	 * Set cookies to prevent foreigner page, and load Game URL.
	 */
	public static Initialize() {
		const gameElem = $("#GAME");
		if (gameElem.length !== 1) throw new Error("Fatal Error, Game frame not found on main frame.");

		const game = gameElem.get(0) as WebviewTag;

		// Event after Game frame ready
		const prepareGameFrame = () => {
			game.removeEventListener("dom-ready", prepareGameFrame); // Only once

			const session = game.getWebContents().session; // Session for set cookies
			Proxy.Instance.SetupEndpoint(session, () => {
				Constants.RequireCookies.forEach((cookie) => { // Prevent foreigner page
					session.cookies.set({
						name: cookie.name,
						value: cookie.value,
						domain: cookie.domain,
						path: cookie.path,
						url: "http://" + cookie.domain + cookie.path
					});
				});
				game.loadURL(Constants.GameURL); // Load Game URL
			});
		};
		game.addEventListener("dom-ready", prepareGameFrame);
		game.addEventListener("dom-ready", () => {
			game.setZoomFactor(2 / 3); // Initial zoom level (66.666%, KC 1.0 size, 800x480, for debug now)
			if (game.getURL() === Constants.GameURL) game.insertCSS(Constants.GameCSS); // Apply stylesheet
		});
	}
}
