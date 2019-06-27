export default class Unicode {
	public static Escape(text: string): string {
		let buffer = "";
		for (let i = 0; i < text.length; i++) {
			const c = text.charCodeAt(i);
			if (c > 0x7F)
				buffer += `\\u${c.toString(16).padStart(4, "0")}`;
			else
				buffer += text[i];
		}
		return buffer;
	}
}