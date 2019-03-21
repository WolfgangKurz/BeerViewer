import { RawDataWrapper } from "System/Base/Wrapper";
import { IIdentifiable } from "System/Base/Interfaces/IIdentifiable";
import { QuestCategory, QuestProgress, QuestState, QuestType } from "System/Enums/Quests";
import { kcsapi_quest } from "System/Interfaces/kcsapi_quest";

class Quest extends RawDataWrapper<kcsapi_quest> implements IIdentifiable {
	public get Id(): number { return this.raw.api_no }

	public get Category(): QuestCategory { return this.raw.api_category }
	public get Type(): QuestType { return this.raw.api_type }
	public get State(): QuestState { return this.raw.api_state }
	public get Progress(): QuestProgress { return this.raw.api_progress_flag }

	public get Title(): string { return this.raw.api_title }
	public get Description(): string { return this.raw.api_detail.replace(/<br>/g, "\r\n") }

	constructor(api_data: kcsapi_quest) {
		super(api_data);
	}
}
export default Quest;