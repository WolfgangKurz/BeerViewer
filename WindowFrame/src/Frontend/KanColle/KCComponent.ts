import i18n from "@/i18n/i18n";
import { Component } from "vue-property-decorator";
import KanColleStoreClient from "@KC/Store/KanColleStoreClient";

@Component
export default class KCComponent extends KanColleStoreClient {
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
