export default class Storage {
	private static table: Map<string, any> = new Map<string, any>();

	public static set(name: string, value: any) {
		this.table.set(name, value);
	}
	public static get<T = any>(name: string): T {
		return this.table.get(name);
	}
	public static delete(name: string) {
		this.table.delete(name);
	}
	public static has(name: string): boolean {
		return this.table.has(name);
	}
}