const path = require('path')
const webpack = require('webpack')
const tsNameof = require("ts-nameof");

const ENV = "development";

module.exports = {
	entry: "./source/System/Base.ts",
	output: {
		path: path.resolve(__dirname, "./dist"),
		filename: "beerviewer.js",
		libraryTarget: 'umd'
	},
	resolve: {
		modules: [
			"node_modules",
			path.resolve(__dirname, "source")
		],
		extensions: ['.ts', '.js', '.vue', '.json']
	},
	module: {
		rules: [
			{
				test: /\.tsx?$/,
				loader: 'ts-loader',
				exclude: /node_modules/,
				options: {
					appendTsSuffixTo: [/\.vue$/],
					getCustomTransformers: () => ({ before: [tsNameof] })
				}
			}
		]
	},
	externals: {
		"vue": "Vue",
		"vuex": "Vuex",
		"tippy.js": {
			root: "tippy",
			commonjs2: "tippy.js",
			commonjs: "tippy.js",
			amd: "tippy.js"
		}
	},
	devtool: "source-map",
	mode: ENV
}