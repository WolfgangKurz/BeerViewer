import { app, BrowserWindow } from "electron";
import * as path from "path";
import * as url from "url";

let mainWindow: Electron.BrowserWindow;

function createWindow() {
	mainWindow = new BrowserWindow({
		width: 1200,
		height: 720,
		frame: false,
		webPreferences: {
			nodeIntegration: true
		}
	});

	const startUrl = process.env.ELECTRON_START_URL || url.format({
		pathname: path.join(__dirname, "../source/index.html"),
		protocol: "file:",
		slashes: true
	});
	mainWindow.loadURL(startUrl);

	mainWindow.webContents.openDevTools();
}

app.on("ready", () => {
	createWindow();
});
app.on("activate", () => {
	if (mainWindow === null) {
		createWindow();
	}
});

app.on("browser-window-focus", () => {
	mainWindow.webContents.send("window-focus-state", "get", 1);
});
app.on("browser-window-blur", () => {
	mainWindow.webContents.send("window-focus-state", "lost", 0);
});
