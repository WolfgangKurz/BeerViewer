import Modules, { IModule } from "../module";
import API from "../../System/Exports/API";

class Logger implements IModule {
	private logger: HTMLElement | null = null;

	public init():void {
		const _this = this;
		const logger = document.createElement("div");
		logger.id = "logger-container";
		this.logger = logger;

		window.CALLBACK.register("Logged", function (text: string) {
			_this.log(text);
		});
		_this.log("Logger module has been loaded");

		window.modules.areas.register("sub", "logger", "Log", "", logger);
	}

	public log(text: string): void {
		if (!this.logger) return;

		const logline = document.createElement("div");
		logline.className = "logger-log";
		logline.appendChild(document.createTextNode(text));
		this.logger.append(logline);
	}
}
window.modules.register("logger", new Logger());
export default Logger;