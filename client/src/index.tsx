import { ChakraProvider, ColorModeScript } from '@chakra-ui/react';
import React from 'react';
import ReactDOM from 'react-dom/client';
import theme from './themes/theme';

import reportWebVitals from './reportWebVitals';

import { AppRouter } from './components/AppRouter';
import './i18n';

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

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
