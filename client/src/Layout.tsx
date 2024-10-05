import { Box } from '@chakra-ui/react'
import { FunctionComponent, ReactElement } from 'react'
import { Outlet, useNavigation } from 'react-router-dom'
import { LoadingIndicator } from './components/LoadingIndicator'
import { NavBar } from './components/NavBar'

export const Layout: FunctionComponent = (): ReactElement => {	
	const navigation = useNavigation();
	
	return (		
		<Box width={"100%"}>
			<NavBar />
			<Box p={[4, 8]} width="100%">				
				<LoadingIndicator opacity={navigation.state === "loading" ? 1 : 0} pointerEvents={"none"} />
				<Outlet />
			</Box>
		</Box>			
	)
}