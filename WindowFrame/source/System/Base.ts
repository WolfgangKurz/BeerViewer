import tippy from "tippy.js";
import Vue from "vue";
import Vuex, { mapState } from "vuex";
import VueTippy from "vendor/vue.tippy";
import Modules, { Callback } from "./Module";
import { Homeport } from "./Homeport/Homeport";
import { Master } from "./Master/Master";
import BaseAPI from "./Base/API"
import Settings, { Settings as _Settings } from "System/Settings";
import { fns } from "./Base/Base";
import { ZoomLevels } from "./Const/ZoomLevels";

Vue.use(Vuex);

tippy.setDefaults({
	arrow: true,
	boundary: "viewport",
	placement: "bottom",
	delay: [40, 100],
	size: "large",
	theme: "light-border",
	interactive: false,
	trigger: 'mouseenter focus',
	hideOnClick: false
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

document.addEventListener("DOMContentLoaded", async () => {
	console.log("DOMContentLoaded");
	if (window.modules.initialized()) return; // Called twice

	console.log("DOMContentLoaded2");

	window.modules.areas.init();
	console.log("Areas init");
	const mainBox = new Vue({
		data: {
			Areas: window.modules.areas.Areas,
			Tools: window.modules.areas.Tools,

			Frame: {
				Width: 1200,
				Height: 720
			},
			Zoom: {
				Index: ZoomLevels.indexOf(1.0),
				Factor: 1.0,
				Percentage: "100.0%"
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
			},
			ZoomFrame(index: number): void {
				if (index === -1)
					this.Zoom.Index--;
				else if (index === -2)
					this.Zoom.Index++;
				else
					this.Zoom.Index = index;

				if (this.Zoom.Index < 0) this.Zoom.Index = 0;
				if (this.Zoom.Index >= this.ZoomLevels.length) this.Zoom.Index = this.ZoomLevels.length - 1;

				this.Zoom.Factor = this.ZoomLevels[this.Zoom.Index];
				this.Zoom.Percentage = Math.floor(this.Zoom.Factor * 1000) / 10 + "%";

				Settings.MainFrame.ZoomFactor.Value = this.Zoom.Factor;

				if (window.Game)
					window.Game.zoom(this.Zoom.Factor * 100);
				else { // Lazyload...
					const timer = setInterval(() => {
						if (window.Game) {
							window.Game.zoom(this.Zoom.Factor * 100);
							clearInterval(timer);
						}
					}, 500);
				}
			}
		},
		computed: {
			ZoomLevels() { return ZoomLevels },
			...mapState({
				i18n: "i18n"
			})
		}
	});
	console.log("Vue instance initialized");
	window.CALLBACK.register("Game.Zoom", (factor: number | string) => {
		const v = typeof factor === "number"
			? factor / 100
			: parseFloat(factor) / 100;

		mainBox.Frame.Width = Math.round(1200 * v);
		mainBox.Frame.Height = Math.round(720 * v);
		mainBox.Zoom.Factor = v;
		mainBox.Zoom.Percentage = Math.floor(v * 1000) / 10 + "%";
		
		window.API.NotifyMainFrameResized(mainBox.Frame.Width, mainBox.Frame.Height);
	});

	console.log("window.CefSharp");

	if ((<any>window).CefSharp)
		await (<any>window).CefSharp.BindObjectAsync({ IgnoreCache: true }, "API");

	console.log("BindObjectAsync");

	if (typeof window.API === "undefined") {
		// Browsed with not-allowed method
		window.modules.init();
		return;
	}
	window.INTERNAL.Initialized();
	console.log("Internal initialized");

	_Settings.Instance.Initialize(); // Initialize
	window.Master = new Master().Ready();
	window.Homeport = new Homeport().Ready();
	console.log("Settings Master Homport ready");

	_Settings.Instance.Ready(() => {
		const value = <number>Settings.MainFrame.ZoomFactor.Value;
		const index = ZoomLevels.findIndex(v => {
			return v.toFixed(4) === value.toFixed(4);
		});
		if (index < 0) return;
		mainBox.ZoomFrame(index);
	});

	console.log("Before get module list");
	window.API.GetModuleList()
		.then(async list => {
			console.log("Module list arrived");
			list.forEach(x => window.modules.load(x.Name, x.RawName, x.Template, x.Scripted, x.Styled));

			window.modules.init();
			window.API.Initialized();
		});
});