<template>
	<div id="top-resources">
		<div
			class="top-resource"
			v-for="(res, key) in Datas"
			:key="`MaterialBar-${key}`"
			:data-overlimit="res.Type === 'Resource' && res.Value >= Overlimit ? 1 : 0"
		>
			<template v-if="res.Type === 'Resource'">
				<img class="resource-icon" :src="res.Icon" />
				<div class="resource-value">{{res.Value}}</div>
			</template>
			<template v-else>
				<img class="resource-icon" :src="'modules/top-resource/icon_'+res.LowerName+'.png'" />
				<div class="resource-value">
					{{res.Value}}
					<small>/{{res.Maximum}}</small>
				</div>
			</template>
		</div>
	</div>
</template>

<script lang="ts">
import { Vue, Component, Prop, PropSync } from "vue-property-decorator";

import path from "path";
import Materials from "@KC/Classes/Materials";

import KanColleStoreClient, { StoreInterface } from "@KC/Store/KanColleStoreClient";

@Component({})
export default class MaterialBar extends KanColleStoreClient {
	private get Materials() {
		return this.StoreMaterials;
	}

	public get Datas() {
		return (Object.keys(this.Materials) as Array<keyof StoreInterface.Materials>)
			.map((x) => [this.Materials[x], x])
			.map((x) => {
				const icon = path.join(process.env.BASE_URL as string, "assets", "icons", `${x[1]}.png`);
				return {
					Type: "Resource",
					Value: x[0],
					Maximum: 300000,
					LowerName: x[1],
					Icon: icon,
				};
			});
	}

	public get Overlimit(): number {
		return 30000;
	}
}
</script>

<style lang="scss">
#top-resources {
	margin-left: auto;
	white-space: nowrap;
	line-height: 0;
	text-align: right;

	> .top-resource {
		display: inline-block;
		margin-right: 1em;
		height: $theme-title-size;
		line-height: $theme-title-size;
		font-size: 11px;
		overflow: hidden;
	}

	> .top-resource > * {
		display: inline-block;

		&:last-child {
			margin-right: 0;
		}
	}

	> .top-resource .resource-icon {
		margin-right: 4px;
		margin-bottom: 2px;
		vertical-align: middle;
	}

	> .top-resource .resource-value {
		white-space: nowrap;

		small {
			font-size: 10px;
			opacity: 0.88;
		}
	}
}
</style>
