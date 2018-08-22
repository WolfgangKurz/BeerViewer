"use strict";
!function () {
	window.modules.register("window", {
		init: function () {
			window.CALLBACK.WindowState = function (_state) {
				var state = parseInt(_state);
				$("#top-systembutton").attr("data-windowstate", state);
			};

			$.all("#top-systembutton > .system-button").event("click", function () {
				window.API.systemCall(this.attr("data-role"));
			});

			$("#top-menubutton").event("click", function (e) {
				alert("!");
			});

			window.API.initialized();
		}
	});
}();