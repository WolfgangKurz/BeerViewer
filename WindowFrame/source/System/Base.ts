import tippy from "tippy.js";
import Vue from "vue";
import VueTippy from "vendor/vue.tippy";
import Modules, { Callback } from "./Module";
import { Homeport } from "./Homeport/Homeport";
import { Master } from "./Master/Master";
import BaseAPI from "./Base/API"
import { Settings } from "./Settings";

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

const baseAPI = new BaseAPI(); // Initialize BaseAPI

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
        methods: {
            OpenMenu: (x: boolean) => window.OpenMenu(x),
            SelectModule: function (Area: string, Name: string) {
                if (!(Area in this.Areas))
                    throw `Area '${Area}' not found, something wrong`;

                const area = this.Areas[Area];
                area.Modules.forEach(x => x.Displaying = x.Name === Name);
            }
        }
    });
    baseAPI.Event("zoomMainFrame", v =>{
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