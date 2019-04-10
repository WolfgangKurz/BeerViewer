export enum MapDifficulty {
	None,
	/** 丁 */
	Casual,
	/** 丙 */
	Easy,
	/** 乙 */
	Normal,
	/** 甲 */
	Hard
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

export enum BattleRank {
	Error = -1,
	None = 0,

	S_Perfect = 1,
	S_Victory = 2,
	A_Victory = 3,
	B_Victory = 4,
	C_Defeat = 5,
	D_Defeat = 6,
	E_Defeat = 7,

	AirRaid = 8
}
export enum AirSupremacy {
	None = -1,
	Parity = 0,
	Supremacy = 1,
	Superiority = 2,
	Denial = 3,
	Incapability = 4

}