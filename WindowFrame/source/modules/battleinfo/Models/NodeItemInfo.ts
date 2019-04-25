export default class NodeItemInfo {
	private static NodeItemList: string[] = [
		"燃料",
		"弾薬",
		"鋼材",
		"ボーキサイト",
		"高速建造材",
		"高速修復材",
		"開発資材",
		"改修資材",
		"",
		"家具箱(小)",
		"家具箱(中)",
		"家具箱(大)"
	];

	public static Exists(id: number) {
		if (id < 0 || id >= this.NodeItemList.length || this.NodeItemList[id].length === 0) return false;
		return true;
	}

	public static Get(id: number): string {
		if (id < 0 || id >= this.NodeItemList.length || this.NodeItemList[id].length === 0) return "???";
		return this.NodeItemList[id];
	}
}