<template>
	<div>
		<div v-for="fleet in Fleets" :key="`FleetView-Fleet-${fleet.Id}`">
			{{fleet.Id}} fleet
			<div v-for="ship in fleet.Ships" :key="`FleetView-Fleet-${fleet.Id}-${ship}`">{{ship}}</div>
			<hr />
		</div>
	</div>
</template>

<script lang="ts">
import { Vue, Component, Prop, PropSync } from "vue-property-decorator";
import KanColleStoreClient from "@KC/Store/KanColleStoreClient";

@Component({})
export default class FleetView extends KanColleStoreClient {
	public get Fleets() {
		return this.StoreFleets;
	}
}
</script>

<style lang="scss">
</style>
