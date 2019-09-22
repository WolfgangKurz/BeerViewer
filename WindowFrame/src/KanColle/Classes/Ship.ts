import { ShipState } from "@KC/Enums/ShipEnums";

import { kcsapi_ship2 } from "@KC/Raw/kcsapi_ship";
import { Master } from "@KC/Store/KanColleStore";

export default class Ship {
	public Id!: number;
	public State!: ShipState;
	public Name!: string;

	public Update(data: kcsapi_ship2): this {
		if (this.Id === undefined)
			this.Id = data.api_id; // Not set yet
		else if (this.Id !== data.api_id)
			throw new Error(`[KanColle::Ship] Ship id mismatch! Original: ${this.Id}, Data: ${data.api_id}.`);

		this.State = ShipState.None;
		this.Name = Master().Ships[data.api_ship_id].api_name;

		return this;
	}

	public Evacuate() {
		this.State |= ShipState.Evacuation;
	}

	public Tow() {
		this.State |= ShipState.Tow;
	}
}
