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
                }
            }
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