<template>
	<div class="right-area">
		<div class="col">
			<div class="hq">
				<div class="hq-line">
					<div class="hq-name">WolfgangKurz</div>
					<div class="hq-level">
						<small>Lv.</small>
						120
					</div>
				</div>
				<div class="hq-line">
					<div class="hq-rank">대장</div>
					<div class="hq-exp">10.0 / 11.4 / 24.3</div>
				</div>
			</div>
			<div class="action">
				<div class="action-line">
					<div>
						<!-- Ship count -->
						<img src="~@/assets/ShipCount.png" />

						<span data-critical>351</span>
						<small>/ 360</small>
					</div>
					<div>
						<!-- Equipment count -->
						<img src="~@/assets/EquipCount.png" />

						<span data-danger>1367</span>
						<small>/ 1480</small>
					</div>
				</div>

				<div class="action-line">
					<div class="action-docks">
						<!-- Construction -->
						<ship-dock name="무장님改2" :time="Date.now()" :progress="100" type="construction"></ship-dock>
						<ship-dock name="무장님改2" :time="Date.now() + 60*30*3*1000" :progress="20" type="construction"></ship-dock>
						<ship-dock name="무장님改2" :time="Date.now() + 60*20*3*1000" :progress="40" type="construction"></ship-dock>
						<ship-dock name="무장님改2" :time="Date.now() + 53*1000" :progress="60" type="construction"></ship-dock>
					</div>
					<div class="action-docks">
						<!-- Repair -->
						<ship-dock name="무장님改2" :time="Date.now()" :progress="100" type="repair"></ship-dock>
						<ship-dock name="무장님改2" :time="Date.now() + 60*30*3*1000" :progress="20" type="repair"></ship-dock>
						<ship-dock name="무장님改2" :time="Date.now() + 60*20*3*1000" :progress="40" type="repair"></ship-dock>
						<ship-dock name="무장님改2" :time="Date.now() + 53*1000" :progress="60" type="repair"></ship-dock>
					</div>
				</div>
			</div>
		</div>
		<div class="col"></div>
	</div>
</template>

<script lang="ts">
import { Vue, Component, Prop, PropSync } from "vue-property-decorator";

import ShipDock from "./ShipDock.vue";

@Component({
	components: {
		ShipDock,
	},
})
export default class RightArea extends Vue {}
</script>

<style lang="scss">
@mixin clearfix {
	&::after {
		content: "";
		display: table;
		clear: both;
	}
}
img {
	vertical-align: middle;
}

.panel {
	@include clearfix;

	display: grid;
	grid-template-columns: auto 1fr;
	grid-column-gap: 10px;

	background: repeating-linear-gradient(-45deg, transparent, transparent 4px, rgba(255, 255, 255, 0.11) 4px, rgba(255, 255, 255, 0.11) 8px);

	padding: 6px 10px;

	> .col {
		float: left;

		&:nth-child(1) {
			grid-column: 1 / 2;
		}
		&:nth-child(2) {
			grid-column: 2 / 3;
		}

		> * {
			@include clearfix;

			margin-bottom: 15px;
			background-color: #222225;
			&:last-child {
				margin-bottom: 0;
			}
		}
	}

	.hq {
		grid-column: 1 / 2;
		grid-row: 1 / 2;

		.hq-line {
			margin-top: 2px;
			&:first-child {
				margin-top: 0;
			}
		}

		.hq-name {
			float: left;
			font-size: 16px;
		}
		.hq-level {
			float: right;
			font-size: 16px;
		}
		.hq-rank {
			float: left;
			clear: both;
		}
		.hq-exp {
			float: right;
		}
	}
	.action {
		.action-line {
			display: grid;
			margin-bottom: 5px;
			grid-template-columns: 1fr 1fr;
			grid-auto-columns: 1fr;
			grid-column-gap: 5px;

			> div {
				font-size: 13px;

				img {
					margin-top: -2px;
					margin-right: 6px;
					padding: 1px;
					border-radius: 99px;
					background-color: #fff;
				}

				span {
					&[data-critical] {
						color: #c62828;
					}
					&[data-danger] {
						color: #f57c00;
					}
					font-weight: bold;
				}
				small {
					margin-left: 0.3em;
				}
			}
			@include clearfix;
		}
	}
}
</style>
