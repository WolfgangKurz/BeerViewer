import { remote, ipcRenderer } from "electron";
import $ from "jquery";
import Game from "./Game";

const updateWindowFocus = (state: 0 | 1) => {
	if (state === 0)
		$("html").removeClass("focused");
	else
		$("html").addClass("focused");
};
ipcRenderer.on("window-focus-state", (_e: Event, _: string, state: 0 | 1) => updateWindowFocus(state));
updateWindowFocus(remote.getCurrentWindow().isFocused() ? 1 : 0);

$(".titlebar .systembox .close").click(() => {
	remote.getCurrentWindow().close();
});
$(".titlebar .systembox .minimize").click(() => {
	remote.getCurrentWindow().minimize();
});
$(".titlebar .systembox .maximize").click(() => {
	const window = remote.getCurrentWindow();
	if (window.isMaximized())
		remote.getCurrentWindow().restore();
	else
		remote.getCurrentWindow().maximize();
});
Game.Initialize();