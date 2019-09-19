import Vue from "vue";
import Vuex from "vuex";
import App from "@/Frontend/App.vue";

import Proxy from "@/Proxy/Proxy";
import KanColleStore from "@KC/KanColleStore";

import FloatComponent from "@Components/Basic/FloatComponent.vue";
import GroupComponent from "@Components/Basic/GroupComponent.vue";

// Vue.config.productionTip = false;

// Global components
Vue.component("float", FloatComponent);
Vue.component("group", GroupComponent);

Vue.use(Vuex);

// Initialize Proxy and Store
Proxy.Instance.Empty();

new Vue({
	render: (h) => h(App),
	store: new Vuex.Store({
		state: {},
		modules: {
			KanColleStore
		}
	})
}).$mount("#app");
