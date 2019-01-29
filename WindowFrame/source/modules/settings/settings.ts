/// <reference path="../../../node_modules/ts-nameof/ts-nameof.d.ts" />
import Vue from "vue";
import { IModule } from "System/Module";
import { SettingInfo } from "System/Exports/API";
import { LoSCalculator } from "System/Models/LoSCalculator/LoSCalculator";

declare global {
	interface Window {
		OpenMenu(open?: boolean): boolean;
	}
}
interface ProcessedSettingInfo {
	i18nPrefix: string;

	Type: string;

	Name: string;
	Provider: string;
	Value: any;

	DisplayName: string;
	Description: string | null | undefined;
	Caution: string | null | undefined;

	Enums: { [key: string]: any } | null;
}

class SettingsModule implements IModule {
	private VueObject = new Vue({
		data: {
			i18n: window.i18n,

			Settings: <{ [key: string]: ProcessedSettingInfo[] }>{}
		},
		el: $("#settings-container")[0]
	});

	init(): void {
		(async () => {
			const settings = await window.API.GetSettings();
			this.VueObject.Settings = this.Preprocess(settings);
		})();

		window.modules.areas.register("main", "settings", "Settings", "", this.VueObject);
	}

	private Preprocess(settings: SettingInfo[]): { [key: string]: ProcessedSettingInfo[] } {
		const _ = settings.map(x => {
			let prefix = "";
			let _enums: { [key: string]: any } | null = null;
			if (x.Enums) {
				_enums = {};
				x.Enums.forEach(x => { _enums![x] = x });
			}

			if (x.Provider === "LoS") {
				if (x.Name === "LoSCalculator") {
					_enums = {};
					prefix = "los.";
					LoSCalculator.Instance.Logics.forEach(x => {
						_enums![x.Id] = x.Name;
					});
				}
			}
			return <ProcessedSettingInfo>{
				i18nPrefix: prefix,

				Type: x.Type,

				Name: x.Name,
				Provider: x.Provider,
				Value: x.Value,

				DisplayName: x.DisplayName,
				Description: x.Description,
				Caution: x.Caution,
				Enums: _enums
			};
		});
		const map = new Map<String, ProcessedSettingInfo[]>();
		_.forEach(item => {
			const key = item.Provider;
			const c = map.get(key);
			if (!c) map.set(key, [item]);
			else c.push(item);
		});

		const _output: { [key: string]: ProcessedSettingInfo[] } = {};
		map.forEach((v, k) => _output[k.toString()] = v);
		return _output;
	}
}
window.modules.register("settings", new SettingsModule());
export default SettingsModule;