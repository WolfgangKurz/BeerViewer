import tippy from "tippy.js";
import Vue from "vue";
import Vuex, { mapState } from "vuex";
import VueTippy from "vendor/vue.tippy";
import Modules, { Callback } from "./Module";
import { Homeport } from "./Homeport/Homeport";
import { Master } from "./Master/Master";
import BaseAPI from "./Base/API"
import { Settings } from "./Settings";
import { fns } from "./Base/Base";

Vue.use(Vuex);

tippy.setDefaults({
	arrow: true,
	boundary: "viewport",
	placement: "bottom",
	size: "large",
	theme: "light-border",
	interactive: false,
	trigger: 'mouseenter focus',
	hideOnClick: false,
	performance: false
});

Vue.directive("dom", function (el, binding) {
	if (binding.value === binding.oldValue) return;

	if (binding.oldValue && binding.oldValue instanceof Element && binding.oldValue.parentNode === el) {
		$(binding.oldValue).appendTo($("#modules"));
	}

	if (binding.value instanceof Element)
		$(el).append(binding.value);
});

Vue.use(VueTippy, tippy.defaults);

window.modules = Modules.Instance;
window.CALLBACK = Callback.Instance;

window.BaseAPI = new BaseAPI(); // Initialize BaseAPI
const vueStore = new Vuex.Store({
	state: {
		i18n: window.i18n,
		events: <{ [name: string]: Function[] }>{}
	},
	mutations: {
		i18n(state, n) {
			state.i18n = n;
		},

		e_register(state, data: { name: string, callback: Function }) {
			const name = data.name, callback = data.callback;
			if (!(name in state.events))
				state.events[name] = [];

			state.events[name].push(callback);
		},
		e_raise(state, data: { name: string, args: any[] }) {
			const name = data.name, args = data.args;
			fns(state.events[name], args);
		}
	}
});
window.BaseAPI.Event("i18n", v => vueStore.commit("i18n", window.i18n));

document.addEventListener("DOMContentLoaded", async function () {
	if (window.modules.initialized()) return; // Called twice

	window.modules.areas.init();
	const mainBox = new Vue({
		data: {
			Areas: window.modules.areas.Areas,
			Tools: window.modules.areas.Tools,

			Frame: {
				Width: 1200,
				Height: 720
			}
		},
		el: $("#mainbox")[0],
		store: vueStore,
		methods: {
			OpenMenu: (x: boolean) => window.OpenMenu(x),
			SelectModule: function (Area: string, Name: string) {
				if (!(Area in this.Areas))
					throw `Area '${Area}' not found, something wrong`;

				const area = this.Areas[Area];
				area.Modules.forEach(x => x.Displaying = x.Name === Name);
			}
		},
		computed: mapState({
			i18n: "i18n"
		})
	});
	window.CALLBACK.register("Game.Zoom", (factor: number | string) => {
		const v = typeof factor === "number"
			? factor / 100
			: parseFloat(factor) / 100;

		mainBox.Frame.Width = 1200 * v;
		mainBox.Frame.Height = 720 * v;
	});

	if ((<any>window).CefSharp)
		await (<any>window).CefSharp.BindObjectAsync({ IgnoreCache: true }, "API");

	if (typeof window.API === "undefined") {
		// Browsed with not-allowed method
		window.modules.init();
		return;
	}
	window.INTERNAL.Initialized();

	window.Settings = new Settings().Ready();
	window.Master = new Master().Ready();
	window.Homeport = new Homeport().Ready();

	window.API.GetModuleList()
		.then(async list => {
			list.forEach(x => window.modules.load(x.Name, x.RawName, x.Template, x.Scripted, x.Styled));

			window.modules.init();
			window.API.Initialized();
		});
});