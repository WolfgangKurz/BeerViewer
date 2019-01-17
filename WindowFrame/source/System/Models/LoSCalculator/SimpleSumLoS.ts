import { LoSCalcLogic } from "./LoSCalculator";
import { Fleet } from "../../Homeport/Fleet";
import { Ship } from "../../Homeport/Ship";

/** Sum all LoS value simply */
export class SimpleSumLoS extends LoSCalcLogic {
    public readonly Id: string = "SimpleSum";
    public readonly Name: string = "Simple Sum";
    public readonly HasCombinedSettings: boolean = false;

    public Calc(fleets: Fleet[]): number {
        if (fleets && fleets.length > 0)
            return fleets.reduce((a, c) => a.concat(c.Ships), <Ship[]>[])
                .reduce((a, c) => a + c.LoS, 0);
        return 0;
    }
}