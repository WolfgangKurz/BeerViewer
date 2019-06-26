import { app, BrowserWindow } from "electron";
import cluster from "cluster";
import path from "path";
import url from "url";
import Proxy from "./Proxy/Proxy";

if (cluster.isWorker) { // porked
	if (process.env["ClusterType"] === "ProxyWorker") {
		require("./Proxy/ProxyWorker");
	}
} else {
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

		const startUrl = process.env.ELECTRON_START_URL || url.format({
			pathname: path.join(__dirname, "../source/Form/index.html"),
			protocol: "file:",
			slashes: true
		});
		mainWindow.loadURL(startUrl);
		console.log(startUrl, __dirname, __filename);

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

	Proxy.Instance.SetupProxyServer();
}