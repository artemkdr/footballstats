import { defineConfig } from "vite"
import tsconfigPaths from "vite-tsconfig-paths"
import checker from 'vite-plugin-checker';
import react from "@vitejs/plugin-react-swc";

export default defineConfig({
  plugins: [
    react(), 
    tsconfigPaths(),
    checker({
      typescript: true,
      eslint: {
          lintCommand: 'eslint .',
          useFlatConfig: true,
      },
    })
  ],
  build: {
    outDir: "build",
    chunkSizeWarningLimit: 1000,
  },
})