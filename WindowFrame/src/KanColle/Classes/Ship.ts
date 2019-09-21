import { kcsapi_ship2 } from "@KC/Raw/kcsapi_ship";
import { ShipState } from "@KC/Enums/ShipEnums";

export default class Ship {
	public readonly Id: number;
	public State: ShipState;

	constructor(data: kcsapi_ship2) {
		this.Id = data.api_id;

		this.State = ShipState.None;
	}

	public Update(data: kcsapi_ship2) {
		if (this.Id !== data.api_id)
			throw new Error(`[KanColle::Ship] Ship id mismatch! Original: ${this.Id}, Data: ${data.api_id}.`);

		//
	}

	public Evacuate() {
		this.State |= ShipState.Evacuation;
	}

	public Tow() {
		this.State |= ShipState.Tow;
	}
}
