import { MutationMethod } from "./StoreBase";
import { StoreInterface } from "./KanColleStoreClient";

import Master from "@KC/Master";
import Materials from "@KC/Classes/Materials";

let inMaster: Master;
let inMaterials: Materials;

/**
 * Initialize store managers
 */
export function InitializeStore() {
	inMaster = new Master();
	inMaterials = new Materials();
}

/**
 * Store for KanColle data.
 *
 * Check `KanColleStoreClient` to read JSDoc.
 */
export default class KanColleStore {
	public StoreMaterials: StoreInterface.Materials = {
		Fuel: 0,
		Ammo: 0,
		Steel: 0,
		Bauxite: 0,
		Bucket: 0,
		Construction: 0,
		Development: 0,
		Improvement: 0
	};

	@MutationMethod()
	public StoreUpdateMaterials(payload: StoreInterface.MaterialsPayload) {
		if (payload.Fuel)
			this.StoreMaterials.Fuel = payload.Fuel;
		if (payload.Ammo)
			this.StoreMaterials.Ammo = payload.Ammo;
		if (payload.Steel)
			this.StoreMaterials.Steel = payload.Steel;
		if (payload.Bauxite)
			this.StoreMaterials.Bauxite = payload.Bauxite;
		if (payload.Bucket)
			this.StoreMaterials.Bucket = payload.Bucket;
		if (payload.Construction)
			this.StoreMaterials.Construction = payload.Construction;
		if (payload.Development)
			this.StoreMaterials.Development = payload.Development;
		if (payload.Improvement)
			this.StoreMaterials.Improvement = payload.Improvement;
	}
}
