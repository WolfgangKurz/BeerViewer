import { SubscribeKcsapi } from "System/Base/KcsApi";
import { kcsapi_start2 } from "System/Interfaces/Master/kcsapi_start2";
import { ShipType } from "System/Enums/ShipEnums";
import { ShipInfo } from "./Wrappers/ShipInfo";
import { ShipTypeInfo } from "./Wrappers/ShipTypeInfo";
import { IIdentifiable } from "System/Base/Interfaces/IIdentifiable";
import { EquipTypeInfo } from "./Wrappers/EquipTypeInfo";
import { EquipInfo } from "./Wrappers/EquipInfo";
import { ExpeditionInfo } from "./Wrappers/ExpeditionInfo";
import { MapWorldInfo, MapAreaInfo } from "./Wrappers/MapInfos";
import { UseItemInfo } from "./Wrappers/UseItemInfo";
import { MasterWrapper } from "System/Models/TableWrapper";

export class Master {
    public static get Instance(): Master { return window.Master }
    private IsReady: boolean = false;

    public ShipTypes: MasterWrapper<ShipTypeInfo> | null = null;
    public Ships: MasterWrapper<ShipInfo> | null = null;

    public EquipTypes: MasterWrapper<EquipTypeInfo> | null = null;
    public Equips: MasterWrapper<EquipInfo> | null = null;

    public UseItems: MasterWrapper<UseItemInfo> | null = null;

    public Expeditions: MasterWrapper<ExpeditionInfo> | null = null;

    public MapWorlds: MasterWrapper<MapWorldInfo> | null = null;
    public MapAreas: MasterWrapper<MapAreaInfo> | null = null;

    public Ready(): Master {
        if (this.IsReady) return this;
        this.IsReady = true;

        SubscribeKcsapi<kcsapi_start2>("api_start2/getData", x => {
            this.ShipTypes = new MasterWrapper<ShipTypeInfo>(x.api_mst_stype.map(y => new ShipTypeInfo(y)));
            this.Ships = new MasterWrapper<ShipInfo>(x.api_mst_ship.map(y => new ShipInfo(y)));

            this.EquipTypes = new MasterWrapper<EquipTypeInfo>(x.api_mst_slotitem_equiptype.map(y => new EquipTypeInfo(y)));
            this.Equips = new MasterWrapper<EquipInfo>(x.api_mst_slotitem.map(y => new EquipInfo(y)));

            this.Expeditions = new MasterWrapper<ExpeditionInfo>(x.api_mst_mission.map(y => new ExpeditionInfo(y)));

            this.MapWorlds = new MasterWrapper<MapWorldInfo>(x.api_mst_maparea.map(y => new MapWorldInfo(y)));
            this.MapAreas = new MasterWrapper<MapAreaInfo>(x.api_mst_mapinfo.map(y => new MapAreaInfo(y)));
        });
        return this;
    }
}
