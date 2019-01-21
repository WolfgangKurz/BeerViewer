const path = require('path')
const webpack = require('webpack')
const tsNameof = require("ts-nameof");

module.exports = {
  entry: "./source/System/Base.ts",
  output: {
    path: path.resolve(__dirname, "./dist"),
    filename: "beerviewer.js"
  },
  resolve: {
    extensions: ['.ts', '.js', '.vue', '.json'],
    alias: {
      'vue$': 'vue/dist/vue.esm.js'
    }
  },
  module: {
    rules: [
      {
        test: /\.vue$/,
        loader: 'vue-loader',
        options: {
          loaders: {
            // Since sass-loader (weirdly) has SCSS as its default parse mode, we map
            // the "scss" and "sass" values for the lang attribute to the right configs here.
            // other preprocessors should work out of the box, no loader config like this necessary.
            'scss': 'vue-style-loader!css-loader!sass-loader',
            'sass': 'vue-style-loader!css-loader!sass-loader?indentedSyntax',
          }
          // other vue-loader options go here
        }
      },
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
  devtool: "source-map",
  mode: "development"
}