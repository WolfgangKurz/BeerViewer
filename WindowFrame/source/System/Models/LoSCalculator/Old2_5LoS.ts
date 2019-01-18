import { Fleet } from "../../Homeport/Fleet";
import { Ship } from "../../Homeport/Ship";
import { EquipDictCategory } from "../../Enums/EquipEnums";
import { LoSCalcLogic } from "./LoSCalcLogic";

export class Old2_5LoS extends LoSCalcLogic {
    public readonly Id: string = "Old2_5";
    public readonly Name: string = "Old 2-5";
    public readonly HasCombinedSettings: boolean = false;

    public Calc(fleets: Fleet[]): number {
        if (!fleets || fleets.length === 0) return 0;

        const ships = fleets.reduce((a, c) => a.concat(c.Ships), <Ship[]>[]);

        const recon = ships
            .reduce((a, c) =>
                a.concat(
                    c.EquippedItems.filter(x => x.Item.Info.Type === EquipDictCategory.Recon)
                        .filter(x => x.CurrentAircraft > 0)
                        .map(x => x.Item.Info.LoS)
                ),
                <number[]>[]
            )
            .reduce((a, c) => a + c, 0);

        const radar = ships
            .reduce((a, c) =>
                a.concat(
                    c.EquippedItems.filter(x => x.Item.Info.Type === EquipDictCategory.Radar)
                        .map(x => x.Item.Info.LoS)
                ),
                <number[]>[]
            )
            .reduce((a, c) => a + c, 0);

        return (recon * 2) + radar + Math.sqrt(ships.reduce((a, c) => a + c.LoS, 0) - recon - radar);
    }
}