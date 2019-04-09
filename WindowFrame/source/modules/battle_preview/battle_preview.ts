import Vue from "vue";
import { mapState } from "vuex";
import { IModule, GetModuleTemplate } from "System/Module";
import { SubscribeKcsapi, KcsApiCallback } from "System/Base/KcsApi";
import { HTTPRequest } from "System/Exports/API";
import { Homeport } from "System/Homeport/Homeport";
import { MembersShipData } from "./ShipData";
import { EventKind, EventId, BattleEngage, Formation, FleetType } from "./Types/Enums";
import FleetData from "./FleetData";
import { kcsapi_map_next, kcsapi_map_start } from "./Types/kcsapi_map";
import battle, { battle_base } from "./Types/battle";

import MapEdges from "./edges.json";

const Endpoint = {
	get_member: {
		/** LBAS, Current information and state of inserted aircrafts */
		base_air_corps: "/api_get_member/base_air_corps",
	},
	req_air_corps: {
		/** LBAS, Insert aircraft to slot */
		set_plane: "/api_req_air_corps/set_plane",
		/** LBAS, Change name of squad */
		change_name: "/api_req_air_corps/change_name",
		/** LBAS, Change action of squad */
		set_action: "/api_req_air_corps/set_action",
		/** LBAS, Supply aircrafts */
		supply: "/api_req_air_corps/supply",
		/** LBAS, Purchase squad */
		expand_base: "api_req_air_corps/expand_base"
	},
	api_get_member: {
		mapinfo: "/api_get_member/mapinfo",
		practice: "/api_get_member/practice",
		mission: "/api_get_member/mission"
	},
	api_req_map: {
		/** Map start */
		start: "/api_req_map/start",
		/** Map next */
		next: "/api_req_map/next",
		/** Event map difficulty selection */
		select_eventmap_rank: "/api_req_map/select_eventmap_rank"
	},
	api_req_sortie: {
		/** Day battle */
		battle: "/api_req_sortie/battle",
		/** Night to Day battle */
		night_to_day: "/api_req_sortie/night_to_day",
		/** Air battle */
		airbattle: "/api_req_sortie/airbattle",
		/** Air-raid */
		ld_airbattle: "/api_req_sortie/ld_airbattle",
		/** Radar Fire */
		ld_shooting: "/api_req_sortie/ld_shooting",

		/** Single fleet evation */
		goback_port: "/api_req_sortie/goback_port",

		/** Battle result */
		battleresult: "/api_req_sortie/battleresult"
	},
	api_req_battle_midnight: {
		/** Night battle, from Day battle */
		battle: "/api_req_battle_midnight/battle",
		/** Night only battle */
		sp_midnight: "/api_req_battle_midnight/sp_midnight"
	},
	api_req_combined_battle: {
		/** Carrier Task Force */
		battle: "/api_req_combined_battle/battle",
		/** Surface Task Force */
		battle_water: "/api_req_combined_battle/battle_water",

		/** Single fleet vs Combined */
		ec_battle: "/api_req_combined_battle/ec_battle",
		/** Carrier Task Force vs Combined */
		each_battle: "/api_req_combined_battle/each_battle",
		/** Surface Task Force vs Combined */
		each_battle_water: "/api_req_combined_battle/each_battle_water",

		/** Combined fleet Out-range */
		airbattle: "/api_req_combined_battle/airbattle",
		/** Combined fleet Air-raid */
		ld_airbattle: "/api_req_combined_battle/ld_airbattle",
		/** Combined fleet Radar Fire? */
		ld_shooting: "/api_req_combined_battle/ld_shooting",

		/** Combined fleet Night to Day battle */
		ec_night_to_day: "/api_req_combined_battle/ec_night_to_day",
		/** Combined fleet Night battle, from Day battle */
		midnight_battle: "/api_req_combined_battle/midnight_battle",
		/** Combined fleet Night only battle */
		sp_midnight: "/api_req_combined_battle/sp_midnight",

		/** Single fleet vs Combined night battle */
		ec_midnight_battle: "/api_req_combined_battle/ec_midnight_battle",
		/** Combined fleet Battle result */
		battleresult: "/api_req_combined_battle/battleresult",

		/** Combined fleet evation */
		goback_port: "/api_req_combined_battle/goback_port"
	},
	api_req_member: {
		/** Practice, Enemy's information */
		get_practice_enemyinfo: "/api_req_member/get_practice_enemyinfo"
	},
	api_req_practice: {
		/** Practice, Day battle */
		battle: "/api_req_practice/battle",
		/** Practice, Night battle, from Night battle */
		midnight_battle: "/api_req_practice/midnight_battle",
		/** Practice, Battle result */
		battle_result: "/api_req_practice/battle_result"
	}
};

