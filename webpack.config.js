const path = require('path');
const AssetsPlugin = require('assets-webpack-plugin');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const CleanPlugin = require('clean-webpack-plugin');
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
        libraryTarget: "commonjs"
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                exclude: /node_modules\/(?!(([^\/]+?\/){1,2}(src|es6)))/,
                use: [ {
                    loader: 'babel-loader',
                    options: {
                        presets: [
                            [ "env", { "loose": true, "modules": false } ],
                            [ "react" ],
                            [ "stage-0" ]
                        ],
                        plugins: [
                            "transform-decorators-legacy"
                        ]
                    }
                } ],
            }
        ],
    },
    plugins: [
        new CleanPlugin('EmailService/wwwroot/build')
    ],
    target: "node"
};
