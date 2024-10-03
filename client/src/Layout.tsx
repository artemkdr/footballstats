import { Box } from '@chakra-ui/react'
import { FunctionComponent, ReactElement } from 'react'
import { Outlet } from 'react-router-dom'
import { NavBar } from './components/NavBar'

export const Layout: FunctionComponent = (): ReactElement => {	
	return (		
		<Box width={"100%"}>
			<NavBar />
			<Box p={[4, 8]} width="100%">
				<Outlet />
			</Box>
		</Box>			
	)
}