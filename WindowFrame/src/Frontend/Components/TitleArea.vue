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
			<group :order="1">
				<expedition-bar :id="2"></expedition-bar>
				<expedition-bar :id="3"></expedition-bar>
				<expedition-bar :id="38"></expedition-bar>
			</group>

			<group :order="2">
				<material-bar></material-bar>
			</group>
		</div>
	</header>
</template>

<script lang="ts">
import { Vue, Component, Prop, PropSync } from "vue-property-decorator";

import Systembox from "@Components/Systembox.vue";
import ExpeditionBar from "@KCComponents/ExpeditionBar.vue";
import MaterialBar from "@KCComponents/MaterialBar.vue";

@Component({
	components: {
		Systembox,
		ExpeditionBar,
		MaterialBar,
	},
})
export default class TitleArea extends Vue {
	@Prop({
		type: String,
		required: true,
	})
	public title!: string;
}
</script>

<style lang="scss">
.title-area {
	grid-area: 1 / 1 / 2 / 4; // x: 1~4, y: 1~2

	@include clearfix;
	$border-sizer-size: 0px; // 5px;

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
		display: flex;

		margin-top: $border-sizer-size;
		padding: 0 10px;

		height: $theme-title-size - $border-sizer-size;

		line-height: ($theme-title-size - $border-sizer-size * 2);
		white-space: nowrap;
		overflow: hidden;

		&::after {
			content: "";
			display: table;
			clear: both;
		}
		> [data-group] {
			flex: 1;
		}
	}
}
</style>
