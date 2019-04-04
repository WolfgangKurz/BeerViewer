import { Homeport } from "System/Homeport/Homeport";
import { Ship } from "System/Homeport/Ship";
import { Equipment } from "System/Homeport/Equipment/Equipment";

export default class LogHelper {
	private static i18n(name: string): string {
		return window.i18n[name] || name;
	}

	/** Returns material list that different element only */
	public static MaterialDiff(api_list: number[]): string;
	public static MaterialDiff(api_before: number[], api_after: number[]): string;

	public static MaterialDiff(api_before: number[], api_after?: number[]): string {
		const names: string[] = ["Fuel", "Ammo", "Steel", "Bauxite"].map(x => this.i18n(x));
		const output: string[] = [];
		if (api_after === undefined) {
			for (let i = 0; i < api_before.length; i++) {
				if (api_before[i] !== 0) {
					const diff = api_before[i];
					output.push(`${names[i]} ${diff > 0 ? '+' : ''}${diff}`);
				}
			}
		} else {
			const length = Math.min(api_before.length, api_after.length, 4);
			for (let i = 0; i < length; i++) {
				if (api_before[i] !== api_after[i]) {
					const diff = api_after[i] - api_before[i];
					output.push(`${names[i]} ${diff > 0 ? '+' : ''}${diff}`);
				}
			}
		}
		return output
			.map(x => `\`${x}\``)
			.join(", ");
	}

	/** Returns ship name from id comma separated list */
	public static GetShipNamesFromList(api_ship_id_list: string): string {
		return api_ship_id_list.split(",")
			.filter(x => Homeport.Instance.Ships.has(parseInt(x)))
			.map(x => <Ship>Homeport.Instance.Ships.get(parseInt(x)))
			.map(x => `\`${window.i18n[x.Info.Name] || x.Info.Name}\``)
			.join(", ");
	}

	/** Returns equipment name from id comma separated list */
	public static GetEquipNamesFromList(api_equip_id_list: string): string {
		return api_equip_id_list.split(",")
			.filter(x => Homeport.Instance.Equipments.Equips.has(parseInt(x)))
			.map(x => <Equipment>Homeport.Instance.Equipments.Equips.get(parseInt(x)))
			.map(x => `\`${window.i18n[x.Info.Name] || x.Info.Name}\``)
			.join(", ");
	}
}