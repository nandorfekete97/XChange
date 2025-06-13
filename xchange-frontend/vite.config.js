import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5215',
        changeOrigin: true,
        rewrite: (path) => {
          console.log('[Proxy Rewrite]', path);
          return path.replace(/^\/api/, '');
        }
      }
    },
    hot: true
  }
})
