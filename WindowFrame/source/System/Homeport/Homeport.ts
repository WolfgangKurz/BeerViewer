import { Admiral } from "./Admiral";
import { Materials } from "./Materials";
import { Ship } from "./Ship";
import { kcsapi_basic } from "../Interfaces/kcsapi_basic";
import { RepairDock } from "./RepairDock";
import { SubscribeKcsapi } from "../Base/KcsApi";
import { kcsapi_port } from "../Interfaces/kcsapi_port";
import { kcsapi_require_info } from "../Interfaces/kcsapi_require_info";
import { Observable } from "../Base/Observable";

export class Homeport extends Observable {
    public static readonly Instance: Homeport = new Homeport;

    public Admiral: Admiral | null = null;
    public Materials: Materials | null = null;
    public RepairDock: RepairDock | null = null;

    public Ships: Ship[] = [];

    public Ready(): void {
        this.Materials = new Materials();
        this.RepairDock = new RepairDock(this);
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
    }
}