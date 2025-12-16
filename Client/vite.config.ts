import { defineConfig } from "vite";
import react from "@vitejs/plugin-react-swc";

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  build: {
    outDir: "dist",
    emptyOutDir: true,
  },
  server: {
    proxy: {
      // any request starting with /api goes to the .net API
      "/api": {
        target: "https://localhost:7212",
        changeOrigin: true,
        secure: false, // for self-signed dev certificates
      },
    },
  },
});
