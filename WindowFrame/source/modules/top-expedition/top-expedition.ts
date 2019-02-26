/// <reference path="../../../node_modules/ts-nameof/ts-nameof.d.ts" />
import Vue from "vue";
import { IModule } from "System/Module";
import { Homeport } from "System/Homeport/Homeport";
import { IdentifiableTable } from "System/Models/TableWrapper";
import { Progress } from "System/Models/GuageValue";

interface ExpeditionData {
	Enabled: boolean;
	Activated: boolean;

	Id: number;
	RemainingText: string;
	Progress: number;
}

class TopExpedition implements IModule {
	private readonly MAX_FLEETS = 4; // Maximum fleets

	private Expedition: IdentifiableTable<ExpeditionData> | null = null;
	private Data = {
		Expeditions: <ExpeditionData[]>[]
	};

	private VueObject = Vue.component("top-expedition-component", {
		data: () => this.Data,
		template: $("#top-expeditions").prop("outerHTML"),
		methods: {
			ExpeditionState(exp: ExpeditionData): string {
				return exp.Enabled
					? exp.Activated
						? "executing" : "waiting"
					: "disabled";
			}
		}
	});

	init(): void {
		// First fleet cannot go expedition
		const list: ExpeditionData[] = [];
		for (let i = 2; i <= this.MAX_FLEETS; i++) {
			const data: ExpeditionData = {
				Enabled: false,
				Activated: false,

				Id: i,
				RemainingText: "--:--:--",
				Progress: 0
			};
			list.push(data);
		}
		this.Expedition = new IdentifiableTable<ExpeditionData>(list);
		this.Data.Expeditions = this.Expedition.values();

		Homeport.Instance.Observe(() => this.Reload(), nameof(Homeport.Instance.Fleets));

		window.modules.areas.register("top", "top-expedition", "Expeditions bar", "", "top-expedition-component");
	}

	private Reload(): void {
		Homeport.Instance.Fleets.forEach(x => {
			const id = x.Id; // Begin at 2nd fleet
			const target = this.Expedition!.get(id);
			if (!target) return;
			target.Enabled = x ? true : false;

			if (x && x.Expedition) {
				const exp = x.Expedition;
				exp.Observe((_, value: boolean) => target.Activated = value, nameof(exp.IsInExecution));
				exp.Observe((_, value: number) => target.Id = value, nameof(exp.Id));
				exp.Observe((_, value: number) => target.RemainingText = this.GetRemainingText(value), nameof(exp.Remaining));
				exp.Observe((_, value: Progress) => {
					target.Progress = value.Maximum > 0
						? value.Percentage * 100 : 0;
				}, nameof(exp.Progress));
			}
			this.Data.Expeditions = this.Expedition!.values(); // Update?
		});
	}

	private GetRemainingText(remaining: number): string {
		const asSecs = Math.floor(remaining / 1000);
		const hours = Math.floor((asSecs / 3600) % 60);
		const mins = Math.floor((asSecs / 60) % 60);
		const secs = (asSecs % 60);
		return `${("0" + hours).slice(-2)}:${("0" + mins).slice(-2)}:${("0" + secs).slice(-2)}`;
	}
}

window.modules.register("top-expedition", new TopExpedition());
export default TopExpedition;
