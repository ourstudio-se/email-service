const path = require('path');
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
                use: [ {
                    loader: 'babel-loader',
                    options: {
                        presets: [
                            [ "es2015" ],
                            [ "react" ],
                            [ "stage-0" ]
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
