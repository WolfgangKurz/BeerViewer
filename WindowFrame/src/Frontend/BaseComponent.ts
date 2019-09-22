import Vue from "vue";
import { Component } from "vue-property-decorator";
import i18n from "@/i18n/i18n";

@Component
export default class BaseComponent extends Vue {
	/**
	 * Gets translated text.
	 *
	 * If translation not exists, returns `key`.
	 * @param key Key of translation.
	 */
	protected i18n(key: string): string {
		return i18n.get(key) || key;
	}
}
