export enum QuestCategory {
	Composition = 1,
	編成 = 1,

	Sortie = 2,
	出撃 = 2,

	Practice = 3,
	練習 = 3,

	Expeditions = 4,
	遠征 = 4,

	Supply = 5,
	補給 = 5,

	Construction = 6,
	建造 = 6,

	Remodeling = 7,
	改修 = 7,

	Sortie2 = 8,
	出撃2 = 8,

	Other = 9,
	その他 = 9
}
export enum QuestProgress {
	None = 0,
	Progress50,
	Progress80,
	Complete
}
export enum QuestState {
	None = 1,
	TakeOn = 2,
	Complete = 3,
}
export enum QuestType {
	Daily = 1,
	Weekly = 2,
	Monthly = 3,
	OneTime = 4,
	Other = 5,
	Quarterly = Other,
}