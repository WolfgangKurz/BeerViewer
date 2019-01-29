import { Fleet } from "System/Homeport/Fleet";
import { Ship } from "System/Homeport/Ship";
import { EquipCategory } from "System/Enums/EquipEnums";
import { EquipInfo } from "System/Master/Wrappers/EquipInfo";
import { Homeport } from "System/Homeport/Homeport";
import { ShipEquip } from "System/Homeport/Equipment/ShipEquip";
import { LoSCalcLogic } from "./LoSCalcLogic";

export class Fall2_5LoS extends LoSCalcLogic {
	public readonly Id: string = "Fall2_5";
	public readonly Name: string = "Fall 2-5";
	public readonly HasCombinedSettings: boolean = false;

	public Calc(fleets: Fleet[]): number {
		if (!fleets || fleets.length === 0) return 0;

		const ships = fleets.reduce((a, c) => a.concat(c.Ships), <Ship[]>[]);
		const _this = this;

		const itemScore = (function (ships: Ship[]) {
			const infos = ships
				.reduce((a, c) => a.concat(c.EquippedItems), <ShipEquip[]>[])
				.map(x => x.Item.Info);

			const map = new Map<EquipCategory, number[]>();
			infos.forEach((x: EquipInfo) => {
				const key = x.Category;
				const item = x.LoS;
				const collection = map.get(key);
				if (!collection) {
					map.set(key, [item]);
				} else {
					collection.push(item);
				}
			});

			let output: number = 0;
			map.forEach((v, k) => output += _this.GetScore(k, v.reduce((a, c) => a + c, 0)));
			return output;
		})(ships);

		const shipScore = ships
			.map(x => x.LoS - x.EquippedItems.reduce((a, c) => a + c.Item.Info.LoS, 0))
			.map(x => Math.sqrt(x))
			.reduce((a, c) => a + c, 0) * 1.69;

		const level: number = (((Homeport.Instance.Admiral!.Level + 4) / 5) * 5);
		const admiralScore: number = level * -0.61;

		return itemScore + shipScore + admiralScore;
	}

	private GetScore(type: EquipCategory, score: number): number {
		switch (type) {
			case EquipCategory.CarrierBasedDiveBomber:
				return score * 1.04;
			case EquipCategory.CarrierBasedTorpedoBomber:
				return score * 1.37;
			case EquipCategory.CarrierBasedRecon:
			case EquipCategory.CarrierBasedRecon_II:
				return score * 1.66;

			case EquipCategory.SeaplaneRecon:
				return score * 2.00;
			case EquipCategory.SeaplaneBomber:
				return score * 1.78;

			case EquipCategory.SmallRadar:
				return score * 1.00;
			case EquipCategory.LargeRadar:
			case EquipCategory.LargeRadar_II:
				return score * 0.99;

			case EquipCategory.Searchlight:
				return score * 0.91;
		}
		return 0.0;
	}
}