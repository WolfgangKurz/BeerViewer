<template>
	<div :data-float="float">
		<slot></slot>
	</div>
</template>

<script lang="ts">
import { Vue, Component, Prop, PropSync } from "vue-property-decorator";

@Component({})
export default class FloatComponent extends Vue {
	/** float to left */
	@Prop({
		type: Boolean,
		default: false,
	})
	public left!: boolean;

	/** float to right */
	@Prop({
		type: Boolean,
		default: false,
	})
	public right!: boolean;

	/** result float position */
	private get float() {
		if (this.left) return "left";
		else if (this.right) return "right";
		return "none";
	}
}
</script>

<style lang="scss">
[data-float="left"] {
	float: left;
}
[data-float="right"] {
	float: right;
}
[data-float="none"] {
	float: none;
}
</style>
