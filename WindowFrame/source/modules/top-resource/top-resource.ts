/// <reference path="../../../node_modules/ts-nameof/ts-nameof.d.ts" />
import Vue from "vue";
import { IModule, GetModuleTemplate } from "System/Module"
import { Homeport } from "System/Homeport/Homeport";
import { Materials } from "System/Homeport/Materials";
import { Admiral } from "System/Homeport/Admiral";
import Settings, { Settings as SettingsClass } from "System/Settings";
import { IdentifiableTable } from "System/Models/TableWrapper";
import { Ship } from "System/Homeport/Ship";
import { Equipment } from "System/Homeport/Equipment/Equipment";

interface IResource {
	Name: string;
	LowerName: string;
	Maximum?: number;
	Value: number;
	Display: boolean;
	Type: "Resource" | "Count";
}

type TResources = "" | "Fuel" | "Ammo" | "Steel" | "Bauxite" | "Bucket" | "DevMaterial" | "InstConstruction" | "ImprvMaterial" | "ShipCount" | "EquipCount";

class TopResources implements IModule {
	private Data = {
		Overlimit: <number>5000,

		Datas: <{ [key in TResources]: IResource }>{
			Fuel: {
				Name: "Fuel",
				LowerName: "fuel",
				Value: 0,
				Display: true,
				Type: "Resource"
			},
			Ammo: {
				Name: "Ammo",
				LowerName: "ammo",
				Value: 0,
				Display: true,
				Type: "Resource"
			},
			Steel: {
				Name: "Steel",
				LowerName: "steel",
				Value: 0,
				Display: true,
				Type: "Resource"
			},
			Bauxite: {
				Name: "Bauxite",
				LowerName: "bauxite",
				Value: 0,
				Display: true,
				Type: "Resource"
			},
			Bucket: {
				Name: "Repair Bucket",
				LowerName: "repairbucket",
				Value: 0,
				Display: true,
				Type: "Resource"
			},
			DevMaterial: {
				Name: "Development Material",
				LowerName: "developmentmaterial",
				Value: 0,
				Display: false,
				Type: "Resource"
			},
			InstConstruction: {
				Name: "Instant Construction",
				LowerName: "instantconstruction",
				Value: 0,
				Display: false,
				Type: "Resource"
			},
			ImprvMaterial: {
				Name: "Improvement Material",
				LowerName: "improvementmaterial",
				Value: 0,
				Display: true,
				Type: "Resource"
			},
			ShipCount: {
				Name: "Ship Count",
				LowerName: "shipcount",
				Value: 0,
				Display: false,
				Type: "Count"
			},
			EquipCount: {
				Name: "Equip Count",
				LowerName: "equipcount",
				Value: 0,
				Display: false,
				Type: "Count"
			}
		}
	}

	constructor() {
		Vue.component("top-resource-component", {
			data: () => this.Data,
			template: GetModuleTemplate()
		});
		
		let key: TResources;
		for (key in this.Data.Datas) {
			console.log(key, this.Data.Datas[key]);
			SettingsClass.Instance.Register({
				Provider: "Top-Resources",
				Name: `Disp${key}`,
				DisplayName: `Display ${this.Data.Datas[key].Name}`,
				Value: this.Data.Datas[key].Display,
				Type: "Boolean"
			});
			SettingsClass.Instance.Observe(`Top-Resources.Disp${key}`, value => this.Data.Datas[key].Display = <boolean>value);
		}
	}

	init(): void {
		(function (this: Materials | null, _this: TopResources) {
			if (!this) throw "Materials not set yet, something wrong";

			this.Observe((name, value) => value && _this.UpdateResource("Fuel", value), nameof(this.Fuel));
			this.Observe((name, value) => value && _this.UpdateResource("Ammo", value), nameof(this.Ammo));
			this.Observe((name, value) => value && _this.UpdateResource("Steel", value), nameof(this.Steel));
			this.Observe((name, value) => value && _this.UpdateResource("Bauxite", value), nameof(this.Bauxite));
			this.Observe((name, value) => value && _this.UpdateResource("Bucket", value), nameof(this.RepairBucket));
			this.Observe((name, value) => value && _this.UpdateResource("DevMaterial", value), nameof(this.DevelopmentMaterial));
			this.Observe((name, value) => value && _this.UpdateResource("InstConstruction", value), nameof(this.InstantConstruction));
			this.Observe((name, value) => value && _this.UpdateResource("ImprvMaterial", value), nameof(this.ImprovementMaterial));

			Homeport.Instance.Observe((_, value: Admiral) => value && value.Observe((_, value: number) => _this.Data.Datas.ShipCount.Maximum = value, nameof(Homeport.Instance.Admiral!.MaximumShips)), nameof(Homeport.Instance.Admiral));
			Homeport.Instance.Observe((_, value: Admiral) => value && value.Observe((_, value: number) => _this.Data.Datas.EquipCount.Maximum = value, nameof(Homeport.Instance.Admiral!.MaximumEquips)), nameof(Homeport.Instance.Admiral));

			Homeport.Instance.Observe((_, value: IdentifiableTable<Ship>) => value && (_this.Data.Datas.ShipCount.Value = value.size), nameof(Homeport.Instance.Ships));
			Homeport.Instance.Equipments.Observe((_, value: number) => value && (_this.Data.Datas.EquipCount.Value = value), nameof(Homeport.Instance.Equipments.EquipCount));
		}).call(Homeport.Instance.Materials, this);

		Homeport.Instance.Observe(
			(_, value: Admiral) => value && value.Observe(
				(_, value: number) => value && (this.Data.Overlimit = value),
				nameof(Homeport.Instance.Admiral!.ResourceLimit)
			),
			nameof(Homeport.Instance.Admiral)
		);

		window.modules.areas.register("top", "top-resource", "Resources bar", "", "top-resource-component");
	}

	private UpdateResource(name: TResources, value: number) {
		if (typeof value !== "number") return;

		if (!(name in this.Data.Datas)) return;
		this.Data.Datas[name].Value = value;
	}
}
window.modules.register("top-resource", new TopResources());
export default TopResources;