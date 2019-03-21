import { Ship } from "System/Homeport/Ship";
import { Homeport } from "System/Homeport/Homeport";
import { Fleet } from "System/Homeport/Fleet";
import { EquipCategory } from "System/Enums/EquipEnums";
import { Equipment } from "System/Homeport/Equipment/Equipment";
import { ShipEquip } from "System/Homeport/Equipment/ShipEquip";
import { LoSCalcLogic } from "./LoSCalcLogic";
import Settings from "System/Settings";

export abstract class TypeCnLoS implements LoSCalcLogic {
	public abstract readonly Id: string;
	public abstract readonly Name: string;
	public abstract readonly Cn: number;
	public readonly HasCombinedSettings: boolean = true;

	public Calc(fleets: Fleet[]): number {
		if (!fleets || fleets.length === 0) return 0;

		const ships: Ship[] = this.GetTargetShips(fleets)
			.filter(x => (x.State & Ship.State.Evacuation) === 0)
			.filter(x => (x.State & Ship.State.Tow) === 0);

		if (ships.length === 0) return 0;

		const isCombined: boolean = fleets.length > 1
			&& <boolean>Settings.LoS.IsLoSIncludeFirstFleet.Value
			&& <boolean>Settings.LoS.IsLoSIncludeSecondFleet.Value;

		const itemScore: number = ships
			.reduce((a, c) => a.concat(c.EquippedItems), <ShipEquip[]>[])
			.map(x => x.Item)
			.reduce((a, c) => a + (c.Info.LoS + TypeCnLoS.GetLevelCoefficient(c)) * TypeCnLoS.GetTypeCoefficient(c.Info.Category), 0);

		const shipScore: number = ships
			.map(x => x.LoS - x.EquippedItems.reduce((a, c) => a + c.Item.Info.LoS, 0))
			.reduce((a, c) => a + Math.sqrt(c));

		const admiralScore: number = Math.ceil(Homeport.Instance.Admiral!.Level * 0.4);
		const vacancyScore: number = ((isCombined ? 12 : 6) - ships.length) * 2;

		return itemScore * this.Cn + shipScore - admiralScore + vacancyScore;
	}

	private GetTargetShips(fleets: Fleet[]): Ship[] {
		if (fleets.length == 1)
			return fleets[0].Ships;

		if (Settings.IsLoSIncludeFirstFleet && Settings.IsLoSIncludeSecondFleet)
			return fleets.reduce((a, c) => a.concat(c.Ships), <Ship[]>[]);

		if (Settings.IsLoSIncludeFirstFleet)
			return fleets[0].Ships;

		if (Settings.IsLoSIncludeSecondFleet)
			return fleets[1].Ships;

		return [];
	}
	private static GetLevelCoefficient(item: Equipment): number {
		switch (item.Info.Category) {
			case EquipCategory.SeaplaneReconnaissance:
				return Math.sqrt(item.Level) * 1.2;

			case EquipCategory.SmallRadar:
			case EquipCategory.LargeRadar:
			case EquipCategory.LargeRadar_II:
				return Math.sqrt(item.Level) * 1.25;

			default:
				return 0;
		}
	}
	private static GetTypeCoefficient(type: EquipCategory): number {
		switch (type) {
			case EquipCategory.CarrierBasedFighter:
			case EquipCategory.CarrierBasedDiveBomber:
			case EquipCategory.SmallRadar:
			case EquipCategory.LargeRadar:
			case EquipCategory.LargeRadar_II:
			case EquipCategory.AntiSubmarinePatrolAircraft:
			case EquipCategory.Searchlight:
			case EquipCategory.CommandFacility:
			case EquipCategory.AviationPersonnel:
			case EquipCategory.SurfaceShipPersonnel:
			case EquipCategory.LargeSonar:
			case EquipCategory.LargeFlyingBoat:
			case EquipCategory.LargeSearchlight:
			case EquipCategory.SeaplaneFighter:
			case EquipCategory.JetPoweredFighter: // Maybe
			case EquipCategory.JetPoweredFighterBomber:
				return 0.6;

			case EquipCategory.CarrierBasedTorpedoBomber:
			case EquipCategory.JetPoweredTorpedoBomber: // Maybe
				return 0.8;

			case EquipCategory.CarrierBasedReconnaissance:
			case EquipCategory.CarrierBasedReconnaissance_II:
				return 1.0;

			case EquipCategory.SeaplaneBomber:
				return 1.1;

			case EquipCategory.SeaplaneReconnaissance:
			case EquipCategory.JetPoweredReconnaissance: // Maybe
				return 1.2;

			default:
				return 0.0;
		}
	}
}
