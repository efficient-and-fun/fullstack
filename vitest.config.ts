import { defineConfig } from "vitest/config";

export default defineConfig({
  test: {
    globals: true, // Enables global variables like describe, it, expect
    environment: "jsdom", // Use jsdom for browser-like environment (if you're testing React)
    testTransformMode: {
      web: ["\\.[tj]sx$"], // Enable transformations for TypeScript/JSX files
    },
  },
});
