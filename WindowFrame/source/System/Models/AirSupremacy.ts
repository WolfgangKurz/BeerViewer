export class AirSupremacy {
	public readonly Minimum: number;
	public readonly Maximum: number;

	constructor(Minimum: number = 0, Maximum: number = 0) {
		this.Minimum = Minimum;
		this.Maximum = Maximum;
	}

	public static Sum(data: AirSupremacy[]) {
		return new AirSupremacy(
			data.reduce((a, c) => a + c.Minimum, 0),
			data.reduce((a, c) => a + c.Maximum, 0)
		);
	}

	public Display(Simple: boolean = true): string {
		if (Simple)
			return `${this.Minimum}~${this.Maximum}`;
		else
			return `${this.Minimum} ~ ${this.Maximum}`;
	}

	public toString(): string {
		return `{"Minimum": ${this.Minimum}, "Maximum": ${this.Maximum}}`;
	}
}