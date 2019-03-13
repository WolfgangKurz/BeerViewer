/** Types of ship */
export enum ShipType {
	None = 0,
	// Undefined = 0,

	// DE = 1,
	CoastalDefenseShip = 1,
	// 海防艦 = 1,

	// DD = 2,
	Destroyer = 2,
	// 駆逐艦 = 2,

	// CL = 3,
	LightCruiser = 3,
	// 軽巡洋艦 = 3,
	// 軽巡 = 3,

	// CLT = 4,
	TorpedoCruiser = 4,
	// 重雷装巡洋艦 = 4,
	// 重雷装艦 = 4,

	// CA = 5,
	HeavyCruiser = 5,
	// 重巡洋艦 = 5,
	// 重巡 = 5,

	// CAV = 6,
	AviationCruiser = 6,
	// 航空巡洋艦 = 6,
	// 航巡 = 6,

	// CVL = 7,
	LightAircraftCarrier = 7,
	// 軽航空母艦 = 7,
	// 軽空母 = 7,

	// FBB = 8,
	FastBattleship = 8,
	// 高速戦艦 = 8,

	// BB = 9,
	Battleship = 9,
	// 戦艦 = 9,

	// BBV = 10,
	AviationBattleship = 10,
	// 航空戦艦 = 10,

	// CV = 11,
	// AircraftCarrier = 11,
	RegularAircraftCarrier = 11,
	// 正規航空母艦 = 11,
	// 正規空母 = 11,

	// SuperDreadnoughtClassBattleship = 12,
	// SuperDreadnoughtBattleship = 12,
	// 超弩級戦艦 = 12,

	// SS = 13,
	Submarine = 13,
	// 潜水艦 = 13,

	// SSV = 14,
	SubmarineAircraftCarrier = 14,
	// 潜水航空母艦 = 14,
	// 潜水空母 = 14,

	// EnemyAO = 15,
	// AO_Enemy = 15,
	EnemyFleetOiler = 15,
	// FleetOiler_Enemy = 15,
	// 敵補給船 = 15,
	// 補給船_敵 = 15,

	// AV = 16,
	SeaplaneTender = 16,
	// 水上機母艦 = 16,
	// 水母艦 = 16,

	// LHA = 17,
	AmphibiousAssaultShip = 17,
	// 強襲揚陸艦 = 17,
	// 揚陸艦 = 17,

	// CVB = 18,
	ArmoredAircraftCarrier = 18,
	// 装甲航空母艦 = 18,
	// 装甲空母 = 18,

	// AR = 19,
	RepairShip = 19,
	// 工作艦 = 19,

	// AS = 20,
	SubmarineTender = 20,
	// 潜水母艦 = 20,

	// CT = 21,
	TrainingCruiser = 21,
	// 練習巡洋艦 = 21,
	// 練巡 = 21,

	// AO = 22,
	// AO_Alias = 22,
	// Alias_AO = 22,
	FleetOiler = 22,
	// FleetOiler_Alias = 22,
	// Alias_FleetOiler = 22,
	//補給船 = 22,
	// 味方補給船 = 22,
	// 補給船_味方 = 22
}

/** Speed of ship */
export enum ShipSpeed {
	None = 0,
	Immovable = 0,
	// LandBased = 0,
	// 移動不可 = 0,
	// 陸上基地 = 0,

	Slow = 5,
	// 低速 = 5,

	Fast = 10,
	// 高速 = 10,

	/** Fast+, 高速+ */
	Faster = 15,
	// FastPlus = 15,
	// 上高速 = 15,
	// 高速上 = 15,
	// 高速プラス = 15,

	Fastest = 20,
	// 最高速 = 20,
	// 最速 = 20
}