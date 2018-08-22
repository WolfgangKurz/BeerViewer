"use strict";
!function () {
	if (!window.modules) throw "Cannot find `module`";

	var newExpedition = function () {
		return $.new("div", "top-expedition")
			.append($.new("div", "expedition-id"))
			.append($.new("div", "expedition-time"));
	};
	window.modules.register("expedition", {
		consts: {
			count: 3
		},
		fleets: [],

		update: function (index, id, time) {
		},

		init: function () {
			const expeditions = $.new("div", "top-expeditions")
				.event("mousedown", function (e) {
					e.preventDefault();
					return false;
				});

			for (let i = 0; i < this.consts.count; i++) {
				let elem = newExpedition();
				this.fleets.push(elem);
				expeditions.append(elem);
			}

			window.modules.areas.top.append();
		}
	});
} ();