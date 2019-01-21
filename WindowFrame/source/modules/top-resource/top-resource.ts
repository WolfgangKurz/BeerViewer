/// <reference path="../../../node_modules/ts-nameof/ts-nameof.d.ts" />
import Vue from "vue";
import { IModule } from "../../System/Module"
import { Homeport } from "../../System/Homeport/Homeport";
import { Materials } from "../../System/Homeport/Materials";

interface ResourceData {
	Name: string;
	LowerName: string;
	Value: number;
}
class TopResources implements IModule {
	private readonly ResourceList: string[] = ["Fuel", "Ammo", "Steel", "Bauxite", "RepairBucket", "ImprovementMaterial"];

	public Resources: ResourceData[] = [];
	public Overlimit: number = 5000;

	init(): void {
		this.ResourceList.forEach(x => {
			const res: ResourceData = {
				Name: x,
				LowerName: x.toLowerCase(),
				Value: 0
			};
			this.Resources.push(res);
		});

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
			(_, value) => Homeport.Instance.Admiral!.Observe((_, value) => value && (this.Overlimit = value), nameof(Homeport.Instance.Admiral!.ResourceLimit)),
			nameof(Homeport.Instance.Admiral)
		);

		window.modules.areas.register("top", "top-resource", "Resources bar", "", topres);
	}

	private UpdateResource(name: string, value: number) {
		if (typeof value !== "number") return;
		this.Resources.forEach(x => x.Name == name && (x.Value = value));
	}
}
export default TopResources;

const topres = new Vue({
	data: {
		Resources: [],
		Overlimit: 5000
	},
	el: $("#top-resources")[0]
});

window.modules.register("top-resource", new TopResources());