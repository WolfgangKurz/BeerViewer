"use strict";
!function () {
	if (!window.modules) throw "Cannot find `module`";

	const getTextWidth = function (text, font) {
		const canvas = getTextWidth.canvas || (getTextWidth.canvas = document.createElement("canvas"));
		const context = canvas.getContext("2d");
		context.font = font;
		return context.measureText(text).width;
	};

	const MAX_FLEETS = 4; // Maximum fleets
	const MAX_SHIPS = 7; // Maximum ships per fleet
	const MAX_SLOTS = 5 + 1; // Maximum equipment slots + extra slot
	const overview = new Vue({
		data: {
			i18n: window.i18n,

			SelectedTab: 1,
			Fleets: [],
			RepairDock: [],
			ConstructionDock: [],

			LoSType: ""
		},
		el: $("#overview-container"),
		methods: {
			SelectFleet: function (id) {
				this.SelectedTab = id;
			},

			LevelTooltip: function (fleet) {
				return String.format(
					"{0}: {1}\n{2}: {3}",
					i18n.fleet_totallevel,
					fleet.TotalLevel,
					i18n.fleet_averagelevel,
					fleet.AvgLevel.toFixed(2)
				);
			},
			AATooltip: function (fleet) {
				return String.format(
					"{0}: {1}\n{2}: {3}",
					i18n.fleet_aa_min,
					fleet.AA.Min,
					i18n.fleet_aa_max,
					fleet.AA.Max
				);
			},
			SpeedTooltip: function (fleet) {
				return String.format("{0}: {1}", i18n.fleet_speed, fleet.Speed);
			},
			LoSTooltip: function (fleet) {
				return String.format(
					"{0}: {1}\n{2}",
					i18n.fleet_los,
					fleet.LoS,
					this.LoSType
				);
			},

			ParseShipSituation: function (ship) {
				const status = [];
				if (ship.Situation & 1) status.push("repairing");
				else if (ship.Situation & 1 << 1) status.push("evacuation");
				else if (ship.Situation & 1 << 2) status.push("tow");
				else if (ship.Situation & 1 << 3) status.push("damaged");
				else if (ship.Situation & 1 << 4) status.push("damagecontrolled");
				return status.join(" ");
			}
		},
		watch: {
			Fleets: {
				handler: function (val) {
					const font = `14px ${getComputedStyle(document.body)["fontFamily"]}`;
					// See overview.scss:137

					val.forEach(x => {
						x.NameSize = x.Ships
							.reduce((a, c) => Math.max(
								a,
								getTextWidth(c.Name || "", font)
							), 0)
							+ 6; // See overview.scss:131
					});
				},
				deep: true
			}
		}
	});

	const updateSize = function () {
		const el = [
			$('.fleet[data-display="1"] .ship td:first-of-type'),
			$('.fleet[data-display="1"] .ship td:last-of-type')
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

			const DefaultLimitedValue = {
				Current: 0,
				Maximum: 0
			};

			// Setup Fleets & Ships
			!function () {
				for (let i = 1; i <= MAX_FLEETS; i++) {
					!async function (fleetId) {
						const fleet = {
							Id: i,
							Available: false,
							State: "empty",
							Ships: [],

							TotalLevel: 0,
							AvgLevel: 0,
							Speed: "",
							LoS: "0.00",
							AA: {
								Min: 0,
								Max: 0
							},

							SupplyFuel: 0,
							SupplyAmmo: 0,
							SupplyBauxite: 0,

							IsRejuvenating: false,
							RejuvenateText: "--:--:--",

							NameSize: 0
						};
						const updateFleetLevel = function () {
							fleet.TotalLevel = fleet.Ships.filter(x => x.Available).reduce((a, c) => a + c.Level, 0);
							fleet.AvgLevel = fleet.Ships.length > 0 ? fleet.TotalLevel / fleet.Ships.length : 0;
						};
						const updateFleetSupply = function () {
							fleet.SupplyFuel = fleet.Ships.filter(x => x.Available).reduce((a, c) => a + (c.Fuel.Maximum - c.Fuel.Current), 0);
							fleet.SupplyAmmo = fleet.Ships.filter(x => x.Available).reduce((a, c) => a + (c.Ammo.Maximum - c.Ammo.Current), 0);
							fleet.SupplyBauxite = fleet.Ships.filter(x => x.Available).reduce((a, c) => a + c.UsedBauxite, 0);
						};

						window.API.ObserveData("Homeport", `Organization.Fleets[${fleetId}]`, function (value) {
							fleet.Available = value !== null;

							if (!overview.Fleets[overview.SelectedTab].Available) {
								const availables = overview.Fleets
									.map((x, y) => ({ idx: y, data: x }))
									.filter(x => x.data.Available);
								if (availables.Length > 0) overview.SelectFleet(available[0].idx);
							}
						});
						window.API.ObserveData("Homeport", `Organization.Fleets[${fleetId}].State`, function (value) {
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
						window.API.ObserveData("Homeport", `Organization.Fleets[${fleetId}].Speed`, function (value) {
							const speedList = {
								0: "fleet_speed_immovable",
								5: "fleet_speed_slow",
								10: "fleet_speed_fast",
								15: "fleet_speed_faster",
								20: "fleet_speed_fastest"
							};
							fleet.Speed = i18n[speedList[value]];
						});
						window.API.ObserveData("Homeport", `Organization.Fleets[${fleetId}].LOS`, value => fleet.LoS = parseFloat(value).toFixed(2));
						window.API.ObserveData("Homeport", `Organization.Fleets[${fleetId}].AirSuperiorityPotentialMinimum`, value => fleet.AA.Min = value);
						window.API.ObserveData("Homeport", `Organization.Fleets[${fleetId}].AirSuperiorityPotentialMaximum`, value => fleet.AA.Max = value);
						window.API.ObserveData("Homeport", `Organization.Fleets[${fleetId}].IsRejuvenating`, value => fleet.IsRejuvenating = value);
						window.API.ObserveData("Homeport", `Organization.Fleets[${fleetId}].RejuvenateText`, value => fleet.RejuvenateText = value);

						for (let j = 0; j < MAX_SHIPS; j++)
							!function (shipId) {
								const ship = {
									Id: j,
									Hash: `${i}${j}`,
									Available: false,
									Situation: 0,

									Slots: [],

									Name: null,
									Level: null,
									HP: DefaultLimitedValue,
									Fuel: DefaultLimitedValue,
									Ammo: DefaultLimitedValue,
									UsedBauxite: 0,
									Morale: {
										Value: 0,
										Level: "0"
									}
								};
								for (let c = 0; c < MAX_SLOTS; c++) {
									ship.Slots.push({
										Available: false, // Slot exists?
										Equiped: false, // Slot using?

										Aircraft: {
											Maximum: 0,
											Current: 0
										}, // Aircraft count
										Equipment: {
											Name: "",
											Level: 0
										}
									});
								}

								window.API.ObserveData("Homeport", `Organization.Fleets[${fleetId}].Ships[${shipId}]`, value => ship.Available = value !== null);
								window.API.ObserveData("Homeport", `Organization.Fleets[${fleetId}].Ships[${shipId}].Situation`, value => ship.Situation = value);

								window.API.ObserveData("Homeport", `Organization.Fleets[${fleetId}].Ships[${shipId}].Info.Name`, function (value) {
									ship.Name = i18n[value];
									updateSize();
								});
								window.API.ObserveData("Homeport", `Organization.Fleets[${fleetId}].Ships[${shipId}].Level`, value => {
									ship.Level = value || 0;
									updateFleetLevel();
								});
								window.API.ObserveData("Homeport", `Organization.Fleets[${fleetId}].Ships[${shipId}].HP`, value => ship.HP = value || DefaultLimitedValue);
								window.API.ObserveData("Homeport", `Organization.Fleets[${fleetId}].Ships[${shipId}].Fuel`, value => {
									ship.Fuel = value || DefaultLimitedValue;
									updateFleetSupply();
								});
								window.API.ObserveData("Homeport", `Organization.Fleets[${fleetId}].Ships[${shipId}].Ammo`, value => {
									ship.Ammo = value || DefaultLimitedValue;
									updateFleetSupply();
								});
								window.API.ObserveData("Homeport", `Organization.Fleets[${fleetId}].Ships[${shipId}].UsedBauxite`, value => {
									ship.UsedBauxite = parseInt(value || 0) || 0;
									updateFleetSupply();
								});
								window.API.ObserveData("Homeport", `Organization.Fleets[${fleetId}].Ships[${shipId}].Condition`, function (value) {
									let morale = "0";

									if (value >= 50) morale = "+1";
									else if (value >= 40) morale = "0";
									else if (value >= 30) morale = "-1";
									else if (value >= 20) morale = "-2";
									else morale = "-3";

									ship.Morale.Level = morale;
									ship.Morale.Value = value;
								});

								window.API.ObserveData("Homeport", `Organization.Fleets[${fleetId}].Ships[${shipId}].Slots.Length`, function (value) {
									// ship.Slots
									console.log(value);
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
					const dockState = {
						"-1": "locked",
						0: "empty",
						1: "repairing",
						2: "done"
					};
					const dock = {
						Id: i + 1,
						Ship: "???",
						RemainingTime: "--:--:--",
						IsCompleted: false,
						State: 0,
						StateText: "locked"
					};
					window.API.ObserveData("Homeport", `Repairyard.Docks[${i + 1}].Ship.Info.Name`, value => dock.Ship = value);
					window.API.ObserveData("Homeport", `Repairyard.Docks[${i + 1}].State`, value => {
						dock.State = value || -1;
						dock.StateText = dockState[value || -1];
					});
					window.API.ObserveData("Homeport", `Repairyard.Docks[${i + 1}].IsCompleted`, value => dock.IsCompleted = value);
					window.API.ObserveData("Homeport", `Repairyard.Docks[${i + 1}].RemainingText`, value => dock.RemainingTime = value);

					overview.RepairDock.push(dock);
				}

				// Construction docks
				for (let i = 0; i < 4; i++) {
					const dockState = {
						"-1": "locked",
						0: "empty",
						2: "building",
						3: "done"
					};
					const dock = {
						Id: i + 1,
						Ship: "???",
						RemainingTime: "--:--:--",
						IsCompleted: false,
						State: 0,
						StateText: "locked"
					};
					window.API.ObserveData("Homeport", `Dockyard.Docks[${i + 1}].Ship.Name`, value => dock.Ship = value);
					window.API.ObserveData("Homeport", `Dockyard.Docks[${i + 1}].State`, value => {
						dock.State = value || -1;
						dock.StateText = dockState[value || -1];
					});
					window.API.ObserveData("Homeport", `Dockyard.Docks[${i + 1}].IsCompleted`, value => dock.IsCompleted = value);
					window.API.ObserveData("Homeport", `Dockyard.Docks[${i + 1}].RemainingText`, value => dock.RemainingTime = value);

					overview.ConstructionDock.push(dock);
				}
			}();

			// Setup quests
			/*
			(async function () {
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
			*/
			window.addEventListener("resize", function () {
				updateSize();
			});
			updateSize();

			window.modules.areas.register("side", "overview", "Overview", "", overview);
		}
	});
}();