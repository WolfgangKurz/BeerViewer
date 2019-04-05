import { SettingInfo } from "./Exports/API";
import { fns } from "./Base/Base";

declare global {
	interface Window {
		Settings: Settings;
	}
}

export type SettingsData = {
	[provider: string]: {
		[name: string]: SettingInfo;
	}
};
export class Settings {
	private _Initialized: boolean = false;
	private _Loaded: boolean = false;
	private readonly _ReadyCallbacks: (() => void)[] = [];

	private constructor() { }

	public static get Instance(): Settings {
		if(!window.Settings) window.Settings = new Settings();
		return window.Settings;
	}

	public Initialize(): void {
		if (this._Initialized) return;
		this._Initialized = true;

		(async () => {
			const settings = await window.API.GetSettings();
			const map = new Map<string, SettingInfo[]>();
			settings.forEach(item => {
				const key = item.Provider;
				const c = map.get(key);
				if (!c) map.set(key, [item]);
				else c.push(item);
			});

			map.forEach((arr, provider) => {
				const group: { [name: string]: SettingInfo } = {};

				for (let i = 0; i < arr.length; i++) {
					const item = this.BuildSetting(arr[i]);

					// Set setting item as readonly
					Object.defineProperty(group, item.Name, {
						value: item,
						configurable: false,
						enumerable: true,
						writable: false
					});
				}

				// Set provider group as readonly
				Object.defineProperty(this.data, provider, {
					value: group,
					configurable: false,
					enumerable: true,
					writable: false
				});
			});

			this._Loaded = true;
			fns(this._ReadyCallbacks);
		})();
	}
	public Ready(callback: () => void): void {
		this._ReadyCallbacks.push(callback);
		if (this._Loaded)
			callback && callback();
	}

	public Register(setting: SettingInfo) {
		(async () => {
			const info = await window.API.GetWindowFrameSetting(setting.Provider, setting.Name, setting.Type);
			if(!info) return;

			this.data[setting.Provider][setting.Name].Value = info.Value;
		})();

		const provider = setting.Provider;
		const item = this.BuildSetting(setting);
		const group = this.data[provider] || {};

		// Set setting item as readonly
		Object.defineProperty(group, item.Name, {
			value: item,
			configurable: false,
			enumerable: true,
			writable: false
		});

		if (!(provider in this.data)) {
			// Set provider group as readonly
			Object.defineProperty(this.data, provider, {
				value: group,
				configurable: false,
				enumerable: true,
				writable: false
			});
		}

		if (this._Loaded)
			fns(this._ReadyCallbacks);
	}

	private BuildSetting(info: SettingInfo) {
		const _this = this;
		const item: SettingInfo = <SettingInfo>{};

		let key: keyof SettingInfo;
		for (key in info) { // Without "Value", readonly
			if (key === "Value") continue;

			Object.defineProperty(item, key, {
				value: info[key],
				configurable: false,
				enumerable: false,
				writable: false
			});
		}

		// "Value" is writable, settable.
		Object.defineProperty(item, "Value", {
			get() {
				return info.Value;
			},
			set(value) {
				(async () => {
					if (await window.API.UpdateSetting(item.Provider, item.Name, value)) {
						info.Value = value;
						_this.CallObservers(`${item.Provider}.${item.Name}`, value);
					}
				})();
			},
			configurable: false,
			enumerable: false
		});
		return item;
	}

	public Observe(key: string, callback: (value: string | number | boolean) => void) {
		if (!(key in this.observers))
			this.observers[key] = [callback];
		else
			this.observers[key].push(callback);
	}
	private CallObservers(key: string, value: string | number | boolean) {
		fns(this.observers[key], value);
	}

	private readonly observers: { [key: string]: ((value: string | number | boolean) => void)[] } = {};
	public readonly data: SettingsData = {};
}
export default Settings.Instance.data;