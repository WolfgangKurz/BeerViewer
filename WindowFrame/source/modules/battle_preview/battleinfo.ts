import Vue from "vue";
import { mapState } from "vuex";
import { IModule, GetModuleTemplate } from "System/Module";
import { SubscribeKcsapi } from "System/Base/KcsApi";
import { Homeport } from "System/Homeport/Homeport";
import { BattleEngage, Formation, FleetType, MapDifficulty, AirSupremacy, BattleRank } from "./Enums/Battle";
import FleetData from "./Models/FleetData";
import { kcsapi_map_next, kcsapi_map_start, kcsapi_req_map_start, kcsapi_req_map_next } from "./Interfaces/kcsapi_map";
import battle, { battle_base } from "./Interfaces/battle";
import Calculator from "./Calculator";
import SortieInfo from "./Models/SortieInfo";
import AirBattleResult from "./Models/AirBattleResult";
import BattleFlags from "./BattleFlags";
import { MembersShipData } from "./Models/ShipData";
import { EventKind, EventId } from "./Enums/Event";

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

class BattleInfo implements IModule {
	private Data = {
		Map: {
			MapArea: 0,
			MapNo: 0,
			Node: 0,
			NodeDisp: "",

			Extended: false
		},
		Event: {
			Kind: EventKind.None,
			Id: EventId.None
		},
		Battle: {
			Engage: BattleEngage.None,
			AirSupremacy: AirSupremacy.None,
			AirBattleResults: <AirBattleResult[]>[],

			Rank: BattleRank.None,
			AirRank: BattleRank.None
		},

		Fleets: {
			Alias: {
				First: <FleetData | null>null,
				Second: <FleetData | null>null
			},
			Enemy: {
				First: <FleetData | null>null,
				Second: <FleetData | null>null
			},
			CurrentDeckId: 0
		},

		UpdatedTime: 0
	};

	private SortieInfo: SortieInfo = new SortieInfo();
	private Calculator: Calculator = new Calculator();
	private BattleFlags: BattleFlags = new BattleFlags();

	private EventMapDifficulty: Map<number, MapDifficulty> = new Map<number, MapDifficulty>();

	constructor() {
		Vue.component("battleinfo-component", {
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
			let proc = (data: kcsapi_map_start | kcsapi_map_next, request: kcsapi_req_map_start | kcsapi_req_map_next, next: boolean = false) => {
				if (!next) this.Data.Fleets.CurrentDeckId = request.api_deck_id;
				if (this.Data.Fleets.CurrentDeckId < 1) return;

				this.UpdateFriendFleets(this.Data.Fleets.CurrentDeckId);

				if (!next) {
					this.SortieInfo.Initialize(
						data.api_maparea_id,
						data.api_mapinfo_no,
						data.api_eventmap
							? this.EventMapDifficulty.get(data.api_mapinfo_no)
							: MapDifficulty.None
					);
				}

				this.ClearBattleInfo();

				// Map extended with visit node
				if (next && (<kcsapi_map_next>data).api_m1 !== undefined)
					this.Data.Map.Extended = (<kcsapi_map_next>data).api_m1 === 1;

				// Map extended after LBAS raid
				if (data.api_destruction_battle) {
					if (data.api_destruction_battle.api_m1 !== undefined)
						this.Data.Map.Extended = data.api_destruction_battle.api_m1 === 1;
				}

				this.SortieInfo.Update(data.api_no, data.api_event_id, data.api_event_kind, data);
			};

			SubscribeKcsapi<kcsapi_map_start, kcsapi_req_map_start>(Endpoint.api_req_map.start, (x, y) => proc(x, y, false));
			SubscribeKcsapi<kcsapi_map_next, kcsapi_req_map_next>(Endpoint.api_req_map.next, (x, y) => proc(x, y, true));
		})();

		window.API.Log("BattleInfo module has been loaded");

		window.modules.areas.register("sub", "battleinfo", "BattleInfo", "game", "battleinfo-component");
	}

	private UpdateFleets(api_deck_id: number, data: battle_base, api_formation?: [Formation, Formation, BattleEngage]): void {
		// Update alias with deck
		this.UpdateFriendFleets(api_deck_id);

		this.Data.Fleets.Enemy.First = new FleetData(
			battle.ToMastersShipData(data),
			this.Data.Fleets.Enemy.First ? this.Data.Fleets.Enemy.First.Formation : Formation.None,
			this.Data.Fleets.Enemy.First ? this.Data.Fleets.Enemy.First.Name : "",
			FleetType.EnemyFirst
		);

		// No second fleet
		this.Data.Fleets.Enemy.Second = new FleetData([], Formation.None, "", FleetType.EnemySecond);

		// Formation available
		if (api_formation != null) {
			this.Data.Battle.Engage = api_formation[2];

			if (this.Data.Fleets.Alias.First !== null)
				this.Data.Fleets.Alias.First.UpdateFormation(api_formation[0]);

			if (this.Data.Fleets.Enemy.First !== null)
				this.Data.Fleets.Enemy.First.UpdateFormation(api_formation[1]);
		}

		// Update fleet id that in sortie
		this.Data.Fleets.CurrentDeckId = api_deck_id;
	}
	private UpdateFriendFleets(deckID: number): void {
		const fleets = Homeport.Instance.Fleets;
		const combined = Homeport.Instance.FleetCombined;

		this.Data.Fleets.Alias.First = new FleetData(
			fleets.get(deckID)!.Ships.map(s => new MembersShipData(s)),
			this.Data.Fleets.Alias.First
				? this.Data.Fleets.Alias.First.Formation
				: Formation.None,
			fleets.get(deckID)!.Name,
			FleetType.AliasFirst
		);
		this.Data.Fleets.Alias.Second = new FleetData(
			combined && deckID == 1
				? fleets.get(2)!.Ships.map(s => new MembersShipData(s))
				: [],
			this.Data.Fleets.Alias.Second
				? this.Data.Fleets.Alias.Second.Formation
				: Formation.None,
			fleets.get(2)!.Name,
			FleetType.AliasSecond
		);
	}


	private ClearBattleInfo(): void {
		// Reset enemy fleet
		this.Data.Fleets.Enemy.First = null;
		this.Data.Fleets.Enemy.Second = null;
		if (this.Data.Fleets.Alias.First != null)
			this.Data.Fleets.Alias.First.UpdateFormation(Formation.None);

		this.Data.UpdatedTime = Date.now();
		this.BattleFlags.Clear();

		this.Data.Battle.Engage = BattleEngage.None;
		this.Data.Battle.AirSupremacy = AirSupremacy.None;
		this.Data.Battle.AirBattleResults = [];

		// 결과 초기화
		this.Data.Battle.Rank = BattleRank.None;
		this.Data.Battle.AirRank = BattleRank.None;
		//#endregion
	}
}
export default BattleInfo;