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
				window.API.ObserveData("Homeport", "Materials." + x, value => res.Value = parseInt(value || 0) || 0);
			});
			window.API.ObserveData("Homeport", "Admiral.ResourceLimit", value => topres.Overlimit = parseInt(value || 0) || 0);

			window.modules.areas.register("top", "Resources bar", "", topres);
		}
	});
}();