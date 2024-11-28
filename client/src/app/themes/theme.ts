// 1. import `extendTheme` function
import { extendTheme } from '@chakra-ui/react';
import '@fontsource/roboto-condensed';
import '@fontsource/roboto-condensed/300.css';

// 2. Add your color mode config
const config = {
    // System sets initial value.
    initialColorMode: 'system',
    // App subscribes to system color mode changes.
    useSystemColorMode: true,
};

// 3. extend the theme
const theme = extendTheme(
    {
        config,
        fonts: {
            body: 'Roboto Condensed',
            heading: 'Roboto Condensed',
        },
        components: {},
        styles: {
            global: {
                input: {
                    _readOnly: {
                        opacity: '0.5 !important',
                    },
                },
            },
        },
    },
    {
        colors: {
            absent: '#2196F3',
            absentLight: '#2196F307',
            gray: {
                '50': '#F2F2F2',
                '100': '#DBDBDB',
                '200': '#C4C4C4',
                '300': '#ADADAD',
                '400': '#969696',
                '500': '#808080',
                '600': '#666666',
                '700': '#4D4D4D',
                '800': '#333333',
                '900': '#1A1A1A',
            },
            red: {
                '50': '#F9ECEC',
                '100': '#EEC9C9',
                '200': '#E2A6A6',
                '300': '#D78484',
                '400': '#CC6161',
                '500': '#C13E3E',
                '600': '#9A3232',
                '700': '#742525',
                '800': '#4D1919',
                '900': '#270C0C',
            },
            orange: {
                '50': '#FBF0EA',
                '100': '#F3D6C4',
                '200': '#EABC9E',
                '300': '#E2A278',
                '400': '#DA8852',
                '500': '#D26E2D',
                '600': '#A85824',
                '700': '#7E421B',
                '800': '#542C12',
                '900': '#2A1609',
            },
            blue: {
                '50': '#E5F2FF',
                '100': '#B8DBFF',
                '200': '#8AC4FF',
                '300': '#5CADFF',
                '400': '#2E96FF',
                '500': '#007FFF',
                '600': '#0066CC',
                '700': '#004C99',
                '800': '#003366',
                '900': '#001933',
            },
            pink: {
                '50': '#F6EFF3',
                '100': '#E5D2DC',
                '200': '#D4B5C6',
                '300': '#C397B0',
                '400': '#B37A99',
                '500': '#A25D83',
                '600': '#814B69',
                '700': '#61384F',
                '800': '#412534',
                '900': '#20131A',
            },
            purple: {
                '50': '#F1EFF6',
                '100': '#D7D2E5',
                '200': '#BDB5D4',
                '300': '#A497C3',
                '400': '#8A7AB3',
                '500': '#715DA2',
                '600': '#5A4B81',
                '700': '#443861',
                '800': '#2D2541',
                '900': '#171320',
            },
            cyan: {
                '50': '#EEF5F7',
                '100': '#CFE4E8',
                '200': '#B0D2D9',
                '300': '#91C0CA',
                '400': '#72AFBB',
                '500': '#539DAC',
                '600': '#437E89',
                '700': '#325E67',
                '800': '#213F45',
                '900': '#111F22',
            },
            teal: {
                '50': '#EEF6F6',
                '100': '#D0E7E7',
                '200': '#B1D8D7',
                '300': '#93C8C7',
                '400': '#74B9B8',
                '500': '#56A9A8',
                '600': '#458786',
                '700': '#336665',
                '800': '#224443',
                '900': '#112222',
            },
            green: {
                '50': '#EEF7F2',
                '100': '#CFE8DB',
                '200': '#B0D9C4',
                '300': '#91CAAC',
                '400': '#72BB95',
                '500': '#53AC7E',
                '600': '#428A65',
                '700': '#32674C',
                '800': '#214532',
                '900': '#112219',
            },
            yellow: {
                '50': '#F9F8EB',
                '100': '#EFECC7',
                '200': '#E5DFA3',
                '300': '#DBD37F',
                '400': '#D1C65C',
                '500': '#C7BA38',
                '600': '#9F952D',
                '700': '#786F21',
                '800': '#504A16',
                '900': '#28250B',
            },
        },
    }
);

export default theme;
