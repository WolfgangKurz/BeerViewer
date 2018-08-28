"use strict";
!function () {
	window.INTERNAL = (function () {
		return {
			zoomMainFrame: function (_zoomFactor) {
				const zoomFactor = parseFloat(_zoomFactor) / 100;
				const frame = $("#MAIN_FRAME");
				if (frame === null) return;

				frame.css("transform", String.format("scale({0})", zoomFactor))
					.css("margin-right", 1200 * (zoomFactor - 1) + "px")
					.css("margin-bottom", 720 * (zoomFactor - 1) + "px");
			},
			loadMainFrame: function (url) {
				const frame = $("#MAIN_FRAME");
				if (frame === null) return;

				frame.prop("src", url);
			}
		};
	})();
}();