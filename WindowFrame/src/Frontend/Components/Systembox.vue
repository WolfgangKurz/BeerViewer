<template>
	<div class="systembox">
		<div class="minimize" @click="Minimize()"></div>
		<div class="maximize" @click="Maximize()"></div>
		<div class="close" @click="Close()"></div>
	</div>
</template>

<script lang="ts">
import { Vue, Component, Prop, PropSync } from "vue-property-decorator";
import { remote } from "electron";

@Component({})
export default class Systembox extends Vue {
	/** Minimize viewer application */
	private Minimize() {
		remote.getCurrentWindow().minimize();
	}

	/** Maximize or restore viewer application */
	private Maximize() {
		const window = remote.getCurrentWindow();
		if (window.isMaximized()) remote.getCurrentWindow().restore();
		else remote.getCurrentWindow().maximize();
	}

	/** Close viewer application */
	private Close() {
		remote.getCurrentWindow().close();
	}
}
</script>

<style lang="scss">
.systembox {
	float: right;
	-webkit-app-region: no-drag;

	> div {
		float: left;
		width: 28px;
		height: 28px;
		cursor: pointer;

		background-image: url(~@/assets/sysbtn.png);
		background-repeat: no-repeat;

		background-position-y: 0;
		&:hover {
			background-position-y: -28px;
		}
		&:active {
			background-position-y: -56px;
		}

		@mixin _button($x: 0px) {
			background-position-x: -$x * 28px;
		}
		&.close {
			@include _button(0);
		}
		&.minimize {
			@include _button(1);
		}
		&.maximize {
			@include _button(2);
		}
	}
}
</style>
