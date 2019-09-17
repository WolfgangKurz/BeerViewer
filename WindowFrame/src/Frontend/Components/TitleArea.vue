<template>
	<header class="title-area">
		<!-- Left area -->
		<div class="menubox">
			<i>{{title}}</i>
		</div>

		<!-- Right area -->
		<systembox></systembox>

		<!-- Remain area -->
		<div class="content">
			<expedition-bar></expedition-bar>
			<expedition-bar></expedition-bar>
			<expedition-bar></expedition-bar>
		</div>
	</header>
</template>

<script lang="ts">
import { Vue, Component, Prop, PropSync } from "vue-property-decorator";

import Systembox from "@Components/Systembox.vue";
import ExpeditionBar from "@Components/Expedition/ExpeditionBar.vue";

@Component({
	components: {
		Systembox,
		ExpeditionBar
	}
})
export default class TitleArea extends Vue {
	@Prop({
		type: String,
		required: true
	})
	public title!: string;
}
</script>

<style lang="scss">
.title-area {
	grid-area: 1 / 1 / 2 / 4; // x: 1~4, y: 1~2

	@include clearfix;
	$border-sizer-size: 5px;

	height: $theme-title-size;
	line-height: $theme-title-size;
	overflow: hidden;

	user-select: none;
	-webkit-app-region: drag;

	.menubox {
		float: left;
		padding: 0 10px;

		> i {
			font-weight: bold;
			font-style: normal;
			text-transform: uppercase;
			letter-spacing: 1px;
		}
	}
	.content {
		margin-top: $border-sizer-size;
		padding: 0 10px;
		height: $theme-title-size - $border-sizer-size;
		line-height: ($theme-title-size - $border-sizer-size * 2);
		overflow: hidden;
	}
}
</style>
