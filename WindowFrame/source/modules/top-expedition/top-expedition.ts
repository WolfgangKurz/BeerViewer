/// <reference path="../../../node_modules/ts-nameof/ts-nameof.d.ts" />
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

const topexp = new Vue({
	data: {
		Expeditions: []
	},
	el: $("#top-expeditions")[0],
	methods: {
		ExpeditionState: GetExpeditionState
	}
});

class TopExpedition implements IModule {
	private Expeditions: ExpeditionData[] = [];

	init(): void {
		// First fleet cannot go expedition
		for (let i = 1; i < MAX_FLEETS; i++) {
			const data: ExpeditionData = {
				Enabled: false,
				Activated: false,

				Id: 0,
				RemainingText: "--:--:--",
				Progress: 0
			};
			this.Expeditions.push(data);
		}

		Homeport.Instance.Observe(() => this.Reload(), nameof(Homeport.Instance.Fleets));

		window.modules.areas.register("top", "top-expedition", "Expeditions bar", "", topexp);
	}

	private Reload(): void {
		Homeport.Instance.Fleets.forEach((x, idx) => {
			const id = idx - 2; // Begin at 2nd fleet
			this.Expeditions[id].Enabled = x ? true : false;

			if (x) {
				x.Observe((name, value, oldValue) => {
					this.Expeditions[id].Activated = value.IsInExecution;
					this.Expeditions[id].Id = value.Id;
					this.Expeditions[id].RemainingText = this.GetRemainingText(value.Remaining);
					this.Expeditions[id].Progress = value.Progress.Maximum > 0
						? value.Progress.Percentage * 100 : 0;
				}, nameof(x.Expedition));
			}
		});
	}

	private GetRemainingText(remaining: number): string {
		const asSecs = Math.floor(remaining);
		const hours = Math.floor((asSecs / 3600) % 60);
		const mins = Math.floor((asSecs / 60) % 60);
		const secs = (asSecs % 60);
		return `${("0" + hours).slice(-2)}:${("0" + mins).slice(-2)}:${("0" + secs).slice(-2)}`;
	}
}

window.modules.register("top-expedition", new TopExpedition());
export default TopExpedition;