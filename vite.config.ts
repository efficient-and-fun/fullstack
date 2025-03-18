import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    host: true,
    proxy: {
      "/api": {
        target: "http://localhost:5246", // Change to your API URL
        changeOrigin: true,
        secure: false, // Set to true if your API uses HTTPS
      },
    },
  },
  build: {
    outDir: "dist", // Ensure output folder is "dist"
    emptyOutDir: true, // Clears old files before building
  },
});
