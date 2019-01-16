import Vue from "vue";
import { IModule } from "../../System/Module";
import { Homeport } from "../../System/Homeport/Homeport";

interface ExpeditionData {
	Enabled: boolean;
	Activated: boolean;

	Id: number;
	RemainingText: string;
	Progress: number;
}

const MAX_FLEETS = 4; // Maximum fleets
function GetExpeditionState(expedition: ExpeditionData): string {
	return expedition.Enabled
		? expedition.Activated
			? "executing" : "waiting"
		: "disabled";
};
class TopExpedition implements IModule {
	private Expeditions: ExpeditionData[];

	init(): void {
		// First fleet cannot go expedition
		for (let i = 1; i < MAX_FLEETS; i++) {
			const data = {
				Enabled: false,
				Activated: false,

				Id: 0,
				RemainingText: "--:--:--",
				Progress: 0
			};

			window.API.ObserveData("Homeport", "Organization.Fleets[" + (i + 1) + "]", value => data.Enabled = value !== null, true);
			window.API.ObserveData("Homeport", "Organization.Fleets[" + (i + 1) + "].Expedition", value => {
				data.Activated = value.IsInExecution;
				data.Id = Mission.DisplayNo;
				data.RemainingText = value.RemainingText;
				data.progress = value.Progress ? value.Progress.Current * 100 / value.Progress.Maximum : 0;
			});
			topexp.Expeditions.push(data);
		}

		Homeport.Instance!.Observe((name, value: Fleet[]) => {
			if (!value) return;
			this.Expeditions.forEach(x => x.Enabled = false);
			value.length
		}, "Fleets");

		window.modules.areas.register("top", "top-expedition", "Expeditions bar", "", topexp);
	}
}

const topexp = new Vue({
	data: {
		Expeditions: []
	},
	el: $("#top-expeditions")[0],
	methods: {
		ExpeditionState: GetExpeditionState
	}
});

window.modules.register("top-expedition", new TopExpedition());
export default TopExpedition;