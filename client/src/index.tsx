import theme from '@/app/themes/theme';
import { ChakraProvider, ColorModeScript } from '@chakra-ui/react';
import React from 'react';
import ReactDOM from 'react-dom/client';

import '@/app/i18n';
import { AppRouter } from '@/app/router';

const root = ReactDOM.createRoot(
    document.getElementById('root') as HTMLElement
);
root.render(
    <React.StrictMode>
        <ChakraProvider
            theme={theme}
            toastOptions={{
                defaultOptions: {
                    isClosable: true,
                    duration: 3000,
                    position: 'top',
                },
            }}
        >
            <ColorModeScript />
            <AppRouter />
        </ChakraProvider>
    </React.StrictMode>
);
