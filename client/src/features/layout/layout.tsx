import { LoadingIndicator } from '@/features/layout/loading-indicator';
import { NavBar } from '@/features/navigation/nav-bar';
import { Box } from '@chakra-ui/react';
import { FunctionComponent, ReactElement } from 'react';
import { Outlet, useNavigation } from 'react-router-dom';

export const Layout: FunctionComponent = (): ReactElement => {
    const navigation = useNavigation();

    return (
        <Box width={'100%'}>
            <NavBar />
            <Box p={[4, 8]} width="100%">
                <LoadingIndicator
                    opacity={navigation.state === 'loading' ? 1 : 0}
                    pointerEvents={'none'}
                />
                <Outlet />
            </Box>
        </Box>
    );
};
