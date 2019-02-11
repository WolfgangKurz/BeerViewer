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
		return window.Settings = window.Settings || new Settings();
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
					const item: SettingInfo = <SettingInfo>{};

					let key: keyof SettingInfo;
					for (key in arr[i]) { // Without "Value", readonly
						if (key === "Value") continue;

						Object.defineProperty(item, key, {
							value: arr[i][key],
							configurable: false,
							enumerable: false,
							writable: false
						});
					}

					// "Value" is writable, settable.
					Object.defineProperty(item, "Value", {
						get() {
							return arr[i].Value;
						},
						set(value) {
							(async () => {
								if (await window.API.UpdateSetting(item.Provider, item.Name, value))
									arr[i].Value = value;
							})();
						},
						configurable: false,
						enumerable: false
					});

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
			this._ReadyCallbacks.splice(0, this._ReadyCallbacks.length);
		})();
	}
	public Ready(callback: () => void): void {
		if (this._Loaded)
			callback && callback();
		else
			this._ReadyCallbacks.push(callback);
	}

	public readonly data: SettingsData = {};
}
export default Settings.Instance.data;