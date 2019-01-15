import tippy from "tippy.js";
import Vue from "vue";
import VueTippy from "../vendor/vue.tippy";
import Modules, { Callback } from "./Module";
import { Homeport } from "./Homeport/Homeport";
import { Master } from "./Master/Master";

tippy.setDefaults({
    arrow: true,
    boundary: "viewport",
    placement: "bottom",
    size: "large",
    theme: "light-border",
    interactive: false,
    trigger: 'mouseenter focus',
    hideOnClick: false,
    performance: true
});

Vue.directive("dom", function (el, binding) {
    if (binding.oldValue && binding.oldValue instanceof Element && binding.oldValue.parentNode === el)
        binding.oldValue.remove();

    if (binding.value instanceof Element)
        el.append(binding.value);
});

Vue.use(VueTippy, tippy.defaults);

window.modules = Modules.Instance;
window.CALLBACK = Callback.Instance;

document.addEventListener("DOMContentLoaded", async function () {
    if (window.modules.initialized()) return; // Called twice

    window.modules.areas.init();
    const _mainbox_elem = $("#mainbox")[0];
    const mainBox = new Vue({
        data: {
            Areas: window.modules.areas.Areas,
            Tools: window.modules.areas.Tools
        },
        el: _mainbox_elem ? _mainbox_elem : undefined,
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

    if ((<any>window).CefSharp)
        await (<any>window).CefSharp.BindObjectAsync({ IgnoreCache: true }, "API");

    if (typeof window.API === "undefined") {
        // Browsed with not-allowed method
        window.modules.registerDefault();
        window.modules.init();
        return;
    }
    window.INTERNAL.Initialized();

    window.API.GetModuleList()
        .then(list => {
            list.forEach(x => window.modules.load(x.Name, x.Template, x.Scripted, x.Styled));

            window.modules.registerDefault();
            window.modules.init();
            window.API.Initialized();
        });

    Master.Instance.Ready();
    Homeport.Instance.Ready();
});