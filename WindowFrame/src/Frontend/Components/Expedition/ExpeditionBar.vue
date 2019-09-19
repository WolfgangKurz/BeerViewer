<template>
	<div class="expedition-bar">
		<div class="expedition-progress" :style="{width:Progress}"></div>
		<div class="expedition-id">{{Id}}</div>
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
import { Vue, Component, Prop, PropSync } from "vue-property-decorator";

@Component({})
export default class ExpeditionBar extends Vue {
	public get Progress(): string {
		return 90 + "%";
	}

	public get Id(): number {
		return 2;
	}

	public get Remaining(): string {
		return "00:25:23";
	}
}
</script>

<style lang="scss">
.expedition-bar {
	float: left;
	position: relative;

	margin: 2px 10px 0 0;
	width: 80px;
	height: 14px;

	line-height: 14px;
	font-size: 10px;
	text-align: center;

	text-shadow: 0 0 3px $theme-background-color, 0 0 3px $theme-background-color;
	background-color: $theme-highlight-line-color;
	color: $theme-text-color;

	vertical-align: middle;

	.expedition-progress {
		height: 14px;
		background-color: $theme-highlight-color;
	}
	.expedition-id,
	.expedition-time {
		position: absolute;
		top: 0;
	}
	.expedition-id {
		left: 5px;
	}
	.expedition-time {
		right: 5px;
	}
}
</style>
