import { RawDataWrapper } from "System/Base/Wrapper";
import { kcsapi_mst_ship } from "System/Interfaces/Master/kcsapi_mst_ship";
import { IIdentifiable } from "System/Base/Interfaces/IIdentifiable";
import { ShipType, ShipSpeed } from "System/Enums/ShipEnums";
import { Master } from "System/Master/Master";
import { MasterMinMax } from "System/Master/MasterMinMax";
import { ShipTypeInfo } from "./ShipTypeInfo";

export class ShipInfo extends RawDataWrapper<kcsapi_mst_ship> implements IIdentifiable {
    //#region Wrapping
    public get Id(): number { return this.raw.api_id }
    public get SortId(): number { return this.raw.api_sortno }

    public get Name(): string { return this.raw.api_name }
    public get ShipType(): ShipTypeInfo | null { return Master.Instance.ShipTypes!.get(this.raw.api_stype) || null }

    public get Slots(): number { return this.raw.api_slot_num }
    public get SlotAircraftCounts(): number[] { return this.raw.api_maxeq }
    public get Speed(): ShipSpeed { return this.raw.api_soku }

    public get NextRemodelLevel(): number | null { return this.raw.api_afterlv || null }

    public get HP(): MasterMinMax { return new MasterMinMax(this.raw.api_taik) }
    public get Armor(): MasterMinMax { return new MasterMinMax(this.raw.api_souk) }
    public get FirePower(): MasterMinMax { return new MasterMinMax(this.raw.api_houg) }
    public get Torpedo(): MasterMinMax { return new MasterMinMax(this.raw.api_raig) }
    public get AA(): MasterMinMax { return new MasterMinMax(this.raw.api_tyku) }
    public get Luck(): MasterMinMax { return new MasterMinMax(this.raw.api_luck) }

    public get Fuel(): number { return this.raw.api_fuel_max }
    public get Ammo(): number { return this.raw.api_bull_max }
    //#endregion

    public IsAircraftShipType(): boolean { return this.SlotAircraftCounts.some(x => x > 0) }

    constructor(api_data: kcsapi_mst_ship) {
        super(api_data);
    }

    public toString(): string {
        return `{"Id": ${this.Id}, "Name": "${this.Name}", "ShipType": ${this.ShipType}}`
    }

    public static readonly Empty: ShipInfo = new ShipInfo({
        api_id: 0,
        api_sortno: 0,
        api_name: "???",
        api_yomi: "???",
        api_stype: 0,
        api_afterlv: 0,
        api_aftershipid: "",
        api_taik: [0, 0],
        api_souk: [0, 0],
        api_houg: [0, 0],
        api_raig: [0, 0],
        api_tyku: [0, 0],
        api_luck: [0, 0],
        api_soku: 0,
        api_leng: 0,
        api_slot_num: 0,
        api_maxeq: [],
        api_buildtime: 0,
        api_broken: [0, 0, 0, 0],
        api_powup: [0, 0, 0, 0],
        api_backs: 0,
        api_getmes: "",
        api_afterfuel: 0,
        api_afterbull: 0,
        api_fuel_max: 0,
        api_bull_max: 0,
        api_voicef: 0
    });
}