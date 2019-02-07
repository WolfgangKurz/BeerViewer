import Vue from "vue";
import { IModule } from "System/Module";
import { Keys } from "System/Enums/Keys";
import { CombinedVueInstance } from "vue/types/vue";
import { fns } from "System/Base/Base";

class GameModule implements IModule {
	private frame: CombinedVueInstance<Vue, {}, {
		reload(): void,
		zoom(_zoomFactor: number | string): void,
		load(url: string): void
	}, {}, Readonly<Record<never, any>>> | null = null;

	constructor() {
		const _this = this;
		Vue.component("game-component", {
			template: `<iframe src="MainFramePlaceholder.html" id="MAIN_FRAME" name="MAIN_FRAME" ref="DOMElement"></iframe>`,
			mounted() {
				_this.frame = this;

				this.$store.commit("e_register", {
					name: "game.reload",
					callback: () => this.reload()
				});
			},
			methods: {
				reload() {
					const frame = (<HTMLIFrameElement>this.$refs.DOMElement) || null;
					if (frame === null) return;
					
					window.API.ReloadMainFrame();
				},
				zoom(_zoomFactor: number | string): void {
					const frame = (<HTMLIFrameElement>this.$refs.DOMElement) || null;
					if (frame === null) return;

					const zoomFactor = typeof _zoomFactor === "number"
						? _zoomFactor / 100
						: parseFloat(_zoomFactor) / 100;

					$(frame)
						.css("transform", `scale(${zoomFactor})`)
						.css("marginRight", 1200 * (zoomFactor - 1) + "px")
						.css("marginBottom", 720 * (zoomFactor - 1) + "px");
				},
				load(url: string): void {
					const frame = (<HTMLIFrameElement>this.$refs.DOMElement) || null;
					if (frame === null) return;

					$(frame).prop("src", url);
				}
			}
		});
	}

	init(): void {
		window.CALLBACK.register("GlobalKeyInput", (key: Keys, modifiers: number) => key === Keys.F5 && this.reload());
		window.CALLBACK.register("Game.Zoom", (factor: number | string) => this.zoom(factor));
		window.CALLBACK.register("Game.Load", (url: string) => this.load(url));

		window.modules.areas.register("main", "game", "Game", "game", "game-component");
	}

	public reload(): void {
		if (this.frame === null) return;
		this.frame.reload();
	}
	public zoom(factor: number | string): void {
		if (this.frame === null) return;
		this.frame.zoom(factor);
	}
	public load(url: string): void {
		if (this.frame === null) return;
		this.frame.load(url);
	}
}
window.modules.register("game", new GameModule());
export default GameModule;
