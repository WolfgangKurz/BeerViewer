import { remote, ipcRenderer } from "electron";
import $ from "jquery";

import Game from "../System/Game";

/**
 * Update window focus state
 * @param state 1 if focused, 0 if blurred.
 */
function updateWindowFocus(state: 0 | 1): void  {
	if (state === 0)
		$("html").removeClass("focused");
	else
		$("html").addClass("focused");
}

// Register window focus changed event, Initial focus state
ipcRenderer.on("window-focus-state", ($event: Event, _: string, state: 0 | 1) => updateWindowFocus(state));
updateWindowFocus(remote.getCurrentWindow().isFocused() ? 1 : 0);

// System button handlers
$(".titlebar .systembox .close").click(() => remote.getCurrentWindow().close());
$(".titlebar .systembox .minimize").click(() => remote.getCurrentWindow().minimize());
$(".titlebar .systembox .maximize").click(() => {
	const window = remote.getCurrentWindow();
	if (window.isMaximized())
		remote.getCurrentWindow().restore();
	else
		remote.getCurrentWindow().maximize();
});

// Initialize Game
Game.Initialize();
