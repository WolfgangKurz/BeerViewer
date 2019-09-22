<template>
	<div class="ship-dock" :data-type="type" :data-raw="TimeStr.Raw">
		<div class="ship-dock-progress" :style="{ width: progress+'%' }"></div>
		<div class="ship-dock-name">{{name}}</div>
		<div class="ship-dock-time" :data-level="TimeStr.Level">
			<span>{{TimeStr.Hours}}</span>
			:
			<span>{{TimeStr.Minutes}}</span>
			:
			<span>{{TimeStr.Seconds}}</span>
		</div>
	</div>
</template>

<script lang="ts">
import { Vue, Component, Prop, PropSync } from "vue-property-decorator";

@Component({})
export default class ShipDock extends Vue {
	@Prop({
		type: String,
		required: true,
	})
	public name!: string;

	@Prop({
		type: Number,
		required: true,
	})
	public time!: number;

	@Prop({
		type: Number,
		default: 0,
	})
	public progress!: number;

	@Prop({
		type: String,
		default: "repair",
	})
	public type!: "repair" | "construction";

	private get TimeStr() {
		const to = Math.max(0, this.time - Date.now()) / 1000;

		const h = Math.floor(to / 3600);
		const m = Math.floor(to / 60) % 60;
		const s = Math.floor(to) % 60;

		let level: number = 0;
		if (h === 0 && m === 0 && s === 0) level = 3;
		else if (h === 0 && m === 0) level = 2;
		else if (h === 0) level = 1;

		return {
			Hours: ("0" + h).substr(-2),
			Minutes: ("0" + m).substr(-2),
			Seconds: ("0" + s).substr(-2),
			Level: level,
			Raw: to,
		};
	}
}
</script>

<style lang="scss">
.ship-dock {
	position: relative;
	margin-top: 5px;
	padding: 2px 3px;
	height: 25px;

	background-color: $theme-dock-background;
	text-shadow: $theme-dock-text-shadow;
	color: $theme-dock-text-color;

	line-height: 1.1em;
	font-size: 12px;
	text-align: center;

	&:first-child {
		margin-top: 0;
	}

	> .ship-dock-progress {
		position: absolute;
		left: 0;
		top: 0;
		height: 100%;
		z-index: 1;
	}
	> .ship-dock-name,
	> .ship-dock-time {
		position: relative;
		z-index: 2;
	}

	> .ship-dock-time {
		$zero_opacity: 0.45;

		&[data-level="3"] > span {
			opacity: $zero_opacity;
		}
		&[data-level="2"] {
			> span:nth-child(1),
			> span:nth-child(2) {
				opacity: $zero_opacity;
			}
		}
		&[data-level="1"] {
			> span:nth-child(1) {
				opacity: $zero_opacity;
			}
		}
	}

	&[data-raw="0"] {
		> .ship-dock-name {
			line-height: 25px;
		}
		> .ship-dock-time {
			display: none;
		}
	}

	&[data-type="repair"] .ship-dock-progress {
		background-color: map-get($state-colors, "ready");
	}
	&[data-type="construction"] .ship-dock-progress {
		background-color: map-get($state-colors, "repairing");
	}
}
</style>
