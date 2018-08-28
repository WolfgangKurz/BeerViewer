"use strict";
!function () {
	if (!window.modules) throw "Cannot find `module`";

	const initOverviewFleet = function (id, maxShips) {
		const elem = $.new("div", "oerview-fleet");

		window.API.ObserveData("Homeport", "Organization.Fleets[" + id + "]", function (value) {
			elem.attr("data-status", (value === null) ? "disabled" : "enabled");
		});

		for (let i = 0; i < maxShips; i++) {
			const ship = $.new("div", "oerview-fleet-ship").attr("data-idx", i);
			elem.append(ship);

			!function (ship) {
				window.API.ObserveData("Homeport", "Organization.Fleets[" + id + "].Ships[" + i + "]", function (value) {
					ship.attr("data-disabled", (value === null) ? "true" : "false");
				});
			}(ship);
		}
	};

	window.modules.register("overview", {
		const: {
			fleets: 4,
			ships: 7,
		},

		init: function () {
			const _this = this;
			const overview = $.new("div").prop("id", "overview-container");

			for (let i = 0; i < this.const.fleets; i++) {
				initOverviewFleet(i + 1, this.const.ships);
			}

			window.modules.areas.register("side", "Overview", "", overview);
		}
	});
}();