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
import { kcsapi_ship2, kcsapi_ship3, kcsapi_destroyship, kcsapi_ship_deck, kcsapi_req_kousyou_destroyship } from "../Interfaces/kcsapi_ship";
import { kcsapi_deck, kcsapi_req_hensei_combined } from "../Interfaces/kcsapi_deck";
import { kcsapi_charge } from "../Interfaces/kcsapi_charge";
import { kcsapi_powerup, kcsapi_req_kaisou_powerup } from "../Interfaces/kcsapi_powerup";
import { kcsapi_slot_exchange_index, kcsapi_slot_deprive, kcsapi_req_kaisou_slot_exchange_index } from "../Interfaces/kcsapi_slot";
import { kcsapi_hensei_combined, kcsapi_req_hensei_change } from "../Interfaces/kcsapi_hensei";
import { CombinedFleetType } from "../Enums/CombinedFleetType";
import { IdentifiableTable } from "../Models/TableWrapper";
import { HTTPRequest } from "../Exports/API";
import { Equipments } from "./Equipment/Equipments";
import { ConstructionDock as ConstructionDock } from "./ConstructionDock";
import { kcsapi_kdock_getship } from "../Interfaces/kcsapi_dock";

export class Homeport extends Observable {
    public static get Instance(): Homeport { return window.Homeport }

    //#region Admiral
    private _Admiral: Admiral | null = null;
    public get Admiral(): Admiral | null { return this._Admiral }
    //#endregion

    //#region Materials
    private _Materials: Materials | null = null;
    public get Materials(): Materials | null { return this._Materials }
    //#endregion

    //#region RepairDock
    private _RepairDock: RepairDock | null = null;
    public get RepairDock(): RepairDock | null { return this._RepairDock }
    //#endregion

    //#region ConstructionDock
    private _ConstructionDock: ConstructionDock | null = null;
    public get ConstructionDock(): ConstructionDock | null { return this._ConstructionDock }
    //#endregion


    //#region FleetCombinedType
    private _FleetCombinedType: CombinedFleetType = CombinedFleetType.NotCombined;
    public get FleetCombinedType(): CombinedFleetType { return this._FleetCombinedType }
    //#endregion

    //#region FleetCombined
    private _FleetCombined: boolean = false;
    public get FleetCombined(): boolean { return this._FleetCombined }
    //#endregion

    //#region Fleets
    private _Fleets: IdentifiableTable<Fleet> = new IdentifiableTable<Fleet>();
    public get Fleets(): IdentifiableTable<Fleet> { return this._Fleets }
    //#endregion

    //#region Ships
    private _Ships: IdentifiableTable<Ship> = new IdentifiableTable<Ship>();
    public get Ships(): IdentifiableTable<Ship> { return this._Ships }
    //#endregion


    //#region Equipments
    private _Equipments: Equipments = new Equipments();
    public get Equipments(): Equipments { return this._Equipments }
    //#endregion


    constructor() {
        super();
    }

