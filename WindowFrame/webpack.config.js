const path = require('path')
const webpack = require('webpack')
const tsNameof = require("ts-nameof");

const ENV = "development";

module.exports = {
	entry: {
		"app": "./source/Base.ts",
		"main": "./source/Frame/Base.ts"
	},
	output: {
		path: path.resolve(__dirname, "./dist"),
		filename: "[name].js",
		libraryTarget: 'commonjs2'
	},
	resolve: {
		modules: [
			"node_modules",
			path.resolve(__dirname, "source")
		],
		extensions: ['.ts', '.js', '.json']
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
	externals: [
		"electron",
		// "jquery"
	],
	devtool: "source-map",
	mode: ENV
}