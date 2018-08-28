"use strict";
!function () {
	if (!window.modules) throw "Cannot find `module`";

	const newResourceView = function (id) {
		const elem = $.new("div", "top-resource")
			.append(
				$.new("img", "resource-icon")
					.prop("src", "modules/top-resource/icon_" + id.toString().toLowerCase() + ".png")
			)
			.append($.new("div", "resource-value").html("0"));

		window.API.ObserveData("Homeport", "Materials."+id, function (value) {
			elem.find(".resource-value").html(value);
		});

		return elem;
	};

	window.modules.register("top-resource", {
		consts: {
			list: [
				"Fuel", "Ammo", "Steel", "Bauxite",
				"RepairBucket", "ImprovementMaterial"
			]
		},

		resourceLimit: 0,
		resources: [],

		update: function (index, value) {
			const item = this.resources[index];

			if (typeof value !== "undefined") {
				value = parseInt(value);
				item.find(".resource-value").html(value);
			} else
				value = parseInt(item.html());

			item.attr("data-overlimit", value >= this.resourceLimit ? "1" : "0");
		},
		updateLimit: function (value) {
			this.resourceLimit = value;
			for (let i = 0; i < this.resources.length; i++)
				this.update(i);
		},

		init: function () {
			const resources = $.new("div").prop("id", "top-resources");

			for (let i = 0; i < this.consts.list.length; i++) {
				let elem = newResourceView(this.consts.list[i]);
				this.resources.push(elem);
				resources.append(elem);
			}
			window.modules.areas.register("top", "Resources bar", "", resources);
		}
	});
}();