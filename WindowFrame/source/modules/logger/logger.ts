import Vue from "vue";
import { mapState } from "vuex";
import { IModule } from "System/Module";

interface LogInfo {
	Date: string;
	Text: string;
	Arguments?: string[];
}
class Logger implements IModule {
	private Data = {
		Logs: <LogInfo[]>[]
	};

	constructor() {
		Vue.component("logger-component", {
			data: () => this.Data,
			template: $("#logger-container").prop("outerHTML"),
			computed: mapState({
				i18n: "i18n"
			}),
			methods: {
				format(format: string, args: string[]) {
					if (args === undefined) return format;
					for (let i = 0; i < args.length; i++) {
						const arg = args[i];
						const reg = new RegExp("\\{" + i + "\\}", "g");
						format = format.replace(reg, arg);
					}
					return format;
				}
			}
		});
	}

	public init(): void {
		const _this = this;
		window.CALLBACK.register("Logged", (Format: string, Arguments: string[] | undefined) => {
			_this.log(Format, Arguments);
		});
		_this.log("Logger module has been loaded");

		window.modules.areas.register("sub", "logger", "Log", "", "logger-component");
	}

	public log(format: string, args?: string[]): void {
		const date = new Date();
		const hours = ("0" + date.getHours()).substr(-2);
		const minutes = ("0" + date.getMinutes()).substr(-2);
		const seconds = ("0" + date.getSeconds()).substr(-2);

		this.Data.Logs.splice(0, 0, {
			Date: `${hours}:${minutes}:${seconds}`,
			Text: format,
			Arguments: args
		});
	}
}
window.modules.register("logger", new Logger());
export default Logger;