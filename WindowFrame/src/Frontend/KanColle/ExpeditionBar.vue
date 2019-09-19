<template>
	<div class="expedition-bar" v-if="Available">
		<div class="expedition-progress" :style="{ width: Progress }"></div>
		<div class="expedition-id">{{id}}</div>
		<div class="expedition-time">{{Remaining}}</div>
	</div>
	<!--
	<div class="expedition-bar">
		<div class="expedition-progress" style="width:90%"></div>
		<div class="expedition-id">2</div>
		<div class="expedition-time">00:25:23</div>
	</div>
	<div class="expedition-bar">
		<div class="expedition-progress" style="width:90%"></div>
		<div class="expedition-id">6</div>
		<div class="expedition-time">00:04:01</div>
	</div>
	<div class="expedition-bar">
		<div class="expedition-progress" style="width:90%"></div>
		<div class="expedition-id">38</div>
		<div class="expedition-time">01:09:56</div>
	</div>
	-->
</template>

<script lang="ts">
import Vuex from "vuex";
import { Vue, Component, Prop, PropSync } from "vue-property-decorator";

import Proxy from "@/Proxy/Proxy";

@Component({})
export default class ExpeditionBar extends Vue {
	/** Fleet id */
	@Prop({
		type: Number,
		required: true,
	})
	public id!: number;

	/**
	 * Is this expedition available?
	 *
	 * `false` if Fleet not exists.
	 */
	private Available: boolean = true;

	/** Percentage progress of expedition */
	public get Progress(): string {
		return 90 + "%";
	}

	/** Remaining time until arrive to port as string */
	public get Remaining(): string {
		return "00:25:23";
	}

	private created() {
		// Register to proxy
		// this.$store.;
		// Proxy.Instance.Register("/kcsapi/api_req_mission/result")
	}
}
</script>

<style lang="scss">
.expedition-bar {
	$height: 14px;
	//////////////////
	float: left;
	position: relative;

	margin: ($theme-title-size/2 - $height/2) 10px 0 0;
	width: 80px;
	height: $height;

	line-height: $height;
	font-size: 10px;
	text-align: center;

	text-shadow: 0 0 3px $theme-background-color, 0 0 3px $theme-background-color;
	background-color: $theme-highlight-line-color;
	color: $theme-text-color;

	vertical-align: middle;

	.expedition-progress {
		height: $height;
		background-color: $theme-highlight-color;
	}
	.expedition-id,
	.expedition-time {
		position: absolute;
		top: 0;
	}
	.expedition-id {
		left: 5px;
		font-weight: bold;
	}
	.expedition-time {
		right: 5px;
	}
}
</style>
