"use strict";
!function () {
	if (!window.modules) throw "Cannot find `module`";

	const initOverviewFleet = function (id, maxShips, overview) {
		const elem = $.new("table", "fleet").attr("data-idx", id);
		const tbody = $.new("tbody");
		elem.append(tbody);

		!function () {
			const summary_container = $.new("div", "summary-container");
			const summary_list = ["level", "aa", "speed", "los"];

			summary_list.forEach(function (x) {
				summary_container.append(
					$.new("div", "summary-info")
						.attr("data-summary", x)
				);
			});

			const updateSummary = async function () {
				const speedList = {
					0: "fleet_speed_immovable",
					5: "fleet_speed_slow",
					10: "fleet_speed_fast",
					15: "fleet_speed_faster",
					20: "fleet_speed_fastest"
				};
				const ship_length = await window.API.GetData("Homeport", "Organization.Fleets[" + id + "].Ships.Length");
				let level = 0;
				for (let i = 0; ship_length && i < ship_length; i++) {
					level += await window.API.GetData("Homeport", "Organization.Fleets[" + id + "].Ships[" + i + "].Level");
				}
				const avglevel = (!ship_length ? 0 : (level / ship_length)).toFixed(2);

				const speed = await window.API.GetData("Homeport", "Organization.Fleets[" + id + "].Speed");

				let los = await window.API.GetData("Homeport", "Organization.Fleets[" + id + "].LOS");
				if (los === null) los = 0;
				los = los.toFixed(2);

				const aa = [
					await window.API.GetData("Homeport", "Organization.Fleets[" + id + "].AirSuperiorityPotentialMinimum"),
					await window.API.GetData("Homeport", "Organization.Fleets[" + id + "].AirSuperiorityPotentialMaximum")
				];

				const leveltext = String.format(
					"{0} ({1})",
					level,
					avglevel
				);
				const speedtext = await window.API.i18n(speedList[speed]);

				const level_tooltip = String.format(
					"{0}: {1}\n{2}: {3}",
					await window.API.i18n("fleet_totallevel"),
					level,
					await window.API.i18n("fleet_averagelevel"),
					avglevel
				);
				const aa_tooltip = String.format(
					"{0}: {1}\n{2}: {3}",
					await window.API.i18n("fleet_aa_min"),
					aa[0],
					await window.API.i18n("fleet_aa_max"),
					aa[1]
				);
				const speed_tooltip = String.format(
					"{0}: {1}",
					await window.API.i18n("fleet_speed"),
					speedtext
				);
				const los_tooltip = String.format(
					"{0}: {1}\n{2}: {3}",
					await window.API.i18n("fleet_los"),
					los,
					await window.API.i18n("fleet_los_type"),
					"???" // lostype
				);

				elem.find('.summary-info[data-summary="level"]')
					.prop("title", level_tooltip)
					.html(leveltext);

				elem.find('.summary-info[data-summary="aa"]')
					.prop("title", aa_tooltip)
					.html(String.format("{0}~{1}", aa[0], aa[1]));

				elem.find('.summary-info[data-summary="speed"]')
					.prop("title", speed_tooltip)
					.html(speedtext);

				elem.find('.summary-info[data-summary="los"]')
					.prop("title", los_tooltip)
					.html(los);
			};

			window.API.ObserveData("Homeport", "Organization.Fleets[" + id + "].Ships", function (value) {
				updateSummary();
			});
			window.API.ObserveData("Homeport", "Organization.Fleets[" + id + "].Speed", function (value) {
				updateSummary();
			});
			window.API.ObserveData("Homeport", "Organization.Fleets[" + id + "].LOS", function (value) {
				updateSummary();
			});
			window.API.ObserveData("Homeport", "Organization.Fleets[" + id + "].AirSuperiorityPotentialMinimum", function (value) {
				updateSummary();
			});
			window.API.ObserveData("Homeport", "Organization.Fleets[" + id + "].AirSuperiorityPotentialMaximum", function (value) {
				updateSummary();
			});

			tbody.append(
				$.new("tr", "summary")
					.append(
						$.new("td").attr("colSpan", "2")
							.append(summary_container)
					)
			);
		}();

		window.API.ObserveData("Homeport", "Organization.Fleets[" + id + "]", function (value) {
			const disabled = value === null;
			elem.attr("data-status", disabled ? "disabled" : "enabled");

			overview.find('.tab-host > a[data-idx="' + id + '"]')
				.attr("data-hide", disabled ? "true" : "false");

			if (overview.find('.tab-host > a.selected:not([data-hide="true"])') === null) {
				const first = overview.find('.tab-host > a:not([data-hide="true"])');
				if(first) first.trigger("click");
			}
		});
		window.API.ObserveData("Homeport", "Organization.Fleets[" + id + "].State", function (value) {
			const target = $('.tab-host .overview-fleet-status[data-idx="' + id + '"]');
			if (!target) return;

			if ((value & (1 << 2)) !== 0) // In sortie
				target.attr("data-status", "sortie");
			else if ((value & (1 << 3)) !== 0) // In expedition
				target.attr("data-status", "expedition");

			else if ((value & (1 << 4)) !== 0) // Homeport, Heavily damaged
				target.attr("data-status", "damaged");
			else if ((value & (1 << 5)) !== 0) // Homeport, Short supply
				target.attr("data-status", "not-ready");
			else if ((value & (1 << 6)) !== 0) // Homeport, Repairing
				target.attr("data-status", "not-ready");
			else if ((value & (1 << 7)) !== 0) // Homeport, Flagship is repairship
				target.attr("data-status", "not-ready");
			else if ((value & (1 << 8)) !== 0) // Homeport, Rejuvenating
				target.attr("data-status", "not-ready");

			else if ((value & 1) !== 0) // Homeport, ready
				target.attr("data-status", "ready");
			else // Empty fleet (no ships)
				target.attr("data-status", "empty");
		});

		for (let i = 0; i < maxShips; i++) {
			const ship = $.new("tr", "ship")
				.attr("data-idx", i);

			(async function (ship, i, tbody, fleetId) {
				ship
					.append(
						$.new("td")
							.append(
								$.new("div", "ship-name")
									.prop("title", "???")
									.html("???")
							)
							.append(
								$.new("div", "ship-level")
									.append($.new.text("Lv."))
									.append($.new("span", "ship-level-value").html("???"))
							)
					)
					.append(
						$.new("td")
							.append(
								$.new("div", "ship-hp-bar")
									.attr("data-type", "progress")
									.attr("data-progress-strip", "4")
									.attr("data-progress", "0")
							)
							.append(
								$.new("div", "ship-hp")
									.append($.new("span", "ship-hp-current").html("0"))
									.append($.new.text("/"))
									.append($.new("span", "ship-hp-maximum").html("0"))
							)
							.append(
								$.new("div", "ship-supply")
									.append(
										$.new("div", "ship-fuel")
											.attr("data-type", "progress")
											.attr("data-progress-strip", "5")
											.attr("data-progress", "0")
									)
									.append(
										$.new("div", "ship-ammo")
											.attr("data-type", "progress")
											.attr("data-progress-strip", "5")
											.attr("data-progress", "0")
									)
							)
							.append(
								$.new("div", "ship-morale")
									.append(
										$.new("div", "ship-morale-box")
											.attr("data-morale", "-3")
									)
									.append(
										$.new("div", "ship-morale-value")
											.attr("data-morale", "0")
									)
							)
							.append(
								$.new("div", "ship-repairing")
									.append(
										$.new("span").html(await window.API.i18n("fleet_repairing"))
									)
							)
					);

				window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Ships[" + i + "]", function (value) {
					ship.attr("data-disabled", value === null ? "true" : "false");
				});
				window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Ships[" + i + "].Situation", function (value) {
					if ((value & 1) == 1) // Repairing
						ship.attr("data-status", "repairing");
					else
						ship.attr("data-status", "");
				});

				window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Ships[" + i + "].Info.Name", async function (value) {
					const name = await window.API.i18n(value);
					ship.find(".ship-name")
						.prop("title", name)
						.html(name);
					updateSize();
				});
				window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Ships[" + i + "].Level", function (value) {
					ship.find(".ship-level-value").html(value);
				});
				window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Ships[" + i + "].HP", function (value) {
					if (value === null) return;

					ship.find(".ship-hp-current").html(value.Current);
					ship.find(".ship-hp-maximum").html(value.Maximum);
					ship.find(".ship-hp-bar").attr("data-progress", 100 * value.Current / value.Maximum);
				});
				window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Ships[" + i + "].Fuel", function (value) {
					if (value === null) return;

					ship.find(".ship-fuel")
						.prop("title", value.Current + " / " + value.Maximum)
						.attr("data-progress", 100 * value.Current / value.Maximum);
				});
				window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Ships[" + i + "].Ammo", function (value) {
					if (value === null) return;

					ship.find(".ship-ammo")
						.prop("title", value.Current + " / " + value.Maximum)
						.attr("data-progress", 100 * value.Current / value.Maximum);
				});
				window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Ships[" + i + "].Condition", function (value) {
					let morale = "0";

					if (value >= 50)
						morale = "+1";
					else if (value >= 40)
						morale = "0";
					else if (value >= 30)
						morale = "-1";
					else if (value >= 20)
						morale = "-2";
					else
						morale = "-3";

					ship.find(".ship-morale-value").html(value);
					ship.find(".ship-morale-box").attr("data-morale", morale);
				});

				tbody.append(ship);
			})(ship, i, tbody, id);
		}

		return elem;
	};

	const updateSize = function () {
		var el = [
			$('.fleet.display .ship:not([data-disabled="true"]) td:first-of-type'),
			$('.fleet.display .ship:not([data-disabled="true"]) td:last-of-type')
		];
		if (el[0] === null || el[1] === null) return;

		var leftSize = el[0].clientWidth;
		var rightSize = el[1].clientWidth;
		var modes = [];
		if (rightSize < 128) modes.push("mini");
		if (rightSize < 84) modes.push("tiny");
		if (rightSize < 76) modes.push("minimal");
		if (leftSize > 104) modes.push("mininame");
		$("#overview-container").attr("data-modes", modes.join(" "));
	};

	window.modules.register("overview", {
		const: {
			fleets: 4,
			ships: 7
		},

		init: function () {
			const _this = this;
			const overview = $.new("div").prop("id", "overview-container");

			// Setup fleet tab
			!function () {
				const host = $.new("div", "tab-host");
				for (let i = 0; i < 4; i++) {
					host.append(
						$.new("a")
							.prop("href", "#")
							.attr("data-idx", i + 1)
							.attr("data-hide", "true")
							.append(
								$.new("div", "overview-fleet-status")
									.attr("data-idx", i + 1)
							)
							.append(
								$.new.text("#" + (i + 1))
							)
							.event("click", function (e) {
								e.preventDefault();

								this.parent().findAll("a").removeClass("selected");
								this.addClass("selected");

								overview.findAll(".fleet").removeClass("display");

								const fleet = overview.find('.fleet[data-idx="' + (i + 1) + '"]');
								if (!fleet) return false;

								fleet.addClass("display");
								return false;
							})
					);
				}

				overview.append(host);
			}();

			// Setup fleets
			for (let i = 0; i < this.const.fleets; i++)
				overview.append(initOverviewFleet(i + 1, this.const.ships, overview));

			// Setup repair/construction dock
			(async function () {
				for (let type = 0; type < 2; type++) {
					const dock = $.new("div", "dock-container");

					for (let i = 0; i < 4; i++) {
						const item = $.new("div", "dock-item");
						item.append($.new("div", "dock-text"));
						item.append($.new("div", "dock-time"));

						!function (item, type) {
							switch (type) {
								case 0: // Repair dock
									window.API.ObserveData("Homeport", "Repairyard.Docks[" + (i + 1) + "].Ship.Info.Name", async function (value) {
										const state = await window.API.GetData("Homeport", "Repairyard.Docks[" + (i + 1) + "].State");
										if (state === 1)
											item.find("div.dock-text").html(await window.API.i18n(value));
									});
									window.API.ObserveData("Homeport", "Repairyard.Docks[" + (i + 1) + "].State", async function (value) {
										if (value === -1 || value === null) {
											item.attr("data-status", "locked");
											item.find("div.dock-text").html(await window.API.i18n("fleet_locked"));
										}
										else if (value === 0) {
											item.attr("data-status", "empty");
											item.find("div.dock-text").html(await window.API.i18n("fleet_repair_empty"));
										}
										else if (value === 1) {
											item.attr("data-status", "repairing");
											item.find("div.dock-time").html("--:--:--"); // Default value

											const name = await window.API.GetData("Homeport", "Repairyard.Docks[" + (i + 1) + "].Ship.Info.Name");
											if (name !== null)
												item.find("div.dock-text").html(await window.API.i18n(name));
											else
												item.find("div.dock-text").html("???");
										}
									});
									window.API.ObserveData("Homeport", "Repairyard.Docks[" + (i + 1) + "].RemainingText", async function (value) {
										const completed = await window.API.GetData("Homeport", "Repairyard.Docks[" + (i + 1) + "].IsCompleted");

										if(completed)
											item.find("div.dock-time").html(await window.API.i18n("fleet_done"));
										else
											item.find("div.dock-time").html(value);
									});
									break;

								case 1: // Construction dock
									window.API.ObserveData("Homeport", "Dockyard.Docks[" + (i + 1) + "].Ship.Name", async function (value) {
										item.find("div.dock-text").html(await window.API.i18n(value));
									});
									window.API.ObserveData("Homeport", "Dockyard.Docks[" + (i + 1) + "].State", async function (value) {
										if (value === -1 || value === null) {
											item.attr("data-status", "locked");
											item.find("div.dock-text").html(await window.API.i18n("fleet_locked"));
										}
										else if (value === 0) {
											item.attr("data-status", "empty");
											item.find("div.dock-text").html(await window.API.i18n("fleet_repair_empty"));
										}
										else if (value === 2) {
											item.attr("data-status", "building");
											item.find("div.dock-time").html("--:--:--"); // Default value

											const name = await window.API.GetData("Homeport", "Dockyard.Docks[" + (i + 1) + "].Ship.Name");
											if (name !== null)
												item.find("div.dock-text").html(await window.API.i18n(name));
											else
												item.find("div.dock-text").html("???");
										}
										else if (value === 3) {
											item.attr("data-status", "done");
											item.find("div.dock-time").html(await window.API.i18n("fleet_done"));

											const name = await window.API.GetData("Homeport", "Dockyard.Docks[" + (i + 1) + "].Ship.Name");
											if (name !== null)
												item.find("div.dock-text").html(await window.API.i18n(name));
											else
												item.find("div.dock-text").html("???");
										}
									});
									window.API.ObserveData("Homeport", "Dockyard.Docks[" + (i + 1) + "].RemainingText", async function (value) {
										const completed = await window.API.GetData("Homeport", "Dockyard.Docks[" + (i + 1) + "].IsCompleted");

										if (completed)
											item.find("div.dock-time").html(await window.API.i18n("fleet_done"));
										else
											item.find("div.dock-time").html(value);
									});
									break;
							}
						}(item, type);

						dock.append(item);
					}
					overview.append(dock);
				}
			})();

			window.addEventListener("resize", function () {
				updateSize();
			});
			updateSize();

			window.modules.areas.register("side", "Overview", "", overview);
		}
	});
}();