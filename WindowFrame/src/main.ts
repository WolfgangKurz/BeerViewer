import Vue from "vue";
import Vuex from "vuex";
import App from "@/Frontend/App.vue";

import Proxy from "@/Proxy/Proxy";

import FloatComponent from "@Components/Basic/FloatComponent.vue";
import GroupComponent from "@Components/Basic/GroupComponent.vue";
import ConvertStore from "./KanColle/Store/StoreBase";
import KanColleStore, { InitializeStore } from "./KanColle/Store/KanColleStore";

// Vue.config.productionTip = false;

// Global components
Vue.component("float", FloatComponent);
Vue.component("group", GroupComponent);

Vue.use(Vuex);

// Initialize Proxy
Proxy.Instance.Empty();

Vue.use({
	install(_, options) {
		_.prototype.$store = ConvertStore(new KanColleStore());
	}
});

new Vue({
	render: (h) => h(App)
}).$mount("#app");
InitializeStore();
