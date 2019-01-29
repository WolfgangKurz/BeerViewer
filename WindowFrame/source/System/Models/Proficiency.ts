export enum AirSupremacyOption {
	Average,
	MinimumValue,
	MaximumValue
}

export class Proficiency {
	private readonly internalMinValue: number;
	private readonly internalMaxValue: number;
	public readonly FighterBonus: number;
	public readonly SeaplaneBomberBonus: number;

	constructor(internalMin: number, internalMax: number, fighterBonus: number, seaplaneBomberBonus: number) {
		this.internalMinValue = internalMin;
		this.internalMaxValue = internalMax;
		this.FighterBonus = fighterBonus;
		this.SeaplaneBomberBonus = seaplaneBomberBonus;
	}

	public GetInternalValue(options: AirSupremacyOption): number {
		if (options & AirSupremacyOption.MinimumValue) return this.internalMinValue;
		if (options & AirSupremacyOption.MaximumValue) return this.internalMaxValue;
		return (this.internalMaxValue + this.internalMinValue) / 2;
	}
}