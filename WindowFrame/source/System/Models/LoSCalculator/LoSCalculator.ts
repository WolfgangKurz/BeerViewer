import { Fleet } from "../../Homeport/Fleet";
import { SimpleSumLoS } from "./SimpleSumLoS";
import { Old2_5LoS } from "./Old2_5LoS";
import { Fall2_5LoS } from "./Fall2_5LoS";
import { Cn1LoS, Cn3LoS, Cn4LoS } from "./CnLoS";

export interface ILoSCalculator {
    readonly Id: string;
    readonly Name: string;

    readonly HasCombinedSettings: boolean;

    Calc(fleets: Fleet[]): number;
}

export class LoSCalculator {
    public static Instance: LoSCalculator = new LoSCalculator();

    private readonly logics: Map<string, ILoSCalculator> = new Map<string, ILoSCalculator>();

    public get Logics(): ILoSCalculator[] {
        const output: ILoSCalculator[] = [];

        this.logics.forEach(x => output.push(x));
        return output;
    }

    public Get(key: string): ILoSCalculator | null {
        return this.logics.get(key) || null;
    }

    constructor() {
        this.AddLogic(new SimpleSumLoS());
        this.AddLogic(new Old2_5LoS());
        this.AddLogic(new Fall2_5LoS());
        this.AddLogic(new Cn1LoS());
        this.AddLogic(new Cn3LoS());
        this.AddLogic(new Cn4LoS());
    }

    public AddLogic(logic: LoSCalcLogic): boolean {
        if (this.logics.has(logic.Id)) return false; // Already registered Id
        this.logics.set(logic.Id, logic);
        return true;
    }
}
export abstract class LoSCalcLogic implements ILoSCalculator {
    public readonly abstract Id: string;
    public readonly abstract Name: string;
    public readonly abstract HasCombinedSettings: boolean;
    public abstract Calc(fleets: Fleet[]): number;
}