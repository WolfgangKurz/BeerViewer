import { app, protocol, BrowserWindow } from "electron";
// import { createProtocol } from "vue-cli-plugin-electron-builder/lib";
import path from "path";

import Proxy from "@/Proxy/Proxy";
import Storage from "@/System/Storage";

import LiveTranslation from "./LiveTranslation";

const isDevelopment = process.env.NODE_ENV !== "production";

export default function Backend() {
	// Scheme must be registered before the app is ready
	/*
	if (protocol)
		protocol.registerSchemesAsPrivileged([{ scheme: "app", privileges: { secure: true, standard: true } }]);
	*/
	process.env.ELECTRON_DISABLE_SECURITY_WARNINGS = "true";

	// Setup local proxy server
	try {
		Proxy.Instance.SetupProxyServer();
	} catch (e) {
		console.log("Failed to startup Local proxy server!\n\n" + e);
		return;
	}

	// Setup live-translation
	new LiveTranslation("ko").Init();

	// Setup WindowFrame window
	let mainWindow: BrowserWindow | null;
	function createWindow() {
		mainWindow = new BrowserWindow({
			width: 1200,
			height: 720,
			icon: path.join(__static, "assets/icon.png"),

			frame: false,
			webPreferences: {
				nodeIntegration: true,
				webviewTag: true
			}
		});
		Storage.set("AppMainWindow", mainWindow); // Set window instance to global storage

		// Set WindowFrame window page
		if (process.env.WEBPACK_DEV_SERVER_URL) {
			// Load the url of the dev server if in development mode
			mainWindow.loadURL(process.env.WEBPACK_DEV_SERVER_URL as string);
		} else {
			// Load the index.html when not in development
			// mainWindow.loadURL("app://Frontend/index.html");
			mainWindow.loadURL("https://google.com/");
		}

		// Open Developer tools
		if (!process.env.IS_TEST)
			mainWindow.webContents.openDevTools();

		mainWindow.on("closed", () => {
			mainWindow = null;
		});
	}

	app.on("ready", () => createWindow());
	app.on("activate", () => {
		if (mainWindow === null) createWindow();
	});
	app.on("window-all-closed", () => {
		Proxy.Instance.Dispose();

		if (process.platform !== "darwin") {
			app.quit();
			process.kill(0);
		}
	});

	app.on("browser-window-focus", () => {
		if (mainWindow)
			mainWindow.webContents.send("window-focus-state", 1);
	});
	app.on("browser-window-blur", () => {
		if (mainWindow)
			mainWindow.webContents.send("window-focus-state", 0);
	});

	// Exit cleanly on request from parent process in development mode.
	if (isDevelopment) {
		if (process.platform === "win32") {
			process.on("message", (data) => {
				if (data === "graceful-exit") app.quit();
			});
		} else {
			process.on("SIGTERM", () => app.quit());
		}
	}
}