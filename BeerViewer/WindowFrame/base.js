"use strict";
!function () {
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

	// Tippy extension for Vue
	!function () {
		const TIPPY = function(el, binding, vnode) {
			const handlers = (vnode.data && vnode.data.on)
				|| (vnode.componentOptions && vnode.componentOptions.listeners);

			let opts = binding.value || {};
			opts = Object.assign({ reactive: false, showOnLoad: false }, opts);

			if (handlers && handlers['show']) opts.onShow = () => handlers['show'].fns(el, vnode);
			if (handlers && handlers['shown']) opts.onShown = () => handlers['shown'].fns(el, vnode);
			if (handlers && handlers['hidden']) opts.onHidden = () => handlers['hidden'].fns(el, vnode);
			if (handlers && handlers['hide']) opts.onHide = () => handlers['hide'].fns(el, vnode);

			if (opts.html) {
				const selector = opts.html;

				if (opts.reactive || !(typeof selector === 'string')) {
					opts.html = selector instanceof Element
						? selector
						: selector instanceof Vue
							? selector.$el
							: document.querySelector(selector);
				} else {
					const htmlElement = document.querySelector(opts.html);
					if (htmlElement) {
						if (htmlElement._tipppyReferences)
							htmlElement._tipppyReferences.push(el);
						else
							htmlElement._tipppyReferences = [el];
					} else {
						console.error(`[TippyExt] Selector '${opts.html}' not found`);
						return;
					}
				}
			}

			tippy(el, (x => {
				let o = Object.assign({}, tippy.defaults);
				for (let i in x)
					if (i in tippy.defaults)
						o[i] = x[i];
				return o;
			})(opts));

			if (opts.showOnLoad) el._tippy.show();

			Vue.nextTick(() => {
				if (handlers && handlers['init'])
					handlers['init'].fns(el._tippy, el);
			});
		};

		Vue.directive("tippy-target", { // Tippy content target
			componentUpdated: function (el) {
				const refs = el._tipppyReferences;
				if (refs && refs.length > 0) {
					Vue.nextTick(() => {
						refs.filter(x => x._tippy).forEach(ref => {
							if (ref._tippy) {
								const content = ref._tippy.popper.querySelector('.tippy-content');
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
						else if (!opts.show && opts.trigger === 'manual')
							el._tippy.hide();
					}
				}
			}
		});
	}();
}();