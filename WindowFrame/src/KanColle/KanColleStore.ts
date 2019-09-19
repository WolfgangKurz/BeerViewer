import Master from "./Master";
import Materials from "./Classes/Materials";

import { Module, VuexModule, Mutation, Action } from "vuex-module-decorators";

/**
 * Store of KanColleStore
 */
@Module({})
export default class KanColleStore extends VuexModule {
	protected data = {
		Materials: {
			fuel: 1,
			ammo: 2,
			steel: 3,
			bauxite: 4,
			bucket: 5,
			construction: 6,
			development: 7,
			improve: 8
		}
	};

	protected inMaster: Master = new Master();
	protected inMaterials: Materials = new Materials();

	public get Master() {
		return this.inMaster;
	}

	public get Materials() {
		return this.data.Materials;
	}

	@Mutation
	public UpdateMaterials(payload: { fuel: number, ammo: number, steel: number, bauxite: number }) {
		this.data.Materials.fuel = payload.fuel;
		this.data.Materials.ammo = payload.ammo;
		this.data.Materials.steel = payload.steel;
		this.data.Materials.bauxite = payload.bauxite;
	}

	@Mutation
	public UpdateResources(payload: { bucket: number, construction: number, development: number, improve: number }) {
		this.data.Materials.bucket = payload.bucket;
		this.data.Materials.construction = payload.construction;
		this.data.Materials.development = payload.development;
		this.data.Materials.improve = payload.improve;
	}
}
