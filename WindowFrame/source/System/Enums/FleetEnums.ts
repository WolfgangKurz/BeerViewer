export enum FleetState {
    /** No ships in fleet */
    Empty = 0,

    /** Fleet in Homeport */
    Homeport = 1,

    /** Combined fleet */
    Combined = 1 << 1,

    /** Fleet sailing */
    Sailing = 1 << 2,

    /** Fleet in expedition */
    Expedition = 1 << 3,

    /** Heavily damaged ship exists in fleet */
    HeavilyDamaged = 1 << 4,

    /** Ship that not supplied fully exists in fleet */
    NeedSupply = 1 << 5,

    /** Repairing ship exists in fleet */
    Repairing = 1 << 6,

    /** Flagship is Repair Ship (Akashi) */
    FlagshipRepairShip = 1 << 7,

    /** Fleet condition restoring */
    ConditionRestoring = 1 << 8
}