    public Ready(): Homeport {
        this.$._Materials = new Materials();
        this.$._RepairDock = new RepairDock(this);
        this.$._ConstructionDock = new ConstructionDock(this);
        this.$._Equipments = new Equipments();

        this.$._Ships = new IdentifiableTable<Ship>();
        this.$._Fleets = new IdentifiableTable<Fleet>();

        // this.Quests = new Quests();

        SubscribeKcsapi<kcsapi_require_info>("api_get_member/require_info", x => {
            if(this._Admiral) this._Admiral.Dispose();
            this.$._Admiral = new Admiral(x.api_basic);
            this.Equipments.Update(x.api_slot_item);
            this.ConstructionDock!.Update(x.api_kdock);
        });
        SubscribeKcsapi<kcsapi_port>("api_port/port", x => {
            if(this._Admiral) this._Admiral.Dispose();
            this.$._Admiral = new Admiral(x.api_basic);
            this.RepairDock!.Update(x.api_ndock);

            this.UpdateShips(x.api_ship);
            this.UpdateDecks(x.api_deck_port);
            this.$._FleetCombined = x.api_combined_flag !== 0;

            this.Materials!.Update(x.api_material);
        });
        SubscribeKcsapi<kcsapi_basic>("api_get_member/basic", x => {
            if(this._Admiral) this._Admiral.Dispose();
            this.$._Admiral = new Admiral(x);
        });


        // For fleets
        SubscribeKcsapi<kcsapi_ship2[]>("api_get_member/ship", x => this.UpdateShips(x));
        SubscribeKcsapi<kcsapi_ship2[]>("api_get_member/ship2", (x, _, raw) => {
            this.UpdateShips(x);
            this.UpdateShips(raw.api_deck_data);
        });
        SubscribeKcsapi<kcsapi_ship3>("api_get_member/ship3", (x, _, raw) => {
            this.UpdateShips(x.api_ship_data);
            this.UpdateDecks(x.api_deck_data);
        });

        SubscribeKcsapi<kcsapi_deck[]>("api_get_member/deck", x => this.UpdateDecks(x));
        SubscribeKcsapi<kcsapi_deck[]>("api_get_member/deck_port", x => this.UpdateDecks(x));
        SubscribeKcsapi<kcsapi_deck>("api_req_hensei_preset/select", x => this.UpdateDeck(x));
        SubscribeKcsapi<kcsapi_ship_deck>("api_get_member/ship_deck", x => this.UpdateShipDeck(x));

        SubscribeKcsapi<{}, kcsapi_req_hensei_change>("api_req_hensei/change", (x, y) => this.ChangeComposite(y));
        SubscribeKcsapi<kcsapi_charge>("api_req_hokyu/charge", x => this.SupplyShip(x));
        SubscribeKcsapi<kcsapi_powerup, kcsapi_req_kaisou_powerup>(
            "api_req_kaisou/powerup", (x, y) => this.ImproveShip(x, y)
        );
        SubscribeKcsapi<kcsapi_slot_exchange_index, kcsapi_req_kaisou_slot_exchange_index>(
            "api_req_kaisou/slot_exchange_index", (x, y) => this.ExchangeEquip(x, y)
        );
        SubscribeKcsapi<kcsapi_slot_deprive>("api_req_kaisou/slot_deprive", x => this.DepriveEquipment(x));

        SubscribeKcsapi<kcsapi_kdock_getship>("api_req_kousyou/getship", x => this.GetShipFromConstruction(x));
        SubscribeKcsapi<kcsapi_destroyship, kcsapi_req_kousyou_destroyship>(
            "api_req_kousyou/destroyship", (x, y) => this.DestroyShip(x, y)
        );

        SubscribeKcsapi<kcsapi_hensei_combined, kcsapi_req_hensei_combined>("api_req_hensei/combined", (x, y) => {
            this.$._FleetCombined = x.api_combined != 0;
            this.$._FleetCombinedType = <CombinedFleetType>y.api_combined_type;
        });

        return this;
    }

    private UpdateDeck(source: kcsapi_deck): void {
        const fleet = this.Fleets.get(source.api_id);
        if (!fleet) return;

        fleet.Update(source);
    }
    private UpdateDecks(source: kcsapi_deck[]): void {
        if (this.Fleets.size === source.length) {
            for (const raw of source)
                this.Fleets.get(raw.api_id)!.Update(raw);
        } else {
            this.Fleets.forEach(x => x.Dispose());
            this.$._Fleets = new IdentifiableTable<Fleet>(source.map(x => new Fleet(this, x)));
        }
    }
    private UpdateShips(source: kcsapi_ship2[]): void {
        if (source.length <= 1) {
            for (const ship of source) {
                const target = this.Ships.get(ship.api_id);
                if (!target) continue;

                target.Update(ship);
            }
        } else {
            const shipsEvacuated: number[] = this._Ships.values().filter(x => x.State & Ship.State.Evacuation).map(x => x.Id);
            const shipsTow: number[] = this._Ships.values().filter(x => x.State & Ship.State.Tow).map(x => x.Id);

            this.$._Ships = new IdentifiableTable<Ship>(
                source.map(x => new Ship(this, x))
            );

            for (const id of shipsEvacuated) {
                const ship = this._Ships.get(id);
                if (ship) ship.Evacuate();
            }
            for (const id of shipsTow) {
                const ship = this._Ships.get(id);
                if (ship) ship.Tow();
            }
        }
    }
    private UpdateShipDeck(source: kcsapi_ship_deck): void {
        if (source.api_ship_data) {
            const shipsEvacuated: number[] = this._Ships.values().filter(x => x.State & Ship.State.Evacuation).map(x => x.Id);
            const shipsTow: number[] = this._Ships.values().filter(x => x.State & Ship.State.Tow).map(x => x.Id);

            for (const ship of source.api_ship_data) {
                const target = this.Ships.get(ship.api_id);
                if (!target) continue;

                target.Update(ship);

                if (shipsEvacuated.some(x => target.Id == x)) target.Evacuate();
                if (shipsTow.some(x => target.Id == x)) target.Tow();
            }
        }

        if (source.api_deck_data) {
            for (const deck of source.api_deck_data) {
                const target = this.Fleets.get(deck.api_id);
                if (!target) continue;
                target.Update(deck);
            }
        }
    }

