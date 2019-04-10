export enum EventId {
	None = 0,
	Obtain = 2,
	Loss = 3,
	NormalBattle = 4,
	BossBattle = 5,
	NoEvent = 6,
	AirEvent = 7,
	Escort = 8,
	TP = 9,
	LDAirBattle = 10
}
export enum EventKind {
	None = 0,

	/** Day battle */
	Battle = 1,

	/** Night only battle */
	NightBattle = 2,

	/** Night to Day battle */
	NightToDayBattle = 3,

	/** Air battle */
	AirBattle = 4,

	/** Enemy combined, Day battle */
	EnemyCombinedBattle = 5,

	/** Out-range Air battle */
	LongDistanceAirBattle = 6,

	/** Enemy combined, Night to Day battle */
	EnemyCombinedNightToDayBattle = 7,

	AirSearch = 0,
	Selectable = 2
}