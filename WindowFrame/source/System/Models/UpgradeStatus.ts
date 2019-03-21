import { GuageValue } from "./GuageValue";
import { MasterMinMax } from "System/Master/MasterMinMax";

export class UpgradeStatus extends GuageValue {
	constructor();
	constructor(Min: number, Max: number, Upgraded: number);
	constructor(MinMax: MasterMinMax, Upgraded: number);

	constructor(Min_or_MinMax: number | MasterMinMax = 0, Max_or_Upgraded: number = 0, Upgraded: number | undefined = 0) {
		if (typeof Min_or_MinMax === "number")
			super(Min_or_MinMax, Max_or_Upgraded, Upgraded || 0);
		else
			super(Min_or_MinMax.Min + Max_or_Upgraded, Min_or_MinMax.Max, Min_or_MinMax.Min);
	}
}