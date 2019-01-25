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

            Settings: <ProcessedSettingInfo[]>[]
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

    private Preprocess(settings: SettingInfo[]): ProcessedSettingInfo[] {
        return settings.map(x => {
            let prefix = "";
            let _enums: { [key: string]: any } | null = null;
            if (x.Enums) {
                _enums = {};
                x.Enums.forEach(x => { _enums![x] = x });
            }

            if (x.Provider === "Setting") {
                if (x.Name === "LoSCalculator") {
                    _enums = {};
                    prefix = "loscalc_";
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
    }
}
window.modules.register("settings", new SettingsModule());
export default SettingsModule;