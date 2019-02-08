import { SettingInfo } from "./Exports/API";
import { fns } from "./Base/Base";

declare global {
	export interface Window {
		Settings: Settings;
		__SettingsInnerInformation: {
			_initialized: boolean;
			_loaded: boolean;
			readyCallbacks: (() => void)[];
		};
	}
}

window.__SettingsInnerInformation = {
	_initialized: false,
	_loaded: false,
	readyCallbacks: []
};

namespace Settings {
	class Settings {
		public static get Instance(): Settings { return instance }
		public static Initialize(): void {
			if (window.__SettingsInnerInformation._initialized) return;
			window.__SettingsInnerInformation._initialized = true;

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
					const _: { [name: string]: SettingInfo } = {};

					for (let i = 0; i < arr.length; i++) {
						_[arr[i].Name] = new Proxy(arr[i], {
							set(target, name: string, value) {
								if (name === "Value") {
									(async () => {
										if (await window.API.UpdateSetting(arr[i].Provider, arr[i].Name, value))
											target.Value = value;
									})();
									return true;
								} else
									return false;
							}
						});
					}

					_origin[provider] = new Proxy(_, {
						get(target, name: string) {
							return target[name];
						},
						set(_, __, ___) { return false } // Dismiss setter
					});
				});

				window.__SettingsInnerInformation._loaded = true;
				fns(window.__SettingsInnerInformation.readyCallbacks);
				window.__SettingsInnerInformation.readyCallbacks.splice(0, window.__SettingsInnerInformation.readyCallbacks.length);
			})();
		}
		public static Ready(callback: () => void) {
			if (window.__SettingsInnerInformation._loaded)
				callback && callback();
			else
				window.__SettingsInnerInformation.readyCallbacks.push(callback);
		}

		[provider: string]: {
			[name: string]: SettingInfo;
		};
	}
}
const _origin = new Settings();
const instance = window.Settings = new Proxy(_origin, {
	get(target, name: string) {
		return target[name];
	},
	set(_, __, ___) { return false } // Dismiss setter
});
export default Settings;
