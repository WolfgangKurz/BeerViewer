"use strict";
!function () {
	window.modules = (function () {
		let initialized = false;
		const list = {};

		return {
			areas: [],

			initialized: function () {
				return initialized;
			},

			load: function (name, css) {
				if (name in list) throw "Tried to load module already loaded";
				list[name] = null;

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
				if (!(name in list)) throw "Tried to register module not loaded";
				list[name] = module;

				if (initialized) module.init();
				return module;
			},
			init: function () {
				for (let i in list) {
					if (list[i] !== null)
						list[i].init();
				}
				initialized = true;
			},

			get: function (name) {
				if (list[name] === null) return null;
				return list[name];
			}
		};
	})();
	window.CALLBACK = (function () {
		const callbacks = {};
		return {
			register: function (name, callback) {
				if (!(name in callbacks))
					callbacks[name] = [];

				callbacks[name].push(callback);
			},
			unregister: function (name, callback) {
				if (!(name in callbacks)) return;

				const index = callbacks[name].indexOf(callback);
				if (index >= 0)
					callbacks[name].splice(index, 1);
			},

			call: function (name) {
				if (!(name in callbacks)) return false;

				const list = callbacks[name];
				for (let i = 0; i < list.length; i++) {
					list[i].apply(
						null,
						Array.prototype.slice.call(arguments, 1)
					);
				}
			}
		};
	})();

	document.event("DOMContentLoaded", async function () {
		if (window.modules.initialized()) return; // Call twice

		await CefSharp.BindObjectAsync({ IgnoreCache: true }, "API");

		window.modules.areas = {
			top: $("#top-module-area"),
			side: $("#side-module-area"),
			sub: $("#sub-module-area")
		};

		!function (list) {
			list.forEach(function (x) {
				window.modules.load(x.Name, x.Styled);
			});

			window.modules.init();
			window.API.Initialized();
		}(await window.API.GetModuleList());
	});
}();