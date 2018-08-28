"use strict";
!function () {
	if (!window.modules) throw "Cannot find `module`";

	const newExpeditionView = function (fleetId) {
		const data = {
			enabled: false,
			active: false,
			id: 0,
			progress: 0.0,
			remaining: ""
		};
		const el = $.new("div", "top-expedition")
			.append($.new("div", "expedition-progress"))
			.append(
				$.new("div", "expedition-text")
					.append($.new("div", "expedition-id"))
					.append($.new("div", "expedition-time"))
		);
		const update = function () {
			let status = "disabled";
			if (data.enabled) {
				if (data.active) status = "executing";
				else status = "waiting";
			}

			if (status !== "executing")
				data.progress = 0.0;

			el.attr("data-status", status);
			el.find(".expedition-id").html(data.id);
			el.find(".expedition-time").html(data.remaining);
			el.find(".expedition-progress").css("width", data.progress+"%");
		};

		window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "]", function (value) {
			data.enabled = value !== null;
			update();
		});
		window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Expedition.IsInExecution", function (value) {
			data.active = value;
			update();
		});
		window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Expedition.Mission.DisplayNo", function (value) {
			data.id = value;
			update();
		});
		window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Expedition.RemainingText", function (value) {
			data.remaining = value;
			update();
		});
		window.API.ObserveData("Homeport", "Organization.Fleets[" + fleetId + "].Expedition.Progress", async function (_, ns, path) {
			const value = await window.API.GetData(ns, path); // Have to call GetData to get object data
			if (value === null)
				data.progress = 0;
			else
				data.progress = value.Current * 100 / value.Maximum;
			update();
		});

		return el;
	};
	window.modules.register("top-expedition", {
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
				let elem = newExpeditionView(i + 2);
				this.fleets.push(elem);
				expeditions.append(elem);
			}
			window.modules.areas.top.append(expeditions);
		}
	});
} ();