"use strict";
!function () {
	window.modules = {
		initialized: false,
		list: {},

		load: function (name, css) {
			if (name in this.list) throw "Tried to load module already loaded";
			this.list[name] = null;

			document.head.append(
				$.new("script")
					.prop("type", "text/javascript")
					.prop("src", "modules/" + name + "/" + name + ".js")
			);

			if (css)
				document.head.append(
					$.new("link")
						.prop("rel", "stylesheet")
						.prop("type", "text/css")
						.prop("href", "modules/" + name + "/" + name + ".css")
				);
		},
		register: function (name, module) {
			if (!(name in this.list)) throw "Tried to register module not loaded";
			this.list[name] = module;

			if (this.initialized) module.init();
			return module;
		},
		init: function () {
			for (let i in this.list) {
				if (this.list[i] !== null)
					this.list[i].init();
			}
			this.initialized = true;
		}
	};

	modules.load("window", false);
	modules.load("expedition", true);
	modules.load("top-resource", true);

	document.event("DOMContentLoaded", async function () {
		window.CALLBACK = {};
		await CefSharp.BindObjectAsync({ IgnoreCache: true }, "API");

		window.modules.areas = {
			top: $("#top-module-area")
		};
		window.modules.init();
	});
}();