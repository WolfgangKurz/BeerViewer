"use strict";
!function () {
	if (!window.modules) throw "Cannot find `module`";

	const MAX_SHIPS = 7; // Maximum ships per fleet
	const overview = new Vue({
		data: {
			SelectedTab: 0,
			Fleets: [],
			RepairDock: [],
			ConstructionDock: [],

			LoSType: ""
		},
		el: "#overview-container",
		methods: {
			SelectFleet: function (id) {
				this.SelectedTab = id;
			}
		},
		computed: {
			i18n: async text => await i18n(text),

			LevelTooltip: function (fleet) {
				const level = fleet.Ships.reduce((a, c) => a + c.Level);
				return String.format(
					"{0}: {1}\n{2}: {3}",
					i18n("fleet_totallevel"),
					level,
					i18n("fleet_averagelevel"),
					(level / fleet.Ships.Length).toFixed(2)
				);
			},
			AATooltip: function (fleet) {
				return String.format(
					"{0}: {1}\n{2}: {3}",
					i18n("fleet_aa_min"),
					fleet.AA.Min,
					i18n("fleet_aa_max"),
					fleet.AA.Max
				);
			},
			SpeedTooltip: function (fleet) {
				return String.format("{0}: {1}", i18n("fleet_speed"), fleet.Speed);
			},
			LosTooltip: function (fleet) {
				return String.format(
					"{0}: {1}\n{2}",
					i18n("fleet_los"),
					fleet.LoS,
					this.LoSType
				);
			}
		}
	});

	const updateSize = function () {
		const el = [
			$('.fleet.display .ship:not([data-disabled="true"]) td:first-of-type'),
			$('.fleet.display .ship:not([data-disabled="true"]) td:last-of-type')
		];
		if (el[0] === null || el[1] === null) return;

		const leftSize = el[0].clientWidth;
		const rightSize = el[1].clientWidth;
		const modes = [];
		if (rightSize < 128) modes.push("mini");
		if (rightSize < 84) modes.push("tiny");
		if (rightSize < 76) modes.push("minimal");
		if (leftSize > 104) modes.push("mininame");
		$("#overview-container").attr("data-modes", modes.join(" "));
	};

	window.modules.register("overview", {
		init: function () {
			const _this = this;

			// Setup Fleets & Ships
			!function () {
				for (let i = 0; i < 4; i++) {
					!async function (fleetId) {
						const fleet = {
							Available: false,
							State: "empty",
							Ships: [],
							Speed: "",
							LoS: "0.00",
							AA: {
								Min: 0,
								Max: 0
							}
						};

						window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "]", function (value) {
							fleet.Available = value !== null;

							if (!overview.Fleets[overview.SelectedTab].Available) {
								const availables = overview.Fleets
									.map((x, y) => ({ idx: y, data: x }))
									.filter(x => x.data.Available);
								if (availables.Length > 0) overview.SelectFleet(available[0].idx);
							}
						});
						window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].State", function (value) {
							var state = "empty"; // Empty fleet (no ships)

							if ((value & 1 << 2) !== 0) // In sortie
								state = "sortie";
							else if ((value & 1 << 3) !== 0) // In expedition
								state = "expedition";

							else if ((value & 1 << 4) !== 0) // Homeport, Heavily damaged
								state = "damaged";
							else if ((value & 1 << 5) !== 0) // Homeport, Short supply
								state = "not-ready";
							else if ((value & 1 << 6) !== 0) // Homeport, Repairing
								state = "not-ready";
							else if ((value & 1 << 7) !== 0) // Homeport, Flagship is repairship
								state = "not-ready";
							else if ((value & 1 << 8) !== 0) // Homeport, Rejuvenating
								state = "not-ready";

							else if ((value & 1) !== 0) // Homeport, ready
								state = "ready";

							fleet.State = state;
						});

						window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Ships", async function (value) {
							const shipsize = await window.API.GetData("Homeport", "Organization.Fleets[" + fleetId + "].Ships.Length");
							const ships = [];
							for (let i = 0; i < shipsize; i++) {
								ships.push({
									Name: await window.API.GetData("Homeport", "Organization.Fleets[" + fleetId + "].Ships[" + i + "].Name"),
									Level: await window.API.GetData("Homeport", "Organization.Fleets[" + fleetId + "].Ships[" + i + "].Level")
								});
							}
							fleet.Ships = ships;
						});
						window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Speed", async function (value) {
							const speedList = {
								0: "fleet_speed_immovable",
								5: "fleet_speed_slow",
								10: "fleet_speed_fast",
								15: "fleet_speed_faster",
								20: "fleet_speed_fastest"
							};
							fleet.Speed = await i18n(speedList[value]);
						});
						window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].LOS", value => fleet.LoS = value.toFixed(2));
						window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].AirSuperiorityPotentialMinimum", value => fleet.AA.Min = value);
						window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].AirSuperiorityPotentialMaximum", value => fleet.AA.Max = value);

						fleet.Ships = [];
						for (let j = 0; j < MAX_SHIPS; j++)
							!function (shipId) {
								const ship = {
									Available: false,
									Situation: 0,

									Name: null,
									Level: null,
									HP: {
										Current: 0,
										Maximum: 0
									},
									Fuel: {
										Current: 0,
										Maximum: 0
									},
									Ammo: {
										Current: 0,
										Maximum: 0
									},
									Morale: {
										Value: 0,
										Level: "0"
									}
								};

								window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Ships[" + shipId + "]", value => ship.Available = value !== null);
								window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Ships[" + shipId + "].Situation", value => ship.Situation = value);

								window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Ships[" + shipId + "].Info.Name", async function (value) {
									ship.Name = await i18n(value);
									updateSize();
								});
								window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Ships[" + shipId + "].Level", value => ship.Level = value);
								window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Ships[" + shipId + "].HP", value => ship.HP = value);
								window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Ships[" + shipId + "].Fuel", value => ship.Fuel = value);
								window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Ships[" + shipId + "].Ammo", value => ship.Ammo = value);
								window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Ships[" + shipId + "].Condition", function (value) {
									let morale = "0";

									if (value >= 50) morale = "+1";
									else if (value >= 40) morale = "0";
									else if (value >= 30) morale = "-1";
									else if (value >= 20) morale = "-2";
									else morale = "-3";

									ship.Morale.Level = morale;
									ship.Morale.Value = value;
								});

								fleet.Ships.push(ship);
							}(j);

						overview.Fleets.push(fleet);
					}(i);
				}
			}();

			// Setup repair/construction dock
			!function () {
				// Repair docks
				for (let i = 0; i < 4; i++) {
					const dock = {
						Ship: "???",
						RemainingTime: "--:--:--",
						IsCompleted: false,
						State: 0
					};
					window.API.ObserveData("Homeport", "Repairyard.Docks[" + (i + 1) + "].Ship.Info.Name", value => dock.Ship = value);
					window.API.ObserveData("Homeport", "Repairyard.Docks[" + (i + 1) + "].State", value => dock.State = value);
					window.API.ObserveData("Homeport", "Repairyard.Docks[" + (i + 1) + "].IsCompleted", value => dock.IsCompleted = value);
					window.API.ObserveData("Homeport", "Repairyard.Docks[" + (i + 1) + "].RemainingText", value => dock.RemainingTime = value);

					overview.RepairDock.push(dock);
				}

				// Construction docks
				for (let i = 0; i < 4; i++) {
					const dock = {
						Ship: "???",
						RemainingTime: "--:--:--",
						IsCompleted: false,
						State: 0
					};
					window.API.ObserveData("Homeport", "Repairyard.Docks[" + (i + 1) + "].Ship.Name", value => dock.Ship = value);
					window.API.ObserveData("Homeport", "Repairyard.Docks[" + (i + 1) + "].State", value => dock.State = value);
					window.API.ObserveData("Homeport", "Repairyard.Docks[" + (i + 1) + "].IsCompleted", value => dock.IsCompleted = value);
					window.API.ObserveData("Homeport", "Repairyard.Docks[" + (i + 1) + "].RemainingText", value => dock.RemainingTime = value);

					overview.ConstructionDock.push(dock);
				}
			}();

			// Setup quests
			if(true === false) (async function () {
				const quests = $.new("div", "quest-container");

				window.API.ObserveData("Homeport", "Quests.All", async function (value) {
					const progress = ["", "50%", "80%", "100%"];

					quests.findAll("div").each(function () {
						this.remove();
					});
					if (value === null) return;

					const length = await window.API.GetData("Homeport", "Quests.All.Length");
					for (let i = 0; i < length; i++) {
						const type = await window.API.GetData("Homeport", "Quests.All[" + i + "].Category");
						const title = await window.API.GetData("Homeport", "Quests.All[" + i + "].Title");
						const progress = await window.API.GetData("Homeport", "Quests.All[" + i + "].Progress");

						const item = $.new("div", "quest-item")
							.append($.new("div", "quest-category").attr("data-quest-category", type))
							.append($.new("div", "quest-title").html(await i18n(title)))
							.append($.new("div", "quest-progress").html(progress[progress]));

						quests.append(item);
					}
				});

				overview.append(quests);
			})();
			window.addEventListener("resize", function () {
				updateSize();
			});
			updateSize();

			window.modules.areas.register("side", "Overview", "", overview);
		}
	});
}();