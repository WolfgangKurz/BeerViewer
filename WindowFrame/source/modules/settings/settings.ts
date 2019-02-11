/// <reference path="../../../node_modules/ts-nameof/ts-nameof.d.ts" />
import Vue from "vue";
import { mapState } from "vuex";
import { IModule } from "System/Module";
import { LoSCalculator } from "System/Models/LoSCalculator/LoSCalculator";
import TemplateContent from "./settings.html";
import Settings, { Settings as _Settings, SettingsData } from "System/Settings";

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
		data: () => this.Data,
		template: TemplateContent,
		computed: mapState({
			i18n: "i18n"
		}),
		methods: {
			UpdateSetting(event: Event) {
				const target = <HTMLInputElement | HTMLSelectElement>event.target;

				const name: string[] = target.name.split(".");
				const Provider: string = name[0];
				const Name: string = name[1];
				let Value: string | boolean = target.value;

				if (target instanceof HTMLInputElement && target.type === "checkbox")
					Value = target.checked;

				const inst = Settings;
				inst[Provider] && inst[Provider][Name] && (inst[Provider][Name].Value = Value);
			}
		}
	});

	init(): void {
		_Settings.Instance.Ready(() => this.Data.Settings = this.Preprocess(Settings));

		window.modules.areas.register("main", "settings", "Settings", "setting", "settings-component");
	}

	private Preprocess(settings: SettingsData): { [key: string]: ProcessedSettingInfo[] } {
		const _: ProcessedSettingInfo[] =
			(() => {
				const _ = [];
				for (const provider in settings)
					for (const name in settings[provider])
						_.push(settings[provider][name]);
				return _;
			})()
			.map(x => {
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
				return {
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

		const map = new Map<string, ProcessedSettingInfo[]>();
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