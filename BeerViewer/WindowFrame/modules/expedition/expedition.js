"use strict";
!function () {
	if (!window.modules) throw "Cannot find `module`";

	const newExpeditionView = function () {
		return $.new("div", "top-expedition")
			.append($.new("div", "expedition-progress"))
			.append(
				$.new("div", "expedition-text")
					.append($.new("div", "expedition-id"))
					.append($.new("div", "expedition-time"))
			);
	};
	window.modules.register("expedition", {
		consts: {
			count: 3
		},
		fleets: [],

		update: function (index, id, time, progress) {
			const item = this.fleets[index];
			item.find(".expedition-id").html(id);
			item.find(".expedition-time").html(time);
			item.find(".expedition-progress").css("width", progress + "%");
		},

		init: function () {
			const expeditions = $.new("div").prop("id", "top-expeditions");

			for (let i = 0; i < this.consts.count; i++) {
				let elem = newExpeditionView();
				this.fleets.push(elem);
				expeditions.append(elem);
			}
			window.modules.areas.top.append(expeditions);


			this.update(1, "37", "01:11:07", 29.9);
			this.update(2, "38", "00:43:44", 63.1);

			this.fleets[0].attr("data-status", "none");
			this.fleets[1].attr("data-status", "active");
			this.fleets[2].attr("data-status", "fail");
		}
	});
} ();