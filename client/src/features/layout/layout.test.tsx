import { Layout } from '@/features/layout/layout';
import { ChakraProvider } from '@chakra-ui/react';
import {
    act,
    fireEvent,
    render,
    screen,
    waitFor,
} from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';

jest.mock('@/lib/net/api');

jest.mock('react-i18next', () => ({
    I18nextProvider: jest.fn(),
    useTranslation: () => {
        return {
            t: (key: string) => {
                return key;
            },
            i18n: {
                language: 'en',
            },
        };
    },
}));

const mockedUseNavigate = jest.fn();
const mockedUseNavigation = jest.fn();
jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useNavigate: () => mockedUseNavigate,
    useNavigation: () => mockedUseNavigation,
}));

describe('<Layout />', () => {
    it('renders Layout', () => {
        render(
            <ChakraProvider>
                <MemoryRouter>
                    <Layout />
                </MemoryRouter>
            </ChakraProvider>
        );
        expect(screen.findByTestId('navbar')).not.toBeNull();
    });

    it('check that dark/light mode switcher works', async () => {
        render(
            <ChakraProvider>
                <MemoryRouter>
                    <Layout />
                </MemoryRouter>
            </ChakraProvider>
        );
        const darkModeButton = screen.getByLabelText(
            new RegExp('switch to (dark|light) mode', 'i')
        );
        const lbl = darkModeButton?.getAttribute('aria-label');
        const newMode =
            lbl != null && lbl.indexOf(' dark') >= 0 ? 'dark' : 'light';
        act(() => fireEvent.click(darkModeButton));
        await waitFor(() => {
            expect(
                document.querySelector(`body.chakra-ui-${newMode}`)
            ).not.toBeNull();
        });
    });
});
