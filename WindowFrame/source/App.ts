import { app, BrowserWindow } from "electron";
import cluster from "cluster";
import path from "path";
import url from "url";
import Proxy from "./Proxy/Proxy";
import Storage from "./System/Storage";

if (cluster.isWorker) { // porked
	if (process.env["ClusterType"] === "ProxyWorker") {
		require("./Proxy/ProxyWorker");
	}
} else {
	process.env['ELECTRON_DISABLE_SECURITY_WARNINGS'] = 'true';

	let mainWindow: Electron.BrowserWindow;
	function createWindow() {
		mainWindow = new BrowserWindow({
			width: 1200,
			height: 720,
			frame: false,
			webPreferences: {
				nodeIntegration: true,
				webviewTag: true
			}
		});
		Storage.set("AppMainWindow", mainWindow);

		const startUrl = process.env.ELECTRON_START_URL || url.format({
			pathname: path.join(__dirname, "./Form/index.html"),
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
	app.on("window-all-closed", ()=>{
		Proxy.Instance.Dispose();
		process.exit(0);
	});

	app.on("browser-window-focus", () => {
		mainWindow.webContents.send("window-focus-state", "get", 1);
	});
	app.on("browser-window-blur", () => {
		mainWindow.webContents.send("window-focus-state", "lost", 0);
	});

	Proxy.Instance.SetupProxyServer();
}