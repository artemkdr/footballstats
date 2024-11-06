import { ChakraProvider, ColorModeScript } from '@chakra-ui/react';
import React from 'react';
import ReactDOM from 'react-dom/client';
import theme from '@/app/themes/theme';

import { AppRouter } from '@/app/router';
import '@/app/i18n';

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
root.render(
  <React.StrictMode>    
    <ChakraProvider theme={theme} toastOptions={{ defaultOptions: { isClosable: true, duration: 3000, position: "top" } }}>
       <ColorModeScript />
       <AppRouter />
    </ChakraProvider>    
  </React.StrictMode>
);