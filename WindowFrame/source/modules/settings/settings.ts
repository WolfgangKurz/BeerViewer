/// <reference path="../../../node_modules/ts-nameof/ts-nameof.d.ts" />
import Vue from "vue";
import { IModule } from "System/Module";
import { SettingInfo } from "System/Exports/API";
import { LoSCalculator } from "System/Models/LoSCalculator/LoSCalculator";
import TemplateContent from "./settings.html";

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
type ListTable<T> = { [key: string]: T[] };

class SettingsModule implements IModule {
	private Data = {
		Settings: <ListTable<ProcessedSettingInfo>>{}
	};

	private VueObject = Vue.component("settings-component", {
		props: ["i18n"],
		data: () => this.Data,
		template: TemplateContent,
		methods: {
			i18nf(input: string, def?: string) {
				console.log(input, def, this.i18n);
				if (input in this.i18n) return this.i18n[input];
				return def || input;
			},

			UpdateSetting(event: Event) {
				const target = <HTMLInputElement | HTMLSelectElement>event.target;

				const name: string[] = target.name.split(".");
				const Provider: string = name[0];
				const Name: string = name[1];
				let Value: string | boolean = target.value;

				if (target instanceof HTMLInputElement && target.type === "checkbox")
					Value = target.checked;

				window.API.UpdateSetting(Provider, Name, Value);
			}
		}
	});

	init(): void {
		(async () => {
			const settings = await window.API.GetSettings();
			this.Data.Settings = this.Preprocess(settings);
		})();

		window.modules.areas.register("main", "settings", "Settings", "setting", "settings-component");
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