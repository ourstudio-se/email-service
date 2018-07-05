const path = require('path');
const webpack = require('webpack');
const buildConfiguration = require('./configurations/buildConfiguration.json')
const CopyWebpackPlugin = require('copy-webpack-plugin')

module.exports = {
    devtool: "cheap-eval-source-map",
    entry: {
        default: path.resolve(__dirname, './javascripts/email.js'),
    },
    output: {
        path: buildConfiguration.TargetDirectory,
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
    "plugins": [
        new CopyWebpackPlugin([
        {
            from: path.resolve(__dirname, './configurations/emailConfiguration.json'),
            to: buildConfiguration.TargetDirectory
        },
        {
            from: path.resolve(__dirname, './configurations/serviceConfiguration.json'),
            to: buildConfiguration.TargetDirectory
        }])
    ],
    target: "node"
};