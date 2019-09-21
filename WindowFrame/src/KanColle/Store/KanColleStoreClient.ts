// tslint:disable:no-namespace

import Vue from "vue";

import { Mutation, State } from "vuex-class";

/**
 * Namespace that contains interfaces of `KanColleStore`.
 */
export namespace StoreInterface {
	/** Material data interface */
	export interface Materials {
		Fuel: number;
		Ammo: number;
		Steel: number;
		Bauxite: number;
		Bucket: number;
		Construction: number;
		Development: number;
		Improvement: number;
	}

	/** Material data interface for payload */
	export interface MaterialsPayload {
		Fuel?: number;
		Ammo?: number;
		Steel?: number;
		Bauxite?: number;
		Bucket?: number;
		Construction?: number;
		Development?: number;
		Improvement?: number;
	}
}

/**
 * KanColleStore implementation class for Component/Pages
 */
export default class KanColleStoreClient extends Vue {
	/**
	 * Material data of `KanColleStore`.
	 */
	@State
	public StoreMaterials!: StoreInterface.Materials;

	/**
	 * Update new materials data to store.
	 */
	@Mutation("StoreUpdateMaterials")
	public StoreUpdateMaterials!: (payload: StoreInterface.MaterialsPayload) => void;
}
