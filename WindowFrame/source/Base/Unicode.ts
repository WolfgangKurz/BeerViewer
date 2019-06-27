export default class Unicode {
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