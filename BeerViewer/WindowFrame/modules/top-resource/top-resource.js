"use strict";
!function () {
	if (!window.modules) throw "Cannot find `module`";

	const topres = new Vue({
		data: {
			Resources: [],
			Overlimit: 5000
		},
		el: $("#top-resources")
	});

	window.modules.register("top-resource", {
		init: function () {
			["Fuel", "Ammo", "Steel", "Bauxite", "RepairBucket", "ImprovementMaterial"].forEach(x => {
				const res = {
					Name: x,
					lowName: x.toLowerCase(),
					Value: 0
				};
				topres.Resources.push(res);
			});
			window.API.ObserveData("Homeport", "Materials", value => {
				if (!value) return;
				topres.Resources[0].Value = parseInt(value.Fuel || 0) || 0;
				topres.Resources[1].Value = parseInt(value.Ammo || 0) || 0;
				topres.Resources[2].Value = parseInt(value.Steel || 0) || 0;
				topres.Resources[3].Value = parseInt(value.Bauxite || 0) || 0;
				topres.Resources[4].Value = parseInt(value.RepairBucket || 0) || 0;
				topres.Resources[5].Value = parseInt(value.ImprovementMaterial || 0) || 0;
			});
			window.API.ObserveData("Homeport", "Admiral.ResourceLimit", value => topres.Overlimit = parseInt(value || 0) || 0);

			window.modules.areas.register("top", "top-resource", "Resources bar", "", topres);
		}
	});
}();