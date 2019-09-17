const path = require("path");
const fs = require("fs");
const glob = require("glob");
const os = require("os");

// SCSS global import files
const preloadScss = [
	"@/Theme/_template.scss",
	"@/Theme/_default.scss"
];

module.exports = {
	filenameHashing: false,
	chainWebpack: config => {
		// import path alias
		config.resolve.alias.set("@", path.resolve(__dirname, "src"));
		config.resolve.alias.set("@Components", path.resolve(__dirname, "src", "Frontend", "Components"));

		// Disable preload, prefetch tags
		config.plugins.delete("preload");
		config.plugins.delete("prefetch");

		// Source-map style (Default was cheap-module-eval-source-map)
		config.set("devtool", "source-map");

		// https://www.npmjs.com/package/fork-ts-checker-webpack-plugin
		config
			.plugin("fork-ts-checker")
			.tap(args => {
				if (args.length > 0) {
					args[0].workers = Math.floor(os.cpus().length / 2);
					args[0].memoryLimit = 2048;
				}
				return args;
			});
	},

	css: {
		modules: true,
		loaderOptions: {
			sass: {
				// CSS global code
				data: preloadScss.map(x => `@import "${x}";`).join("\n")
			},
			postcss: {
				plugins: [
					require("autoprefixer")
				]
			}
		}
	}
};