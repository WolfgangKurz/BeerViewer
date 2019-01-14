var path = require('path')
var webpack = require('webpack')
var glob = require("glob")

function getEntries() {
    const list = glob.sync("./source/modules/**/*.ts");
    const output = {};
    for (let i = 0; i < list.length; i++) {
        const path = list[i];
        const parts = path.split("/");
        const modulename = parts[parts.length - 2];

        let filename = parts[parts.length - 1];
        filename = filename.substr(0, filename.lastIndexOf("."));

        output[`${modulename}/${filename}`] = path;
    }
    return output;
}

module.exports = {
    entry: getEntries(),
    output: {
        path: path.resolve(__dirname, "./dist/modules/"),
        filename: "[name].js"
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
    externals: {
        "vue": "Vue",
        "tippy.js": "Tippy.js",
        "@types/jquery": "jQuery"
    },
    devtool: "source-map",
    mode: "development"
}