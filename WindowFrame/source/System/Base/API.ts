import Vue from "vue";
import { fns } from "./Base";

export class BaseAPI {
	private callbacks: { [key: string]: ((value: any) => void)[] } = {};
	private _origin_i18n: { [key: string]: string } = {};

	constructor() {
		const _this = this;
		Object.freeze(this._origin_i18n);

		window.INTERNAL = {
			async Initialized(): Promise<void> {
				await _this.LoadLanguage();
				fns(_this.callbacks.i18n, window.i18n);

				window.CALLBACK.register("i18n", async (language: string) => {
					await _this.LoadLanguage();
					fns(_this.callbacks.i18n, window.i18n);
				});
			}
		};
		window.i18n = this._origin_i18n;
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

		Object.freeze(this._origin_i18n);
		window.i18n = this._origin_i18n;
	}
}
export default BaseAPI;