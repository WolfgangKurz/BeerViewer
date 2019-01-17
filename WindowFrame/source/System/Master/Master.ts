import { SubscribeKcsapi } from "../Base/KcsApi";
import { kcsapi_start2 } from "../Interfaces/Master/kcsapi_start2";
import { ShipType } from "../Enums/ShipEnums";
import { ShipInfo } from "./Wrappers/ShipInfo";
import { ShipTypeInfo } from "./Wrappers/ShipTypeInfo";
import { IIdentifiable } from "../Base/Interfaces/IIdentifiable";
import { EquipTypeInfo } from "./Wrappers/EquipTypeInfo";
import { EquipInfo } from "./Wrappers/EquipInfo";
import { ExpeditionInfo } from "./Wrappers/ExpeditionInfo";
import { MapWorldInfo, MapAreaInfo } from "./Wrappers/MapInfos";
import { UseItemInfo } from "./Wrappers/UseItemInfo";

export type MasterWrapper<T> = ReadonlyMap<number, T>;

export class Master {
    public static Instance: Master = new Master();
    private IsReady: boolean = false;

    public ShipTypes: MasterWrapper<ShipTypeInfo> | null = null;
    public Ships: MasterWrapper<ShipInfo> | null = null;

    public EquipTypes: MasterWrapper<EquipTypeInfo> | null = null;
    public Equips: MasterWrapper<EquipInfo> | null = null;

    public UseItems: MasterWrapper<UseItemInfo> | null = null;

    public Expeditions: MasterWrapper<ExpeditionInfo> | null = null;

    public MapWorlds: MasterWrapper<MapWorldInfo> | null = null;
    public MapAreas: MasterWrapper<MapAreaInfo> | null = null;

    public Ready(): void {
        if (this.IsReady) return;
        this.IsReady = true;

        SubscribeKcsapi<kcsapi_start2>("/api_start2", x => {
            this.ShipTypes = this.WrapMaster<ShipTypeInfo>(x.api_mst_stype.map(y => new ShipTypeInfo(y)));
            this.Ships = this.WrapMaster<ShipInfo>(x.api_mst_ship.map(y => new ShipInfo(y)));

            this.EquipTypes = this.WrapMaster<EquipTypeInfo>(x.api_mst_slotitem_equiptype.map(y => new EquipTypeInfo(y)));
            this.Equips = this.WrapMaster<EquipInfo>(x.api_mst_slotitem.map(y => new EquipInfo(y)));

            this.Expeditions = this.WrapMaster<ExpeditionInfo>(x.api_mst_mission.map(y => new ExpeditionInfo(y)));

            this.MapWorlds = this.WrapMaster<MapWorldInfo>(x.api_mst_maparea.map(y => new MapWorldInfo(y)));
            this.MapAreas = this.WrapMaster<MapAreaInfo>(x.api_mst_mapinfo.map(y => new MapAreaInfo(y)));
        });
    }

    private WrapMaster<T extends IIdentifiable>(array: T[]) {
        return new Map(
            array.map(x => [x.Id, x] as [number, T])
        );
    }
}