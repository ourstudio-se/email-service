const path = require('path');
const AssetsPlugin = require('assets-webpack-plugin');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const webpack = require('webpack');

module.exports = {
    devtool: "cheap-eval-source-map",
    context: path.resolve(__dirname, './EmailService/wwwroot'),
    entry: {
        default: './javascripts/email.js',
    },
    output: {
        path: path.resolve(__dirname, './EmailService', 'wwwroot', 'build'),
        filename: '[name].bundle.js',
        publicPath: '/build/'
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                exclude: /node_modules\/(?!(([^\/]+?\/){1,2}(src|es6)))/,
                use: [{
                    loader: 'babel-loader',
                    options: {
                        presets: [
                            ["es2015", { "loose": true, "modules": false }],
                            ["react"],
                            ["stage-0"]
                        ],
                        plugins: [
                            "transform-decorators-legacy"
                        ]
                    }
                }],
            }
        ],
    },
    plugins: [
        new AssetsPlugin({
            filename: 'webpack.assets.json',
            path: path.resolve(__dirname, './EmailService', 'wwwroot', 'build'),
            prettyPrint: true
        }),
        new ExtractTextPlugin({
            filename: '[name].bundle.css',
            allChunks: true,
        })
    ],
};