    private ChangeComposite(data: kcsapi_req_hensei_change): void {
        const id = data.api_id;
        if (!id) return;

        const fleet = this.Fleets.get(id);
        if (!fleet) return;

        const index = data.api_ship_idx;
        const shipId = data.api_ship_id;
        if (index === 0 && shipId === -2) {
            fleet.UnsetAll();
            return;
        }

        const ship = this.Ships.get(shipId);
        if (!ship) {
            fleet.Unset(index);
            return;
        }

        const currentFleet: Fleet | undefined = this.GetFleetFromShipId(ship.Id);
        if (!currentFleet) {
            fleet.Change(index, ship);
            return;
        }

        const currentIndex = currentFleet.Ships.findIndex(x => x.Id === ship.Id);
        const old = fleet.Change(index, ship);
        currentFleet.Change(currentIndex, old);
    }
    private SupplyShip(source: kcsapi_charge): void {
        let fleet: Fleet | undefined = undefined;

        for (const ship of source.api_ship) {
            const target = this.Ships.get(ship.api_id);
            if (!target) continue;

            target.Supply(ship.api_fuel, ship.api_bull, ship.api_onslot);

            if (!fleet)
                fleet = this.GetFleetFromShipId(target.Id);
        }

        if (fleet) {
            fleet.UpdateState();
            fleet.Calculate();
        }
    }
    private ImproveShip(source: kcsapi_powerup, request: kcsapi_req_kaisou_powerup): void {
        const ship = this.Ships.get(source.api_ship.api_id);
        if (ship) ship.Update(source.api_ship);

        var items = request.api_id_items
            .split(',').filter(x => x)
            .map(x => parseInt(x))
            .filter(x => this.Ships.has(x))
            .map(x => <Ship>this.Ships.get(x));

        for (const x of items) {
            this.Equipments.RemoveAllFromShip(x);
            this.Ships.delete(x.Id);
        }

        this.UpdateDecks(source.api_deck);
    }

    private ExchangeEquip(source: kcsapi_slot_exchange_index, request: kcsapi_req_kaisou_slot_exchange_index): void {
        const ship = this.Ships.get(request.api_id);
        if (!ship) return;

        ship.raw.api_slot = source.api_slot;
        ship.UpdateEquipSlots();

        const fleet = this.Fleets.values().find(x => x.Ships.some(y => y.Id === ship.Id));
        if (!fleet) return;

        fleet.Calculate();
    }
    private DepriveEquipment(source: kcsapi_slot_deprive): void {
        // TODO
    }
    private GetShipFromConstruction(source: kcsapi_kdock_getship): void {
        // TODO
    }
    private DestroyShip(source: kcsapi_destroyship, request: kcsapi_req_kousyou_destroyship): void {
        // TODO
    }

    private GetFleetFromShipId(shipId: number): Fleet | undefined {
        return this.Fleets.values()
            .find(x => x.Ships.some(s => s.Id === shipId));
    }
}