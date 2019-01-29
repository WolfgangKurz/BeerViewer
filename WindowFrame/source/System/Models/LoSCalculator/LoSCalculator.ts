import { SimpleSumLoS } from "./SimpleSumLoS";
import { Old2_5LoS } from "./Old2_5LoS";
import { Fall2_5LoS } from "./Fall2_5LoS";
import { Cn1LoS, Cn3LoS, Cn4LoS } from "./CnLoS";
import { ILoSCalculator, LoSCalcLogic } from "./LoSCalcLogic";

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