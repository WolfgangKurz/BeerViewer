import { EventId, EventKind } from "../Enums/Event";
import { kcsapi_map_start } from "../Interfaces/kcsapi_map";
import NodeItemInfo from "./NodeItemInfo";

export default class NodeEventInfo {
	private source: kcsapi_map_start | null = null;

	private _Id: EventId;
	public get Id(): EventId { return this._Id }

	private _Kind: EventKind;
	public get Kind(): EventKind { return this._Kind }

	constructor(Id: EventId = EventId.None, Kind: EventKind = EventKind.None, source: kcsapi_map_start | null = null) {
		this._Id = Id;
		this._Kind = Kind;
		this.source = source;
	}

	public toString(): string {
		if (!this.source) return "???";

		const api_itemget = this.source.api_itemget;
		const api_happening = this.source.api_happening;
		const api_itemget_eo = this.source.api_itemget_eo_comment;

		switch (this.Id) {
			case EventId.Obtain:
				if (api_itemget === undefined || !Array.isArray(api_itemget))
					return "Resource";

				return api_itemget.map(x => {
					const ResourceName = NodeItemInfo.Exists(x.api_id - 1)
						? NodeItemInfo.Get(x.api_id - 1)
						: (x.api_name && x.api_name.length > 0 ? x.api_name : "???");

					return x.api_getcount > 1
						? `${ResourceName} +${x.api_getcount}`
						: ResourceName;
				}).join(" ");

			case EventId.Loss:
				if (api_happening === null || api_happening.api_count === 0)
					return "Lost";

				const ResourceName = NodeItemInfo.Exists(api_happening.api_mst_id - 1)
					? NodeItemInfo.Get(api_happening.api_mst_id - 1)
					: "???";

				return api_happening.api_count > 1
					? `${ResourceName} -${api_happening.api_count}`
					: ResourceName;

			case EventId.NormalBattle:
				switch (this.Kind) {
					case EventKind.Battle:
						return "Battle";

					case EventKind.NightBattle:
						return "Night";

					case EventKind.NightToDayBattle:
						return "Night2Day";

					case EventKind.AirBattle:
						return "AirBattle";

					case EventKind.EnemyCombinedBattle:
						return "Combined";

					case EventKind.LongDistanceAirBattle:
						return "Outrange";

					case EventKind.EnemyCombinedNightToDayBattle:
						return "Combined,Night2Day";

					default:
						return "Battle(Unknown)";
				}

			case EventId.BossBattle:
				switch (this.Kind) {
					case EventKind.NightBattle:
						return "Boss(Night)";

					case EventKind.NightToDayBattle:
						return "Boss(Night2Day)";

					case EventKind.EnemyCombinedBattle:
						return "Boss(Combined)";

					case EventKind.EnemyCombinedNightToDayBattle:
						return "Boss(Combined,Night2Day)";

					default:
						return "Boss";
				}

			case EventId.NoEvent:
				if (this.Kind == EventKind.Selectable)
					return "Selection";
				return "No Battle";

			case EventId.TP:
				return "Transport";

			case EventId.AirEvent:
				switch (this.Kind) {
					case EventKind.AirSearch:
						{
							if (api_itemget === undefined || Array.isArray(api_itemget)) return "Scout failed";

							const ResourceName = NodeItemInfo.Exists(api_itemget.api_id - 1)
								? NodeItemInfo.Get(api_itemget.api_id - 1)
								: "???";

							return api_itemget.api_getcount > 1
								? `${ResourceName} +${api_itemget.api_getcount}`
								: ResourceName;
						}

					case EventKind.AirBattle:
					default:
						return "AirBattle";
				}

			case EventId.Escort: // 1-6
				{
					const x = api_itemget_eo;
					if (!x) return "Resource";

					const ResourceName = NodeItemInfo.Exists(x.api_id - 1)
						? NodeItemInfo.Get(x.api_id - 1)
						: (x.api_name && x.api_name.length > 0 ? x.api_name : "???");

					return x.api_getcount > 1
						? `${ResourceName} +${x.api_getcount}`
						: ResourceName;
				}

			case EventId.LDAirBattle:
				return "Outrange";

			default:
				return `??? (${this.Id}.${this.Kind})`;
		}
	}
}