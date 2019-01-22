/// <reference path="../../../node_modules/ts-nameof/ts-nameof.d.ts" />
import Vue from "vue";
import { IModule } from "../../System/Module"
import { Homeport } from "../../System/Homeport/Homeport";
import { Materials } from "../../System/Homeport/Materials";
import { Admiral } from "../../System/Homeport/Admiral";

interface ResourceData {
	Name: string;
	LowerName: string;
	Value: number;
}
class TopResources implements IModule {
	private readonly ResourceList: string[] = ["Fuel", "Ammo", "Steel", "Bauxite", "RepairBucket", "ImprovementMaterial"];

	private VueObject = new Vue({
		data: {
			Resources: <ResourceData[]>[],
			Overlimit: <number>5000
		},
		el: $("#top-resources")[0]
	});

	init(): void {
		const Resources: ResourceData[] = [];
		this.ResourceList.forEach(x => {
			const res: ResourceData = {
				Name: x,
				LowerName: x.toLowerCase(),
				Value: 0
			};
			Resources.push(res);
		});
		this.VueObject.Resources = Resources;

		(function (this: Materials | null, _this: TopResources) {
			if (!this) throw "Materials not set yet, something wrong";

			this.Observe((name, value) => value && _this.UpdateResource(name, value), nameof(this.Fuel));
			this.Observe((name, value) => value && _this.UpdateResource(name, value), nameof(this.Ammo));
			this.Observe((name, value) => value && _this.UpdateResource(name, value), nameof(this.Steel));
			this.Observe((name, value) => value && _this.UpdateResource(name, value), nameof(this.Bauxite));
			this.Observe((name, value) => value && _this.UpdateResource(name, value), nameof(this.RepairBucket));
			this.Observe((name, value) => value && _this.UpdateResource(name, value), nameof(this.ImprovementMaterial));
		}).call(Homeport.Instance.Materials, this);

		Homeport.Instance.Observe(
			(_, value: Admiral) => value && value.Observe(
				(_, value: number) => value && (this.VueObject.Overlimit = value),
				nameof(Homeport.Instance.Admiral!.ResourceLimit)
			),
			nameof(Homeport.Instance.Admiral)
		);

		window.modules.areas.register("top", "top-resource", "Resources bar", "", this.VueObject);
	}

	private UpdateResource(name: string, value: number) {
		if (typeof value !== "number") return;
		this.VueObject.Resources.forEach(x => x.Name == name && (x.Value = value));
	}
}
window.modules.register("top-resource", new TopResources());
export default TopResources;