"use strict";
!function () {
	window.Homeport = {};
	window.Homeport = new function () {
		const Subscribe = SubscribeKcsapi;
		const homeport = this;

		this.Admiral = new function () {
			const RawData = {
				api_member_id: "",
				api_nickname: "",
				// api_nickname_id: 0,
				// api_active_flag: 0,
				// api_starttime: 0,
				api_level: 0,
				api_rank: 0,
				api_experience: 0,
				// api_fleetname: null,
				api_comment: "",
				// api_comment_id: 0,
				api_max_chara: 0,
				api_max_slotitem: 0,
				// api_max_kagu: 0,
				// api_playtime: 0,
				// api_tutorial: 0,
				api_furniture: [],
				api_count_deck: 0,
				api_count_kdock: 0,
				api_count_ndock: 0,
				api_fcoin: 0,
				api_st_win: 0,
				api_st_lose: 0
				// api_ms_count: 0,
				// api_ms_success: 0,
				// api_pt_win: 0,
				// api_pt_lose: 0,
				// api_pt_challenged: 0,
				// api_pt_challenged_win: 0,
				// api_firstflag: 0,
				// api_tutorial_progress: 0,
				// api_pvp: []
			};

			Subscribe("api_port/port", x => RawData = x.api_basic);
			Subscribe( // Update comment
				"api_req_member/updatecomment",
				(x, y) => RawData.api_comment = y.api_cmt
			);

			this.Readonly("MemberId", () => RawData.api_member_id);
			this.Readonly("Nickname", () => RawData.api_nickname);
			this.Readonly("Comment", () => RawData.api_comment);
			this.Readonly("Experience", () => RawData.api_experience);
			this.Readonly("ExperienceForNexeLevel", () => Homeport.Experience[this.RawData.api_level + 1].Total - this.RawData.api_experience);
			this.Readonly("Level", () => RawData.api_level);
			this.Readonly("Rank", () => RawData.api_rank);
			this.Readonly("SortieWins", () => RawData.api_st_win);
			this.Readonly("SortieLoses", () => RawData.api_st_lose);
			this.Readonly("MaxShips", () => RawData.api_max_chara);
			this.Readonly("MaxEquips", () => RawData.api_max_slotitem);
			this.Readonly("ResourceLimit", () => (RawData.api_level + 3) * 250);
			this.Readonly("SortieWinningRate", () => {
				const battleCount = RawData.api_st_win + RawData.api_st_lose;
				return battleCount === 0 ? 0 : x.api_st_win / battleCount;
			});
		};
		this.Materials = new function () {
			const RawData = {
				Fuel: 0,
				Ammo: 0,
				Steel: 0,
				Bauxite: 0,
				InstantConstruction: 0,
				RepairBucket: 0,
				DevelopmentMaterial: 0,
				ImprovementMaterial: 0
			};
			const Update = function (source) {
				if (!source || !Array.isArray(source)) return;
				if (source.length >= 4) {
					RawData.Fuel = source[0];
					RawData.Ammo = source[1];
					RawData.Steel = source[2];
					RawData.Bauxite = source[3];

					if (source.length >= 8) {
						RawData.InstantConstruction = source[4];
						RawData.RepairBucket = source[5];
						RawData.DevelopmentMaterial = source[6];
						RawData.ImprovementMaterial = source[7];
					}
				}
			};

			Subscribe("api_port/port", x => Update(x.api_material));
			Subscribe("api_get_member/material", x => Update(x));
			Subscribe("api_req_hokyu/charge", x => Update(x.api_material));
			Subscribe("api_req_kousyou/destroyship", x => Update(x.api_material));
			Subscribe("api_req_kousyou/destroyitem2", x => Update(x.api_material));

			// Supply Airbase
			Subscribe("api_req_air_corps/supply", x => Update([
				x.Data.api_after_fuel,
				RawData.Ammunition,
				RawData.Steel,
				x.Data.api_after_bauxite
			]));

			// Set aircraft to AirBase
			Subscribe("api_req_air_corps/set_plane", (x, y) => {
				if (y.api_item_id === "-1") return;
				if (x.api_plane_info.length >= 2) return;
				this.Update([RawData.Fuel, RawData.Ammunition, RawData.Steel, x.Data.api_after_bauxite]);
			});

			this.Readonly("Fuel", () => RawData.Fuel);
			this.Readonly("Ammo", () => RawData.Ammo);
			this.Readonly("Steel", () => RawData.Steel);
			this.Readonly("Bauxite", () => RawData.Bauxite);
			this.Readonly("InstantConstruction", () => RawData.InstantConstruction);
			this.Readonly("RepairBucket", () => RawData.RepairBucket);
			this.Readonly("DevelopmentMaterial", () => RawData.DevelopmentMaterial);
			this.Readonly("ImprovementMaterial", () => RawData.ImprovementMaterial);
		};
		this.RepairDock = new function () {
			const RawData = {
			};
			const Start = function (x, y) {
				const ship = Homeport.Ships[y.api_ship_id];
				if (!ship) return;

				if (y.api_highspeed === "1") ship.Repair();
			};
			const Update = function (x) {
				const t = [];
				for (let i = 0; i < x.length; i++)
					t.push(new Class.RepairDock(homeport, x[i]));
				RawData.Docks = t;
			};

			Subscribe("api_get_member/ndock", x => Update(x));
			Subscribe("api_req_nyukyo/start", (x, y) => Start(x, y));
			Subscribe("api_req_nyukyo/speedchange", x => SpeedChange(x)); // Used bucket while normal repair


		};

		Subscribe("api_port/port", x => {
			this.UpdateAdmiral(x.api_basic);
			this.Organization.Update(x.api_ship);
			this.Repairyard.Update(x.api_ndock);
			this.Organization.Update(x.api_deck_port);
			this.Organization.Combined = x.api_combined_flag != 0;
			this.Materials.Update(x.api_material);
		});
	};
}();