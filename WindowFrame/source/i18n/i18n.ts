import fs from "fs";
import glob from "glob";
import path from "path";

export default class i18n {
	public static get(name: string): string | null {
		const list = glob.sync(path.join(__dirname, `${name}?(_)**.txt`))
			.filter(x => fs.existsSync(x));

		if (list.length === 0) return null;

		return list.map(x => fs.readFileSync(x, { encoding: "utf8" }))
			.join("\n");
	}
}