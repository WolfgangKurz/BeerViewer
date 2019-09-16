/**
 * Unicode string process class
 */
export default class Unicode {
	/**
	 * Escape string to `\\` escaped string
	 * @param text String to escape as unicode string
	 * @param forRegex Escapes for Regular Expression? default is `false`
	 * @returns Unicode escaped string
	 */
	public static Escape(text: string, forRegex: boolean = false): string {
		let buffer = "";
		for (let i = 0; i < text.length; i++) {
			const c = text.charCodeAt(i);
			if (c > 0x7F)
				buffer += `\\u${c.toString(16).padStart(4, "0")}`;
			else
				buffer += text[i];
		}

		if (forRegex)
			return buffer.replace(/\\/g, "\\\\");
		else
			return buffer;
	}
}
