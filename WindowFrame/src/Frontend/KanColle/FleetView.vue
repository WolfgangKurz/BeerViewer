<template>
	<div class="fleet-view">
		<tab-host :tabs="TabList"></tab-host>
		<div v-for="fleet in Fleets" :key="`FleetView-Fleet-${fleet.Id}`">
			{{fleet.Id}} fleet
			<div v-for="(ship, idx) in fleet.Ships" :key="`FleetView-Fleet-${fleet.Id}-${idx}`">
				<template v-if="ship !== -1">
					<div>Ship Id: {{ship}}</div>
					<div>Name: {{i18n(Ships[ship].Name)}}</div>
				</template>
			</div>
			<hr />
		</div>
	</div>
</template>

<script lang="ts">
import { Vue, Component, Prop, PropSync } from "vue-property-decorator";
import KCComponent from "@KCComponents/KCComponent";
import { IdMapArray } from "@KC/Basic/IdMap";

@Component({})
export default class FleetView extends KCComponent {
	public get Ships() {
		return this.StoreShips;
	}
	public get Fleets() {
		return this.StoreFleets;
	}

	public get TabList(): string[] {
		return IdMapArray(this.Fleets).map((x) => x.Id.toString());
	}
}
</script>

<style lang="scss">
</style>
