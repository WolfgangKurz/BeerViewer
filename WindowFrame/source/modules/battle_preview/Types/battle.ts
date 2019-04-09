import { MastersShipData } from "../ShipData";
import { Master } from "System/Master/Master";
import ShipEquipData from "../ShipEquipData";
import { ShipInfo } from "System/Master/Wrappers/ShipInfo";
import { EquipInfo } from "System/Master/Wrappers/EquipInfo";

export default class battle {
	public static ToMastersShipData(data: battle_base): MastersShipData[] {
		const ships = Master.Instance.Ships;
		const equips = Master.Instance.Equips;
		if (!ships || !equips) return [];

		return data.api_ship_ke
		.filter(x => x != -1 && ships.has(x))
			.map((x, i) => {
				const ship = <ShipInfo>ships.get(x);

				const output = new MastersShipData(ship);
				output.Update({
					Level: data.api_ship_lv[i],
					FirePower: data.api_eParam[i][0],
					Torpedo: data.api_eParam[i][1],
					AA: data.api_eParam[i][2],
					Armor: data.api_eParam[i][3],
					Equips: data.api_eSlot[i]
						.filter(s => 0 < s).map(s => equips.get(s))
						.filter(s => s).map(s => new ShipEquipData(<EquipInfo>s))
				});
				return output;
			})
			.filter(x => x !== null);
	}
}

export interface battle_base {
	api_ship_ke: number[];
	api_ship_lv: number[];
	api_f_nowhps: number[];
	api_f_maxhps: number[];
	api_e_nowhps: number[];
	api_e_maxhps: number[];
	api_eSlot: number[][];
	api_eKyouka: number[][];
	api_fParam: number[][];
	api_eParam: number[][];
}