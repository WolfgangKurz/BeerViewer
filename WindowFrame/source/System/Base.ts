import tippy from "tippy.js";
import Vue from "vue";
import Vuex, { mapState } from "vuex";
import VueTippy from "vendor/vue.tippy";
import Modules, { Callback } from "./Module";
import { Homeport } from "./Homeport/Homeport";
import { Master } from "./Master/Master";
import BaseAPI from "./Base/API"
import { Settings } from "./Settings";

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

Vue.component("game-component", {
	template: `<iframe src="MainFramePlaceholder.html" id="MAIN_FRAME" name="MAIN_FRAME"></iframe>`
});

Vue.use(VueTippy, tippy.defaults);

window.modules = Modules.Instance;
window.CALLBACK = Callback.Instance;

window.BaseAPI = new BaseAPI(); // Initialize BaseAPI
const vueStore = new Vuex.Store({
	state: {
		i18n: window.i18n
	},
	mutations: {
		i18n(state, n) {
			state.i18n = n;
		}
	}
});
window.BaseAPI.Event("i18n", v => vueStore.commit("i18n", window.i18n));
console.log(vueStore);

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
	window.BaseAPI.Event("zoomMainFrame", v => {
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
			list.forEach(x => window.modules.load(x.Name, x.Template, x.Scripted, x.Styled));

			window.modules.init();
			window.API.Initialized();
		});
});