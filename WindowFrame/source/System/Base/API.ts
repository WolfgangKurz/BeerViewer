import Vue from "vue";
import { fns } from "./Base";

export class BaseAPI {
	private callbacks: { [key: string]: ((value: any) => void)[] } = {};
	private _origin_i18n: { [key: string]: string } = {};

	constructor() {
		const _this = this;

		window.INTERNAL = {
			async Initialized(): Promise<void> {
				await _this.LoadLanguage();

				window.CALLBACK.register("i18n", async (language: string) => {
					await _this.LoadLanguage();
					fns(_this.callbacks.i18n);
					Vue.nextTick();
				});
			},
			zoomMainFrame(_zoomFactor: number | string): void {
				const zoomFactor = typeof _zoomFactor === "number"
					? _zoomFactor / 100
					: parseFloat(_zoomFactor) / 100;

				$("#MAIN_FRAME")
					.css("transform", `scale(${zoomFactor})`)
					.css("marginRight", 1200 * (zoomFactor - 1) + "px")
					.css("marginBottom", 720 * (zoomFactor - 1) + "px");

				fns(_this.callbacks.zoomMainFrame, zoomFactor);
			},
			loadMainFrame(url: string): void {
				$("#MAIN_FRAME").prop("src", url);

				fns(_this.callbacks.loadMainFrame, url);
			}
		};
		window.i18n
			= new Proxy({}, {
				get: function (_, name) {
					const target = _this._origin_i18n;
					if (name in target) return target[name.toString()];
					return undefined; // name;
				}
			});
		window._i18n
			= async function (text: string): Promise<string> {
				return await window.API.i18n(text);
			};
	}

	Event(type: string, callback: (value: any) => void): void {
		if (!(type in this.callbacks))
			this.callbacks[type] = [];

		this.callbacks[type].push(callback);
	}

	private async LoadLanguage() {
		const set = await window.API.i18nSet();
		const target: { [key: string]: string } = {};
		const props = Object.getOwnPropertyNames(target);
		for (let i = 0; i < props.length; i++) delete target[props[i]];
		for (let i in set) target[i] = set[i];
		this._origin_i18n = target;
	}
}
export default BaseAPI;