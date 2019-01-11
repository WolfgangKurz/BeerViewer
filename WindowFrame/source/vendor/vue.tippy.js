import Vue from "vue";
import tippy from "tippy.js";
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
                : selector instanceof Vue
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
        let opt = Object.assign({}, tippy.defaults, opts);
        for (let i in opt)
            if (!(i in tippy.defaults))
                delete opt[i];
        tippy(el, opt);
    }
    if (opts.showOnLoad)
        el._tippy.show();
    Vue.nextTick(() => {
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
export default VueTippy;
//# sourceMappingURL=vue.tippy.js.map