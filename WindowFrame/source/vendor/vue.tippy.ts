import { DirectiveBinding } from "vue/types/options";
import Vue, { VNode, PluginObject } from "vue";
import tippy, { Props } from "tippy.js";

type AnyElement = HTMLElement | any;

const TIPPY = function (el: AnyElement, binding: DirectiveBinding, vnode: VNode) {
	const handlers: any = (vnode.data && vnode.data.on) || (vnode.componentOptions && vnode.componentOptions.listeners);
	const opts = Object.assign({ reactive: false, showOnLoad: false }, binding.value || {});

	if (handlers && handlers["show"]) opts.onShow = () => handlers["show"].fns(el, vnode);
	if (handlers && handlers["shown"]) opts.onShown = () => handlers["shown"].fns(el, vnode);
	if (handlers && handlers["hidden"]) opts.onHidden = () => handlers["hidden"].fns(el, vnode);
	if (handlers && handlers["hide"]) opts.onHide = () => handlers["hide"].fns(el, vnode);

	if (opts.html) {
		const selector = opts.html;

		if (opts.reactive || !(typeof selector === "string")) {
			opts.html = selector instanceof Element
				? selector
				: selector instanceof Vue
					? selector.$el
					: document.querySelector(selector);
		} else {
			const elem = document.querySelector<AnyElement>(opts.html);
			if (elem) {
				if (elem._tipppyReferences)
					elem._tipppyReferences.push(el);
				else
					elem._tipppyReferences = [el];
			} else {
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

	if (opts.showOnLoad) el._tippy.show();

	Vue.nextTick(() => {
		if (handlers && handlers["init"])
			handlers["init"].fns(el._tippy, el);
	});
};

const VueTippy: PluginObject<Props> = {
	install(Vue, options) {
		Vue.directive("tippy-target", { // Tippy content target
			componentUpdated: function (el: AnyElement) {
				const refs = el._tipppyReferences;
				if (refs && refs.length > 0) {
					Vue.nextTick(() => {
						refs.filter((x: AnyElement) => x._tippy)
							.forEach((ref: AnyElement) => {
								if (ref._tippy) {
									const content = ref._tippy.popper.querySelector(".tippy-content");
									content.innerHTML = el.innerHTML;
								}
							});
					});
				}
			},
			unbind: function (el: AnyElement) {
				delete el._tipppyReference;
			}
		});
		Vue.directive("tippy", {
			inserted: function (el, binding, vnode) {
				Vue.nextTick(() => TIPPY(el, binding, vnode));
			},
			unbind: function (el: AnyElement) {
				if (el._tippy)
					el._tippy.destroy();
			},
			componentUpdated: function (el: AnyElement, binding, vnode) {
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
