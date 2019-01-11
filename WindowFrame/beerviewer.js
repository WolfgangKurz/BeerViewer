"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
define("vendor/vue.tippy", ["require", "exports", "vue", "tippy.js"], function (require, exports, vue_1, tippy_js_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    vue_1 = __importDefault(vue_1);
    tippy_js_1 = __importDefault(tippy_js_1);
    const TIPPY = function (el, binding, vnode) {
        const handlers = (vnode.data && vnode.data.on) || (vnode.componentOptions && vnode.componentOptions.listeners);
        const opts = Object.assign({ reactive: false, showOnLoad: false }, binding.value || {});
        if (handlers && handlers["show"])
            opts.onShow = () => handlers["show"].fns(el, vnode);
        if (handlers && handlers["shown"])
            opts.onShown = () => handlers["shown"].fns(el, vnode);
        if (handlers && handlers["hidden"])
            opts.onHidden = () => handlers["hidden"].fns(el, vnode);
        if (handlers && handlers["hide"])
            opts.onHide = () => handlers["hide"].fns(el, vnode);
        if (opts.html) {
            const selector = opts.html;
            if (opts.reactive || !(typeof selector === "string")) {
                opts.html = selector instanceof Element
                    ? selector
                    : selector instanceof vue_1.default
                        ? selector.$el
                        : document.querySelector(selector);
            }
            else {
                const elem = document.querySelector(opts.html);
                if (elem) {
                    if (elem._tipppyReferences)
                        elem._tipppyReferences.push(el);
                    else
                        elem._tipppyReferences = [el];
                }
                else {
                    console.error(`[TippyExt] Selector "${opts.html}" not found`);
                    return;
                }
            }
        }
        {
            let opt = Object.assign({}, tippy_js_1.default.defaults, opts);
            for (let i in opt)
                if (!(i in tippy_js_1.default.defaults))
                    delete opt[i];
            tippy_js_1.default(el, opt);
        }
        if (opts.showOnLoad)
            el._tippy.show();
        vue_1.default.nextTick(() => {
            if (handlers && handlers["init"])
                handlers["init"].fns(el._tippy, el);
        });
    };
    const VueTippy = {
        install(Vue, options) {
            Vue.directive("tippy-target", {
                componentUpdated: function (el) {
                    const refs = el._tipppyReferences;
                    if (refs && refs.length > 0) {
                        Vue.nextTick(() => {
                            refs.filter((x) => x._tippy)
                                .forEach((ref) => {
                                if (ref._tippy) {
                                    const content = ref._tippy.popper.querySelector(".tippy-content");
                                    content.innerHTML = el.innerHTML;
                                }
                            });
                        });
                    }
                },
                unbind: function (el) {
                    delete el._tipppyReference;
                }
            });
            Vue.directive("tippy", {
                inserted: function (el, binding, vnode) {
                    Vue.nextTick(() => TIPPY(el, binding, vnode));
                },
                unbind: function (el) {
                    if (el._tippy)
                        el._tippy.destroy();
                },
                componentUpdated: function (el, binding, vnode) {
                    const ts = JSON.stringify;
                    const opts = binding.value || {};
                    const oldOpts = binding.oldValue || {};
                    if (el._tippy) {
                        if (ts(opts) !== ts(oldOpts))
                            Vue.nextTick(() => TIPPY(el, binding, vnode));
                        if (el._tippy.popperInstance) {
                            if (opts.show)
                                el._tippy.show();
                            else if (!opts.show && opts.trigger === "manual")
                                el._tippy.hide();
                        }
                    }
                }
            });
        }
    };
    exports.default = VueTippy;
});
define("base", ["require", "exports", "tippy.js", "vue", "vendor/vue.tippy"], function (require, exports, tippy_js_2, vue_2, vue_tippy_js_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    tippy_js_2 = __importDefault(tippy_js_2);
    vue_2 = __importDefault(vue_2);
    vue_tippy_js_1 = __importDefault(vue_tippy_js_1);
    tippy_js_2.default.setDefaults({
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
    vue_2.default.directive("dom", function (el, binding) {
        if (binding.oldValue && binding.oldValue instanceof Element && binding.oldValue.parentNode === el)
            binding.oldValue.remove();
        if (binding.value instanceof Element)
            el.append(binding.value);
    });
    vue_2.default.use(vue_tippy_js_1.default, tippy_js_2.default.defaults);
});
class Admiral {
    constructor() {
    }
}
class Homeport {
    constructor() {
        this.Admiral = new Admiral();
    }
}
//# sourceMappingURL=beerviewer.js.map