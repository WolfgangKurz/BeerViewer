import { Admiral } from "./Admiral";
import { Materials } from "./Materials";
import { Ship } from "./Ship";
import { kcsapi_basic } from "../Interfaces/kcsapi_basic";
import { RepairDock } from "./RepairDock";
import { SubscribeKcsapi } from "../Base/KcsApi";
import { kcsapi_port } from "../Interfaces/kcsapi_port";
import { kcsapi_require_info } from "../Interfaces/kcsapi_require_info";
import { Observable } from "../Base/Observable";
import { Fleet } from "./Fleet";
import { kcsapi_mission_result } from "../Interfaces/kcsapi_mission_result";
import { kcsapi_ship2, kcsapi_ship3, kcsapi_destroyship, kcsapi_ship_deck } from "../Interfaces/kcsapi_ship";
import { kcsapi_deck } from "../Interfaces/kcsapi_deck";
import { kcsapi_charge } from "../Interfaces/kcsapi_charge";
import { kcsapi_powerup } from "../Interfaces/kcsapi_powerup";
import { kcsapi_slot_exchange_index, kcsapi_slot_deprive } from "../Interfaces/kcsapi_slot";
import { kcsapi_kdock_getship } from "../Interfaces/kcsapi_kdock";
import { kcsapi_hensei_combined } from "../Interfaces/kcsapi_hensei";
import { CombinedFleetType } from "../Enums/CombinedFleetType";
import { Master } from "../Master/Master";
import { IdentifiableTable } from "../Models/TableWrapper";

export class Homeport extends Observable {
    public static readonly Instance: Homeport = new Homeport;

    public Admiral: Admiral | null = null;
    public Materials: Materials | null = null;
    public RepairDock: RepairDock | null = null;

    public FleetCombinedType: CombinedFleetType = CombinedFleetType.NotCombined;
    public FleetCombined: boolean = false;
    public Fleets: IdentifiableTable<Fleet> = new IdentifiableTable<Fleet>();
    public Ships: Ship[] = [];

    private _Ready: Function;

    constructor() {
        super();
        this._Ready = function () {
        };
    }
    public Ready(): void {
        this.Materials = new Materials();
        this.RepairDock = new RepairDock(this);
        this.Fleets = new IdentifiableTable<Fleet>();
        // this.ConstructionDock = new ConstructionDock(this);

        // this.Itemyard = new Itemyard(this);
        // this.Organization = new Organization(this);
        // this.Quests = new Quests();

        SubscribeKcsapi<kcsapi_require_info>("api_get_member/require_info", x => {
            this.Admiral = new Admiral(x.api_basic);
            // this.Itemyard.Update(x.api_slot_item);
            // this.Dockyard.Update(x.api_kdock);
        });
        SubscribeKcsapi<kcsapi_port>("api_port", x => {
            this.Admiral = new Admiral(x.api_basic);
            this.RepairDock!.Update(x.api_ndock);

            /*
            this.Organization.Update(x.api_ship);
            this.Organization.Update(x.api_deck_port);
            this.Organization.Combined = x.api_combined_flag != 0;
            */
            this.Materials!.Update(x.api_material);
        });
        SubscribeKcsapi<kcsapi_basic>("api_get_member/basic", x => this.Admiral = new Admiral(x));


        // For fleets
        SubscribeKcsapi<kcsapi_ship2[]>("api_get_member/ship", x => this.Update(x));
        SubscribeKcsapi<kcsapi_ship2[]>("api_get_member/ship2", (x, _, raw) => {
            this.Update(x);
            this.Update(raw.api_deck_data);
        });
        SubscribeKcsapi<kcsapi_ship3>("api_get_member/ship3", (x, _, raw) => {
            this.Update(x.api_ship_data);
            this.Update(x.api_deck_data);
        });

        SubscribeKcsapi<kcsapi_deck[]>("api_get_member/deck", x => this.Update(x));
        SubscribeKcsapi<kcsapi_deck[]>("api_get_member/deck_port", x => this.Update(x));
        SubscribeKcsapi<kcsapi_ship_deck>("api_get_member/ship_deck", x => this.Update(x));
        SubscribeKcsapi<kcsapi_deck>("api_req_hensei_preset/select", x => this.Update(x));

        SubscribeKcsapi("api_req_hensei/change", () => this.Change());
        SubscribeKcsapi<kcsapi_charge>("api_req_hokyu/charge", x => this.Charge(x));
        SubscribeKcsapi<kcsapi_powerup>("api_req_kaisou/powerup", () => this.Powerup());
        SubscribeKcsapi<kcsapi_slot_exchange_index>("api_req_kaisou/slot_exchange_index", () => this.ExchangeSlot());
        SubscribeKcsapi<kcsapi_slot_deprive>("api_req_kaisou/slot_deprive", x => this.DepriveSlotItem(x));

        SubscribeKcsapi<kcsapi_kdock_getship>("api_req_kousyou/getship", x => this.GetShip(x));
        SubscribeKcsapi<kcsapi_destroyship>("api_req_kousyou/destroyship", () => this.DestoryShip());
        SubscribeKcsapi("api_req_member/updatedeckname", () => this.UpdateFleetName());

        SubscribeKcsapi<kcsapi_hensei_combined>("api_req_hensei/combined", (x, y) => {
            this.FleetCombined = x.api_combined != 0;
            this.FleetCombinedType = <CombinedFleetType>parseInt(y["api_combined_type"].toString());
        });
    }
}