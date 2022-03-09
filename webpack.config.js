const path = require('path');

module.exports = {
  mode: 'development',
  entry: './webapp/App.fs.js',
  output: {
    filename: 'bundle.js',
    path: path.resolve(__dirname, 'webapp', 'public'),
  },
  devServer: {
    static: {
      directory: path.resolve(__dirname, 'webapp', 'public'),
      publicPath: '/'
    },
    port: 8080,
    open: true,
  },
};