// Deep Freeze
((x: any) => {
	const f = (y: any) => {
		Object.freeze(y);
		Object.getOwnPropertyNames(y).forEach(function (prop) {
			if (y.hasOwnProperty(prop)
				&& y[prop] !== null
				&& (typeof y[prop] === "object" || typeof y[prop] === "function")
				&& !Object.isFrozen(y[prop])) {
				f(y[prop]);
			}
		});
	};
	f(x);
})(Endpoint);

class BattlePreview implements IModule {
	private Data = {
		IsBoss: false,
		IsEnd: false,

		Map: {
			MapArea: 0,
			MapNo: 0,
			Node: 0,

			NodeDisp: ""
		},
		Event: {
			Kind: EventKind.None,
			Id: EventId.None
		},
		Battle: {
			Engage: BattleEngage.None
		},

		Fleets: {
			AliasFirst: <FleetData | null>null,
			AliasSecond: <FleetData | null>null,
			EnemyFirst: <FleetData | null>null,
			EnemySecond: <FleetData | null>null,
			CurrentDeckId: 0
		}
	};

	constructor() {
		Vue.component("battle-preview-component", {
			data: () => this.Data,
			template: GetModuleTemplate(),
			computed: mapState({
				i18n: "i18n"
			}),
			methods: {
				format(format: string, args: string[]) {
					if (args === undefined) return format;
					for (let i = 0; i < args.length; i++) {
						const arg = args[i];
						const reg = new RegExp("\\{" + i + "\\}", "g");
						format = format.replace(reg, arg);
					}
					return format;
				}
			}
		});
	}

	init(): void {
		(() => {
			let proc: KcsApiCallback<kcsapi_map_next, HTTPRequest> = (data => {
				this.ClearEvationList();

				this.Data.Map.MapArea = data.api_maparea_id;
				this.Data.Map.MapNo = data.api_mapinfo_no;
				this.Data.Map.Node = data.api_no;
				this.Data.Map.NodeDisp = this.GetNodeDisp(this.Data.Map.MapArea, this.Data.Map.MapNo, this.Data.Map.Node);

				this.Data.Event.Kind = data.api_event_kind;
				this.Data.Event.Id = data.api_event_id;

				this.Data.IsBoss = this.Data.Event.Id === EventId.BossBattle;
				this.Data.IsEnd = data.api_next === 0;
			});

			SubscribeKcsapi<kcsapi_map_start>(Endpoint.api_req_map.start, proc);
			SubscribeKcsapi(Endpoint.api_req_map.next, proc);
		})();

		window.API.Log("Battle Preview module has been loaded");

		window.modules.areas.register("sub", "battle-preview", "Battle Preview", "game", "battle-preview-component");
	}
	private UpdateFleets(api_deck_id: number, data: battle_base, api_formation?: [Formation, Formation, BattleEngage]): void {
		// Update alias with deck
		this.UpdateFriendFleets(api_deck_id);

		this.Data.Fleets.EnemyFirst = new FleetData(
			battle.ToMastersShipData(data),
			this.Data.Fleets.EnemyFirst ? this.Data.Fleets.EnemyFirst.Formation : Formation.None,
			this.Data.Fleets.EnemyFirst ? this.Data.Fleets.EnemyFirst.Name : "",
			FleetType.EnemyFirst
		);

		// 제 2함대는 없음
		this.Data.Fleets.EnemySecond = new FleetData([],Formation.None,"",FleetType.EnemySecond);

		// 진형과 전투형태 존재하면
		if (api_formation != null) {
			this.Data.Battle.Engage = api_formation[2];

			if (this.Data.Fleets.AliasFirst !== null)
				this.Data.Fleets.AliasFirst.UpdateFormation(api_formation[0]);

			if (this.Data.Fleets.EnemyFirst !== null)
				this.Data.Fleets.EnemyFirst.UpdateFormation(api_formation[1]);
		}

		// 출격중인 함대 번호 갱신
		this.Data.Fleets.CurrentDeckId = api_deck_id;
	}
	private UpdateFriendFleets(deckID: number): void {
		const fleets = Homeport.Instance.Fleets;
		const combined = Homeport.Instance.FleetCombined;

		this.Data.Fleets.AliasFirst = new FleetData(
			fleets.get(deckID)!.Ships.map(s => new MembersShipData(s)),
			this.Data.Fleets.AliasFirst
				? this.Data.Fleets.AliasFirst.Formation
				: Formation.None,
			fleets.get(deckID)!.Name,
			FleetType.AliasFirst
		);
		this.Data.Fleets.AliasSecond = new FleetData(
			combined && deckID == 1
				? fleets.get(2)!.Ships.map(s => new MembersShipData(s))
				: [],
				this.Data.Fleets.AliasSecond
					? this.Data.Fleets.AliasSecond.Formation
					: Formation.None,
			fleets.get(2)!.Name,
			FleetType.AliasSecond
		);
	}
	private ClearEvationList() {

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
}
export default BattlePreview;