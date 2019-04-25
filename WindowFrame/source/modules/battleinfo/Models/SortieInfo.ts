import { MapDifficulty, BattleRank } from "../Enums/Battle";
import { kcsapi_map_start } from "../Interfaces/kcsapi_map";
import { MapDifficultyText } from "../Helper";
import NodeEventInfo from "./NodeEventInfo";
import { BaseNodeData, PracticeNodeData, SortieNodeData } from "./NodeData";
import { Observable } from "System/Base/Observable";
import { EventKind, EventId } from "../Enums/Event";

export default class SortieInfo extends Observable {
	private _Difficulty: MapDifficulty = MapDifficulty.None;
	public get Difficulty(): MapDifficulty { return this._Difficulty }

	private _World: number = 0;
	public get World(): number { return this._World }

	private _Map: number = 0;
	public get Map(): number { return this._Map }

	private _Node: number = 0;
	public get Node(): number { return this._Node }

	private _NodeEvent: NodeEventInfo = new NodeEventInfo();
	public get NodeEvent(): NodeEventInfo { return this._NodeEvent }

	public get NodeHistory(): BaseNodeData[] { return this._NodeHistory }
	private _NodeHistory: BaseNodeData[] = [];

	public get CurrentNode(): BaseNodeData { return this._NodeHistory[this._NodeHistory.length - 1] }

	/** `{World}-{Map} {Difficulty}` */
	public get MapDisplay(): string {
		return `${this.World >= 30 ? "E" : this.World.toString()}-${this.Map} ${MapDifficultyText(this.Difficulty)}`
			.trim();
	}


	public Initialize(World: number, Map: number, Difficulty: MapDifficulty = MapDifficulty.None): void {
		this.$._World = World;
		this.$._Map = Map;
		this.$._Node = 0;
		this.$._NodeEvent = new NodeEventInfo();
		this.$._Difficulty = Difficulty;

		this.$._NodeHistory = [];
	}

	public Practice(): void {
		this.$._Node = 0;
		this.$._NodeEvent = new NodeEventInfo();

		this.$._NodeHistory = [];
		this._NodeHistory.push(new PracticeNodeData());
	}

	public Update(Node: number, EventId: EventId, EventKind: EventKind, source: kcsapi_map_start | null): void {
		this.$._Node = Node;
		this.$._NodeEvent = new NodeEventInfo(EventId, EventKind, source);

		this._NodeHistory.forEach(x => x.Outdate());
		this._NodeHistory.push(new SortieNodeData(this.World, this.Map, Node, this.NodeEvent));
	}

	public UpdateResult(RankResult: BattleRank, DropShipName: string): void {
		if (!("Update" in this.CurrentNode)) return; // Not SortieNodeData

		(<SortieNodeData>this.CurrentNode).Update(RankResult, DropShipName);
	}
}