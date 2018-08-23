﻿"use strict";
!function () {
	window.modules.register("window", {
		init: function () {
			window.CALLBACK.register("WindowState", function (_state) {
				const state = parseInt(_state);
				$("#top-systembutton").attr("data-windowstate", state);
			});

			$.all("#top-systembutton > .system-button").event("click", function () {
				window.API.SystemCall(this.attr("data-role"));
			});

			$("#top-menubutton > button").event("click", function (e) {
				const target = this.parent();
				if (target.hasClass("menu-open"))
					this.parent().removeClass("menu-open");
				else
					this.parent().addClass("menu-open");
			});

			window.API.Initialized();
		}
	});
}();