import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
    proxy: {
      '/api': {
        target: 'https://localhost:7260',
        changeOrigin: true,
        secure: false, // Bỏ qua SSL verification cho development với self-signed certificate
      }
    }
  }
})

