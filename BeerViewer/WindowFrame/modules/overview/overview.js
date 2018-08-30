"use strict";
!function () {
	if (!window.modules) throw "Cannot find `module`";

	const initOverviewFleet = function (id, maxShips, overview) {
		const elem = $.new("table", "fleet").attr("data-idx", id);
		const tbody = $.new("tbody");
		elem.append(tbody);

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

			for (let i = 0; i < this.const.fleets; i++)
				overview.append(initOverviewFleet(i + 1, this.const.ships, overview));

			window.addEventListener("resize", function () {
				updateSize();
			});
			updateSize();

			window.modules.areas.register("side", "Overview", "", overview);
		}
	});
}();