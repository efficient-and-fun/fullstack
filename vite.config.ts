import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    host: true,
  },
  build: {
    outDir: "dist", // Ensure output folder is "dist"
    emptyOutDir: true, // Clears old files before building
  },
});
