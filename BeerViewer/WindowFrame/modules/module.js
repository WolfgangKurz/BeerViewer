"use strict";
!function () {
	window.modules = (function () {
		let initialized = false;
		const list = {};

		const areasObject = (function () {
			let top = null, side = null, sub = null;
			const iconlist = ["", "game", "plugin"];
			const areas = {};

			return {
				init: function () {
					areas.top = $("#top-module-area");
					areas.main = $("#sub-module-area");
					areas.side = $("#side-module-area");
					areas.sub = $("#sub-module-area");
				},

				register: function (area, name, icon, rootElement) {
					if (!(area in areas)) throw "Unknown module area `" + area + "`";

					const areaElem = areas[area];
					if (areaElem === null) throw "Application.html corrupted";

					if (rootElement instanceof Vue)
						rootElement = rootElement.$el;

					const escaped = name.replace(/"/g, "\\\"");
					if (areaElem.find("[data-module-name=\"" + escaped + "\"]") !== null)
						throw "Already registered name";

					areaElem.append(rootElement.attr("data-module-name", escaped));

					const areaMenu = $("ul.menu-host > li.menu-group[data-area=\"" + area + "\"]");
					if (areaMenu === null) return; // Menuless module (for example, top-area modules)

					areaMenu.find("ul").append(
						$.new("li")
							.append(
								$.new("i", "menu-icon").attr("data-icon", iconlist.indexOf(icon) >= 0 ? icon : "unknown")
							)
							.append(
								$.new.text(name)
							)
					);
				}
			};
		})();
		return {
			areas: areasObject,

			initialized: function () {
				return initialized;
			},

			load: function (name, template, script, css) {
				if (name in list) throw "Tried to load module already loaded";

				list[name] = {
					module: null,

					Name: name,
					Template: template,
					Script: script ? "modules/" + name + "/" + name + ".js" : null,
					Style: css ? "modules/" + name + "/" + name + ".css" : null
				};
				const module = list[name];

				const el_module = $.new("div").attr("data-module", module.Name);
				if (module.Script)
					el_module.append(
						$.new("script")
							.prop("type", "text/javascript")
							.prop("src", module.Script)
					);
				if (module.Style)
					el_module.append(
						$.new("link")
							.prop("rel", "stylesheet")
							.prop("type", "text/css")
							.prop("href", module.Style)
					);
				const el_template = $.new("div");
				el_module.append(el_template);
				el_template.outerhtml(module.Template);
				$("#modules").append(el_module);
			},
			register: function (name, module) {
				if (!(name in list)) throw "Tried to register module not loaded";
				list[name].module = module;

				if (initialized) list[name].module.init();
				return module;
			},
			init: function () {
				for (let i in list) {
					if (list[i].module !== null)
						list[i].module.init();
				}
				initialized = true;
			},

			get: function (name) {
				if (!(name in list)) return null;
				if (list[name].module === null) return null;
				return list[name].module;
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

		window.modules.areas.init();

		await CefSharp.BindObjectAsync({ IgnoreCache: true }, "API");
		if (typeof window.API === "undefined") {
			// Design mode
			window.modules.load("window", false);
			window.modules.init();
			return;
		}
		window.INTERNAL.Initialized();

		!function (list) {
			list.forEach(x => window.modules.load(x.Name, x.Template, x.Scripted, x.Styled));

			window.modules.init();
			window.API.Initialized();
		}(await window.API.GetModuleList());
	});
}();