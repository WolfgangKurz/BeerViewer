import tippy from "tippy.js";
import Vue from "vue";
import VueTippy	from "./vendor/vue.tippy.js";

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
