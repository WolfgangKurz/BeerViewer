import { DirectiveBinding } from "vue/types/options";
import Vue, { VNode, PluginObject } from "vue";
import tippy, { Props, Tippy } from "tippy.js";
import Popper from "popper.js";

interface VueTippyHTMLElement extends HTMLElement {
	_tippyReferences: HTMLElement[];
	_tippy: {
		popperInstance: Popper
		popper: HTMLElement;
		show(): void;
		hide(): void;
		destroy(): void;
	};
}

const TIPPY = function (el: VueTippyHTMLElement, binding: DirectiveBinding, vnode: VNode) {
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
			opts.content = opts.html.innerHTML;
		} else {
			const elem = document.querySelector<VueTippyHTMLElement>(opts.html);
			if (elem) {
				if (elem._tippyReferences)
					elem._tippyReferences.push(el);
				else
					elem._tippyReferences = [el];
			} else {
				console.error(`[TippyExt] Selector "${opts.html}" not found`);
				return;
			}
			opts.content = elem.innerHTML;
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
			componentUpdated: function (_el: HTMLElement) {
				const el = <VueTippyHTMLElement>_el;
				const refs = el._tippyReferences;
				if (refs && refs.length > 0) {
					Vue.nextTick(() => {
						refs.filter((x: HTMLElement) => (<VueTippyHTMLElement>x)._tippy)
							.forEach((_ref: HTMLElement) => {
								const ref = <VueTippyHTMLElement>_ref;
								const content = ref._tippy.popper.querySelector(".tippy-content");
								if (content) content.innerHTML = el.innerHTML;
							});
					});
				}
			},
			unbind: function (_el: HTMLElement) {
				const el = <VueTippyHTMLElement>_el;
				delete el._tippyReferences;
			}
		});
		Vue.directive("tippy", {
			inserted: function (_el, binding, vnode) {
				const el = <VueTippyHTMLElement>_el;
				Vue.nextTick(() => TIPPY(el, binding, vnode));
			},
			unbind: function (_el: HTMLElement) {
				const el = <VueTippyHTMLElement>_el;
				if (el._tippy)
					el._tippy.destroy();
			},
			componentUpdated: function (_el: HTMLElement, binding, vnode) {
				const el = <VueTippyHTMLElement>_el;
				const ts = JSON.stringify;
				const opts = binding.value || {};
				const oldOpts = binding.oldValue || {};

				if (el._tippy) {
					if (ts(opts) !== ts(oldOpts))
						Vue.nextTick(() => TIPPY(el, binding, vnode));

					if (el._tippy.popperInstance) {
						if (opts.show)
							el._tippy.show();
						else if (opts.trigger === "manual")
							el._tippy.hide();
					}
				}
			}
		});
	}
};
export default VueTippy;
