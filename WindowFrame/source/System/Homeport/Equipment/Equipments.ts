import { Observable } from "System/Base/Observable";
import { IdentifiableTable } from "System/Models/TableWrapper";
import { Ship } from "System/Homeport/Ship";
import { Equipment } from "./Equipment";
import { SubscribeKcsapi } from "System/Base/KcsApi";
import { kcsapi_slotitem, kcsapi_createitem, kcsapi_destroyitem2, kcsapi_remodel_slot, kcsapi_req_destroyitem2 } from "System/Interfaces/kcsapi_item";
import LogHelper from "System/Base/LogHelper";

export class Equipments extends Observable {
	//#region Equips
	private _Equips: IdentifiableTable<Equipment> = new IdentifiableTable<Equipment>();
	public get Equips(): IdentifiableTable<Equipment> { return this._Equips }
	//#endregion

	public get EquipCount(): number { return this.Equips.size }

	constructor() {
		super();

		SubscribeKcsapi<kcsapi_slotitem[]>("api_get_member/slot_item", x => this.Update(x));

		SubscribeKcsapi<kcsapi_createitem>("api_req_kousyou/createitem", x => this.CreateItem(x));
		SubscribeKcsapi<kcsapi_destroyitem2, kcsapi_req_destroyitem2>(
			"api_req_kousyou/destroyitem2",
			(x, y) => {
				window.API.Log("Equipment {0} destroyed, resource {1} returned.", LogHelper.GetEquipNamesFromList(y.api_slotitem_ids.toString()), LogHelper.MaterialDiff(x.api_get_material));
				this.DestroyItem(y);
			}
		);
		SubscribeKcsapi<kcsapi_remodel_slot>("api_req_kousyou/remodel_slot", x => {
			this.RemoveFromRemodel(x);
			this.RemodelSlotItem(x);
		});
	}

	public Update(source: kcsapi_slotitem[]): void {
		this.$._Equips = new IdentifiableTable<Equipment>(source.map(x => new Equipment(x)));
		this.EquipmentsChanged();
	}

	public RemoveAllFromShip(ship: Ship): void {
		ship.EquippedItems
			.filter(x => x.Equipped)
			.forEach(x => this.Equips.delete(x.Item.Id));

		this.EquipmentsChanged();
	}
	public RemoveFromRemodel(source: kcsapi_remodel_slot): void {
		if (source.api_use_slot_id != null) {
			source.api_use_slot_id.forEach(id => this.Equips.delete(id));

			this.EquipmentsChanged();
		}
	}

	private CreateItem(source: kcsapi_createitem): void {
		if (source.api_create_flag === 1 && source.api_slot_item !== null) {
			const item = new Equipment(source.api_slot_item);
			this.Equips.set(item.Id, item);
		}
		this.EquipmentsChanged();
	}
	private DestroyItem(data: kcsapi_req_destroyitem2) {
		try {
			if (typeof data.api_slotitem_ids === "number") {
				this.Equips.delete(data.api_slotitem_ids);
			} else {
				data.api_slotitem_ids.split(',')
					.map(x => parseInt(x))
					.forEach(x => this.Equips.delete(x));
			}

			this.EquipmentsChanged();
		}
		catch { }
	}
	private RemodelSlotItem(source: kcsapi_remodel_slot): void {
		if (source.api_after_slot == null) return;

		const equip = this.Equips.get(source.api_after_slot.api_id);
		if (!equip) return;

		equip.Remodel(source.api_after_slot.api_level, source.api_after_slot.api_slotitem_id);
	}

	private EquipmentsChanged(): void {
		this.RaisePropertyChanged(nameof(this.Equips));
		this.RaisePropertyChanged(nameof(this.EquipCount));
	}
}