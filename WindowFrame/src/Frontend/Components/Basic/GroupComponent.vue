<template>
	<div data-group :style="{ order: order }">
		<slot></slot>
	</div>
</template>

<script lang="ts">
import { Vue, Component, Prop, PropSync } from "vue-property-decorator";

@Component({})
export default class GroupComponent extends Vue {
	@Prop({
		type: Number,
		default: undefined
	})
	public order!: number | undefined;
}
</script>

<style lang="scss">
</style>
