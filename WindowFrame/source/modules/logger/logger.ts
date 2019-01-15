import { IModule } from "../../System/Module";

class Logger implements IModule {
	private logger: HTMLElement | null = null;

	public init(): void {
		const _this = this;
		this.logger = $('<div id="logger-container">')[0];

		window.CALLBACK.register("Logged", (text: string) => _this.log(text));
		_this.log("Logger module has been loaded");

		window.modules.areas.register("sub", "logger", "Log", "", this.logger);
	}

	public log(text: string): void {
		if (!this.logger) return;

		$(this.logger).append(
			$('<div class="logger-log">').append(text)
		);
	}
}
window.modules.register("logger", new Logger());
export default Logger;