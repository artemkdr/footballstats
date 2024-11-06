import { ChakraProvider } from "@chakra-ui/react";
import { act, fireEvent, render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { Layout } from "@/features/layout/layout";

jest.mock("./lib/api");

jest.mock("react-i18next", () => ({
    I18nextProvider: jest.fn(),
    useTranslation: () => {
        return { 
            t: (key: string) => { return key },
            i18n: {
                language: "en"                
            }
        }
    },    
}));



const mockedUseNavigate = jest.fn();
const mockedUseNavigation = jest.fn();
jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom') as any,
    useNavigate: () => mockedUseNavigate,
    useNavigation: () => mockedUseNavigation,
}));


describe("<Layout />", () => {
	it("renders Layout", () => {
        const { container } = render(<ChakraProvider><MemoryRouter><Layout /></MemoryRouter></ChakraProvider>);
        expect(screen.findByTestId('navbar')).not.toBeNull();
	});
    

    it("check that dark/light mode switcher works", async () => {        
        const { container } = render(<ChakraProvider><MemoryRouter><Layout /></MemoryRouter></ChakraProvider>);        
        const darkModeButton = screen.getByLabelText(new RegExp("switch to (dark|light) mode", "i"));                
        const newMode = darkModeButton.getAttribute("aria-label")!?.indexOf(" dark") >= 0 ? "dark" : "light";
        act(() => fireEvent.click(darkModeButton));
        await waitFor(() => {
            expect(document.querySelector(`body.chakra-ui-${newMode}`)).not.toBeNull()
        });
    });
    
});