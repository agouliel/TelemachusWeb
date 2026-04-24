import react from '@vitejs/plugin-react'
import postcssPluginAutoprefixer from 'autoprefixer'
import browserslistToEsbuild from 'browserslist-to-esbuild'
import postcssPluginDiscardComment from 'postcss-discard-comments'
import postcssPluginNormalizeCharset from 'postcss-normalize-charset'
import { defineConfig, loadEnv } from 'vite'
import { ViteMinifyPlugin } from 'vite-plugin-minify'

export default defineConfig(({ _command, mode }) => {
  // eslint-disable-next-line no-undef
  const env = loadEnv(mode, process.cwd())
  return {
    server: {
      host: '0.0.0.0',
      port: 3000,
      proxy: {
        '/api': { target: env.VITE_PROXY_URL ?? 'http://localhost:3001', changeOrigin: true }
      },
      open: true
    },
    plugins: [react(), ViteMinifyPlugin({ removeComments: true })], // eslint()
    esbuild: {
      target: browserslistToEsbuild(),
      legalComments: 'none'
    },
    build: { target: browserslistToEsbuild() },
    css: {
      postcss: {
        plugins: [
          postcssPluginNormalizeCharset({ add: false }),
          postcssPluginDiscardComment({ removeAll: true }),
          postcssPluginAutoprefixer({
            grid: true
          })
        ]
      }
    },
    resolve: {
      alias: {
        src: '/src'
      }
    }
  }
})
