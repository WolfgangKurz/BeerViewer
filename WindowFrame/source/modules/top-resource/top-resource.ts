import { IModule } from "../../System/Module"
import { Homeport } from "../../System/Homeport/Homeport";
import Vue from "vue";

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
		if(!Homeport.Instance.Materials) throw "Materials not set yet, something wrong";

		Homeport.Instance.Materials.Observe((name, value) => {
			if (!value) return;
			this.Resources[0].Value = parseInt(value.Fuel || 0) || 0;
			this.Resources[1].Value = parseInt(value.Ammo || 0) || 0;
			this.Resources[2].Value = parseInt(value.Steel || 0) || 0;
			this.Resources[3].Value = parseInt(value.Bauxite || 0) || 0;
			this.Resources[4].Value = parseInt(value.RepairBucket || 0) || 0;
			this.Resources[5].Value = parseInt(value.ImprovementMaterial || 0) || 0;
		});
		window.API.ObserveData("Homeport", "Admiral.ResourceLimit", (value:any) => topres.Overlimit = parseInt(value || 0) || 0);

		window.modules.areas.register("top", "top-resource", "Resources bar", "", topres);
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