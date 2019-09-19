import fs from "fs";
import glob from "glob";
import path from "path";

/**
 * i18n manager class
 */
// tslint:disable-next-line:class-name
export default class i18n {
	/**
	 * Get all localization text list of given locale name.
	 * @param name Locale name
	 */
	public static get(name: string): string | null {
		const list = glob.sync(path.join(__static, "i18n", `${name}?(_)**.txt`))
			.filter((x) => fs.existsSync(x));

		if (list.length === 0) return null;

		return list.map((x) => fs.readFileSync(x, { encoding: "utf8" }))
			.join("\n");
	}
}
