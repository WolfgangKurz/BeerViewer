import { Observable } from "System/Base/Observable";
import Quest from "./Quest";
import { SubscribeKcsapi } from "System/Base/KcsApi";
import { kcsapi_questlist, kcsapi_req_quest_requests } from "System/Interfaces/kcsapi_quest";
import { QuestState, QuestCategory, QuestType } from "System/Enums/Quests";

class QuestManager extends Observable {
	private currentQuests: Quest[] = [];

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

		this.currentQuests = this.currentQuests
			.filter(x => !questList.api_list.some(y => y.api_no === x.Id))
			.concat(
				questList.api_list
					.filter(x => x.api_state === QuestState.Complete || x.api_state === QuestState.TakeOn)
					.map(x => new Quest(x))
			);
		this.Publish();
	}

	private ClearQuest(q_id: number): void {
		this.currentQuests = this.currentQuests.filter(x => x.Id !== q_id);
		// TODO for additional processing
		this.Publish();
	}
	private StopQuest(q_id: number): void {
		this.currentQuests = this.currentQuests.filter(x => x.Id !== q_id);
		// TODO for additional processing
		this.Publish();
	}

	private Publish(): void {
		this.currentQuests.sort((x, y) => x.Id === y.Id ? 0 : (x.Id < y.Id ? -1 : 1));
		this.$._All = this.currentQuests;
		this.RaisePropertyChanged(nameof(this.All));
	}
}
export default QuestManager;