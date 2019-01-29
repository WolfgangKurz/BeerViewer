export enum UseItemType {
	None = 0,
	NotConsumable = 0,

	InstantRepairMaterial = 1,
	InstantRepairBucket = 1,
	RepairBucket = 1,

	InstantConstructionMaterial = 2,
	DevelopmentMaterial = 3,

	Consumable = 4,

	Resources = 6
}

export enum UseItemCategory {
	None = 0,
	NotCategorized = 0,

	InstantRepairMaterial = 1,
	InstantRepairBucket = 1,
	RepairBucket = 1,

	InstantConstructionMaterial = 2,

	DevelopmentMaterial = 3,

	EquipmentImprovementMaterial = 4,
	ImprovementMaterial = 4,

	FurnitureCoinBox = 6,
	CoinBox = 6,

	Fuel = 16,
	Ammo = 17,
	Steel = 18,
	Bauxite = 19,
	FurnitureCoin = 21
}