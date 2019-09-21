import { kcsapi_deck } from "@KC/Raw/kcsapi_deck";

export default class Fleet {
	public readonly Id: number;

	public readonly Ships: number[] = [];

	constructor(data: kcsapi_deck) {
		this.Id = data.api_id;

		this.Update(data);
	}

	public Update(data: kcsapi_deck) {
		if (this.Id !== data.api_id)
			throw new Error(`[KanColle::Fleet] Fleet id mismatch! Original: ${this.Id}, Data: ${data.api_id}.`);

		this.Ships.splice(0, this.Ships.length);
		this.Ships.push(...data.api_ship);
		//
	}

	public ChangeShip(index: number, ship: number) {
		if (this.Ships.includes(ship)) {
			// Swap ships
			const oldIndex = this.Ships.findIndex((x) => x === ship);
			const oldShip = this.Ships[index];

			this.Ships[index] = ship;
			this.Ships[oldIndex] = oldShip;
		} else {
			// Set single ship
			this.Ships[index] = ship;
		}
	}
}
