import NodeEventInfo from "./NodeEventInfo";
import MapEdges from "../edges.json";
import { BattleRank } from "../Enums/Battle";

export class BaseNodeData {
	public get Name(): string { return "" }
	public get Detail(): string { return "" }
	public get FullName(): string { return `${this.Name} ${this.Detail}`.trim() }

	public get Description(): string[] { return [""] }

	public NodeTime: number = Date.now();

	private _Outdated: boolean = false;
	public get Outdated(): boolean { return this._Outdated }
	public Outdate(): void {
		this._Outdated = true;
	}
}
export class SortieNodeData extends BaseNodeData {
	//#region World Map Node
	private _World: number = 0;
	public get World(): number { return this._World }

	private _Map: number = 0;
	public get Map(): number { return this._Map }

	private _Node: number = 0;
	public get Node(): number { return this._Node }
	//#endregion

	private EventInfo: NodeEventInfo;
	private RankResult: BattleRank = BattleRank.None;
	private DropShipName: string = "";

	public get Name(): string { return this.GetNodeDisp(this.World, this.Map, this.Node) }
	public get Detail(): string { return this.EventInfo.toString() }

	public get Description(): string[] { return this.GetDescription() }

	constructor(World: number, Map: number, Node: number, EventInfo: NodeEventInfo) {
		super();

		this._World = World;
		this._Map = Map;
		this._Node = Node;
		this.EventInfo = EventInfo;
	}

	public Update(RankResult: BattleRank, DropShipName: string): void {
		if(this.RankResult !== BattleRank.None) throw "Updated twice";
		this.RankResult = RankResult;
		this.DropShipName = DropShipName;
	}

	private GetNodeDisp(world: number, map: number, node: number): string {
		let _map = `${world}-${map}`;
		if (_map in MapEdges) {
			const list = (<any>MapEdges)[_map];
			if (node in list)
				return `${list[node][1]}`;
		}
		return `${_map}-${node}`;
	}

	private GetDescription(): string[] {
		const ret = [`{0} {1}\nResult: {2}\nDrop: {3}`, this.Name, this.Detail];

		if (this.RankResult) {
			ret.push(this.RankResult.toString());
			ret.push(this.DropShipName || "None");
		}
		return ret;
	}
}
export class PracticeNodeData extends BaseNodeData
{
	public get Name(): string { return "" }
	public get Detail(): string { return "PVP" }
}