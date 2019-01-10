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

					window.API.ObserveData("Homeport", "Organization.Fleets[" + (i + 1) + "]", value => data.Enabled = value !== null, true);
					window.API.ObserveData("Homeport", "Organization.Fleets[" + (i + 1) + "].Expedition", value => {
						data.Activated = value.IsInExecution;
						data.Id = Mission.DisplayNo;
						data.RemainingText = value.RemainingText;
						data.progress = value.Progress ? value.Progress.Current * 100 / value.Progress.Maximum : 0;
					});
					topexp.Expeditions.push(data);
				}(i);
			}

			window.modules.areas.register("top", "top-expedition", "Expeditions bar", "", topexp);
		}
	});
} ();