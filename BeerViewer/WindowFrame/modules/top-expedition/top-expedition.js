"use strict";
!function () {
	if (!window.modules) throw "Cannot find `module`";

	const MAX_FLEETS = 4; // Maximum fleets
	const GetExpeditionState = function (exp) {
		return exp.Enabled
			? exp.Activated ? "executing" : "waiting"
			: "disabled";
	};
	const topexp = new Vue({
		data: {
			Expeditions: []
		},
		el: $("#top-expeditions"),
		methods: {
			ExpeditionState: GetExpeditionState
		}
	});
	window.modules.register("top-expedition", {
		init: function () {
			// First fleet cannot go expedition
			for (let i = 1; i < MAX_FLEETS; i++) {
				!function (i) {
					const data = {
						Enabled: false,
						Activated: false,

						Id: 0,
						RemainingText: "--:--:--",
						Progress: 0
					};

					window.API.ObserveData("Homeport", "Organization.Fleets[" + (i + 1) + "]", value => data.Enabled = value !== null);
					window.API.ObserveData("Homeport", "Organization.Fleets[" + (i + 1) + "].Expedition.IsInExecution", value => data.Activated = value);
					window.API.ObserveData("Homeport", "Organization.Fleets[" + (i + 1) + "].Expedition.Mission.DisplayNo", value => data.Id = value);
					window.API.ObserveData("Homeport", "Organization.Fleets[" + (i + 1) + "].Expedition.RemainingText", value => data.RemainingText = value);
					window.API.ObserveData("Homeport", "Organization.Fleets[" + (i + 1) + "].Expedition.Progress", value => data.Progress = value ? value.Current * 100 / value.Maximum : 0);

					topexp.Expeditions.push(data);
				}(i);
			}

			window.modules.areas.register("top", "top-expedition", "Expeditions bar", "", topexp);
		}
	});
} ();