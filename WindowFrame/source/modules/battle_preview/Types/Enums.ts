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
export enum Formation {
	Unknown = -1,
	None = 0,
	LineAhead = 1,
	DoubleLine = 2,
	Diamond = 3,
	Echelon = 4,
	LineAbreast = 5,
	Vanguard = 6,

	AntiSubmarine = 11,
	Forward = 12,
	Ring = 13,
	Battle = 14,
}

export enum FleetType {
	None = 0,
	AliasFirst,
	AliasSecond,
	EnemyFirst,
	EnemySecond
}
export enum BattleEngage {
	None = 0,
	Parallel = 1,
	HeadOn = 2,
	T_Advantage = 3,
	T_Disadvantage = 4
}
