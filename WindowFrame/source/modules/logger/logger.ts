import Vue from "vue";
import { IModule } from "System/Module";
import TemplateContent from "./logger.html";

class Logger implements IModule {
	private Data = {
		Logs: <string[]>[]
	};

	constructor() {
		Vue.component("logger-component", {
			props: ["i18n"],
			data: () => this.Data,
			template: TemplateContent
		});
	}

	public init(): void {
		const _this = this;
		window.CALLBACK.register("Logged", (text: string) => _this.log(text));
		_this.log("Logger module has been loaded");

		window.modules.areas.register("sub", "logger", "Log", "", "logger-component");
	}

	public log(text: string): void {
		this.Data.Logs.splice(0, 0, text);
	}
}
window.modules.register("logger", new Logger());
export default Logger;