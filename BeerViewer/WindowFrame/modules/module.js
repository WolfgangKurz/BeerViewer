"use strict";
!function () {
	window.mainBox = {};

	Vue.directive("dom", function (el, binding) {
		if (binding.oldValue && binding.oldValue instanceof Element && binding.oldValue.parentNode === el)
			binding.oldValue.remove();

		if (binding.value instanceof Element)
			el.append(binding.value);
	});

	window.modules = (function () {
		let initialized = false;
		const list = {};
		const _Areas = {};
		const _Tools = {};

		const areasObject = (function () {
			let top = null, side = null, sub = null;
			const iconlist = ["", "game", "plugin", "devtool"];

			return {
				$_Areas: _Areas,
				$_Tools: _Tools,

				init: function () {
					["Top", "Main", "Side", "Sub"].forEach(name => {
						const id = name.toLowerCase();
						_Areas[id] = {
							Name: name,
							Type: id,
							Modules: [],
							Element: $(`#${id}-module-area`)
						};
					});

					// Main browser
					this.register("main", "game", "Game", "game", $("#MAIN_FRAME"));

					// Devtools
					_Tools.devtools = {
						Id: "devtools",
						Name: "DevTools",
						Action: function () {
							window.API.DevTools();
						}
					};
				},

				register: function (area, id, name, icon, rootElement) {
					if (!(area in _Areas)) throw `Area '${area}' not supported`;
					const areaElem = _Areas[area];

					if (rootElement instanceof Vue)
						rootElement = rootElement.$el;

					if (areaElem.Modules.filter(x => x.Id === id).length > 0) throw "Already registered name";

					areaElem.Modules.push({
						Area: area,
						Id: id,
						Name: name,
						Icon: iconlist.indexOf(icon) >= 0 ? icon : "unknown",
						Displaying: areaElem.Modules.length === 0 || area === "top",
						Element: rootElement
					});
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
					Script: script ? `modules/${name}/${name}.js` : null,
					Style: css ? `modules/${name}/${name}.css` : null
				};
				const module = list[name];
				const el_module = $.new("div").attr("data-module", module.Name);

				const el_template = $.new("div");
				el_module.append(el_template);
				el_template.outerhtml(module.Template);

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

				$("#modules").append(el_module);
			},
			register: function (name, module) {
				if (!(name in list)) throw `Tried to register '${name}' module, but not loaded`;
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
		mainBox = new Vue({
			data: {
				Areas: window.modules.areas.$_Areas,
				Tools: window.modules.areas.$_Tools
			},
			el: $("#mainbox"),
			methods: {
				SelectModule: function (Area, Name) {
					if (!(Area in this.Areas))
						throw `Area '${Area}' not found, something wrong`;

					const area = this.Areas[Area];
					area.Modules.forEach(x => x.Displaying = x.Name === Name);
				}
			}
		});

		if (window.CefSharp) await CefSharp.BindObjectAsync({ IgnoreCache: true }, "API");
		if (typeof window.API === "undefined") {
			// Design mode
			window.modules.load("window", "", true, true);
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