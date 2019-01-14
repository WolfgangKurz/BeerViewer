var path = require('path')
var webpack = require('webpack')

module.exports = {
  entry: "./source/System/Base.ts",
  output: {
    path: path.resolve(__dirname, "./dist"),
    filename: "beerviewer.js"
  },
  resolve: {
    // Add `.ts` and `.tsx` as a resolvable extension.
    extensions: [".ts", ".tsx", ".js"]
  },
  module: {
    rules: [
      // all files with a `.ts` or `.tsx` extension will be handled by `ts-loader`
      { test: /\.tsx?$/, loader: "ts-loader" }
    ]
  },
  devtool: "source-map",
  mode: "development"
}