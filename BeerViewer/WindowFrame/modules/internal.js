"use strict";
!async function () {
	let _origin_i18n = {};

	window.INTERNAL = (function () {
		return {
			Initialized: async function () {
				const set = await window.API.i18nSet();
				const props = Object.getOwnPropertyNames(_origin_i18n);
				for (let i = 0; i < props.length; i++) delete _origin_i18n[props[i]];
				for (let i in set) _origin_i18n[i] = set[i];
			},
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
			},
			focusWindow: function (focused) {
				const mainWindow = $("html");
				if (focused)
					mainWindow.addClass("focused");
				else
					mainWindow.removeClass("focused");
			}
		};
	})();

	window.i18n = new Proxy(_origin_i18n, {
		get: function (target, name) {
			if (name in target) return target[name];
			return name;
		}
	});
	window._i18n = async function (text) {
		return await window.API.i18n(text);
	};
}();