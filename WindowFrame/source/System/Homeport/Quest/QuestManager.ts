import { Observable } from "System/Base/Observable";
import Quest from "./Quest";
import { SubscribeKcsapi } from "System/Base/KcsApi";
import { kcsapi_questlist, kcsapi_req_quest_requests } from "System/Interfaces/kcsapi_quest";
import { QuestState } from "System/Enums/Quests";

class QuestManager extends Observable {
	private readonly currentQuests: Quest[] = [];

	//#region All Property
	private _All: Quest[] = [];
	public get All(): ReadonlyArray<Quest> { return this._All }
	//#endregion

	constructor() {
		super();

		SubscribeKcsapi<kcsapi_questlist>("api_get_member/questlist", x => this.Update(x));
		SubscribeKcsapi<{}, kcsapi_req_quest_requests>("api_req_quest/clearitemget", (x, y) => this.ClearQuest(y.api_quest_id));
		SubscribeKcsapi<{}, kcsapi_req_quest_requests>("api_req_quest/stop", (x, y) => this.StopQuest(y.api_quest_id));
	}

	private Update(questList: kcsapi_questlist): void {
		if (questList.api_list === null) return;

		questList.api_list.forEach(quest => {
			for (let i = 0; i < this.currentQuests.length; i++) {
				if (this.currentQuests[i].Id === quest.api_no) {
					this.currentQuests.splice(i, 1);
					i--;
				}
			}

			switch (quest.api_state) {
				/*
				case QuestState.None:
					break;
				*/
				case QuestState.Complete:
				case QuestState.TakeOn:
					if (!this.currentQuests.some(x => x.Id == quest.api_no))
						this.currentQuests.push(new Quest(quest));
					break;
			}
		});
		this.Publish();
	}

	private ClearQuest(q_id: number): void {
		for (let i = 0; i < this.currentQuests.length; i++) {
			if (this.currentQuests[i].Id === q_id) {
				this.currentQuests.splice(i, 1);
				i--;
			}
		}
		// TODO for additional processing
		this.Publish();
	}
	private StopQuest(q_id: number): void {
		for (let i = 0; i < this.currentQuests.length; i++) {
			if (this.currentQuests[i].Id === q_id) {
				this.currentQuests.splice(i, 1);
				i--;
			}
		}
		// TODO for additional processing
		this.Publish();
	}

	private Publish(): void {
		this.$._All = this.currentQuests.sort(x => x.Id);
	}
}
export default QuestManager;