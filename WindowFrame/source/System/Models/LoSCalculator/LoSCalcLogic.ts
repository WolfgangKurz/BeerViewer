import { Fleet } from "System/Homeport/Fleet";

export interface ILoSCalculator {
	readonly Id: string;
	readonly Name: string;

	readonly HasCombinedSettings: boolean;

	Calc(fleets: Fleet[]): number;
}

export abstract class LoSCalcLogic implements ILoSCalculator {
	public readonly abstract Id: string;
	public readonly abstract Name: string;
	public readonly abstract HasCombinedSettings: boolean;
	public abstract Calc(fleets: Fleet[]): number;
}