import react from '@vitejs/plugin-react-swc';
import { defineConfig } from 'vite';
import checker from 'vite-plugin-checker';
import tsconfigPaths from 'vite-tsconfig-paths';

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
        }),
    ],
    build: {
        outDir: 'build',
        chunkSizeWarningLimit: 1000,
    },
});
