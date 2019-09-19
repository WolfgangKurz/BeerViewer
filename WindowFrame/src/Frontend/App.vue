<template>
	<div class="windowframe" :class="{ focused: WindowFocused, maximized: WindowMaximized }">
		<title-area title="BeerViewer"></title-area>
		<aside class="sidemenu">
			<div class="menu selected">
				<img src="../assets/icons/menu-overview.png" />
			</div>
			<div class="menu">
				<img src="../assets/icons/menu-settings.png" />
			</div>
		</aside>
		<article class="content">
			<div class="game">
				<div class="systeminfo">
					<div>
						Node.js
						<span>{{Vers.node}}</span>
					</div>
					<div>
						Chromium
						<span>{{Vers.chromium}}</span>
					</div>
					<div>
						Electron
						<span>{{Vers.electron}}</span>
					</div>
				</div>

				<img
					src="../assets/0795_9863.png"
					style="position: absolute;right:-100px;bottom:-300px;opacity:0.3"
				/>
				<webview id="GAME" src="about:blank"></webview>
			</div>
		</article>
		<div class="r-content">Right content</div>
		<div class="b-content">Bottom content</div>
	</div>
</template>

<script lang="ts">
import { Vue, Component, Prop, PropSync } from "vue-property-decorator";

import TitleArea from "@Components/TitleArea.vue";
import { ipcRenderer, remote } from "electron";

@Component({
	components: {
		TitleArea,
	},
})
export default class App extends Vue {
	/** Is viewer application focused? */
	private WindowFocused: boolean = false;

	/** Is viewer application maximized? */
	private WindowMaximized: boolean = false;

	/** System versions */
	private get Vers() {
		return {
			node: process.versions.node,
			chromium: process.versions.chrome,
			electron: process.versions.electron,
		};
	}

	/**
	 * Update window focus state
	 * @param _ Unused
	 * @param state 1 if focused, 0 if not focused.
	 */
	private updateWindowFocus(_: unknown, state: 0 | 1) {
		this.WindowFocused = state !== 0;
	}

	/**
	 * Update window maximized state
	 * @param _ Unused
	 * @param state 1 if focused, 0 if not focused.
	 */
	private updateWindowMaximized(_: unknown, state: 0 | 1) {
		this.WindowMaximized = state !== 0;
	}

	private mounted() {
		// Register window focus changed event, Initial focus state
		ipcRenderer.on("window-focus-state", this.updateWindowFocus);
		// this.updateWindowFocus(null, remote.getCurrentWindow().isFocused() ? 1 : 0);

		// Register window style changed event
		ipcRenderer.on("window-maximized-state", this.updateWindowMaximized);
		// this.updateWindowFocus(null, remote.getCurrentWindow().isMaximized() ? 1 : 0);

		// Initialize Game
		// Game.Initialize();
	}

	private beforeDestroy() {
		ipcRenderer.removeListener("window-focus-state", this.updateWindowFocus);
		ipcRenderer.removeListener("window-maximized-state", this.updateWindowMaximized);
	}
}
</script>

<style lang="scss">
@font-face {
	font-family: "Spoqa Han Sans";
	font-weight: 700;
	src: url("~@/assets/fonts/SpoqaHanSansBold.woff2") format("woff2");
}
@font-face {
	font-family: "Spoqa Han Sans";
	font-weight: 400;
	src: url("~@/assets/fonts/SpoqaHanSansRegular.woff2") format("woff2");
}
@font-face {
	font-family: "Spoqa Han Sans JP";
	font-weight: 700;
	src: url("~@/assets/fonts/SpoqaHanSansJPBold.woff2") format("woff2");
}
@font-face {
	font-family: "Spoqa Han Sans JP";
	font-weight: 400;
	src: url("~@/assets/fonts/SpoqaHanSansJPRegular.woff2") format("woff2");
}

html,
body {
	margin: 0;
	padding: 0;
	width: 100%;
	height: 100%;
	border: 0;
	outline: 0;
}
body {
	background-color: $theme-color;
	font: $theme-font-size $theme-font-face, "Segoe UI", "Malgun Gothic", sans-serif;
	color: $theme-text-color;
	overflow: hidden;
}

.windowframe {
	display: grid;
	grid-template-columns: auto auto 1fr; // side, game, right-content
	grid-template-rows: auto auto 1fr; // title-area, game, bottom-content

	height: calc(100% - 2px);
	border: 0 solid transparent;
	&:not(.maximized) {
		border: 1px solid $theme-active-color;
	}
	&.focused {
		border-color: $theme-highlight-color;
	}

	.sidemenu {
		display: flex;
		flex-direction: column;

		grid-area: 2 / 1 / 4 / 2; // x: 1~2, y: 2~4
		-webkit-user-select: none;

		.menu {
			display: block;
			width: 40px;
			height: 40px;
			line-height: 40px;
			text-align: center;
			background-color: $theme-color;
			cursor: pointer;

			> img {
				display: inline-block;
				margin: 2px;
			}
			&:hover {
				background-color: $theme-hover-color;
			}
			&:active {
				background-color: $theme-active-color;
			}
			&.selected {
				background-color: $theme-highlight-color;
				&:hover {
					background-color: lighten($theme-highlight-color, 8%);
				}
				&:active {
					background-color: darken($theme-highlight-color, 8%);
				}
			}
		}

		&::after {
			content: "";
			box-shadow: -3px 8px 10px inset transparentize($theme-background-color, 0.51);
			flex: 1;
		}
	}
	.content {
		grid-area: 2 / 2 / 3 / 3; // x: 2~3, y: 2~3
	}
	.r-content {
		grid-area: 2 / 3 / 4 / 4; // x: 3~4, y: 2~4
	}
	.b-content {
		grid-area: 3 / 2 / 4 / 3; // x: 2~3, y: 3~4
	}
}

.windowframe > .content .game {
	$factor: (2 / 3);

	position: relative;
	width: 1200px * $factor;
	height: 720px * $factor;
	background-color: $theme-highlight-line-color;
	overflow: hidden;

	.systeminfo {
		padding: 10px 20px;
		font-weight: bold;
		font-size: 28px;
		color: $theme-color;
		opacity: 0.47;
	}

	#GAME {
		position: absolute;
		display: inline-flex;
		left: 0;
		top: 0;
		width: 800px;
		height: 480px;
		z-index: 1;
	}
}
</style>
