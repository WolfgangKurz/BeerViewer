const path = require("path");
const os = require("os");

// SCSS global import files
const preloadScss = [
	"~@/Theme/_loader.scss"
];

const setupAliasPath = (config) => {
	// import path alias
	config.resolve.alias.set("@", path.resolve(__dirname, "src"));
	config.resolve.alias.set("@Components", path.resolve(__dirname, "src", "Frontend", "Components"));
	config.resolve.alias.set("@KC", path.resolve(__dirname, "src", "KanColle"));
	config.resolve.alias.set("@KCComponents", path.resolve(__dirname, "src", "Frontend", "KanColle"));
};

module.exports = {
	chainWebpack: (config) => {
		setupAliasPath(config);

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
	},

	pluginOptions: {
		electronBuilder: {
			chainWebpackMainProcess: config => {
				setupAliasPath(config);
/*
				config.plugins.push(
					new CopyWebpackPlugin([
						{
							from: path.join(__dirname, '../src/worker'),
							to: path.join(__dirname, '../dist/electron/worker'),
							ignore: ['.*']
						}
					])
				)
*/
			},
			chainWebpackRendererProcess: config => {
				setupAliasPath(config);
			},
		}
	}
};