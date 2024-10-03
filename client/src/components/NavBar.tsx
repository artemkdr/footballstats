import { Link as ChakraLink, Flex, HStack, useColorMode } from '@chakra-ui/react'
import { FunctionComponent, ReactElement } from 'react'
import { useTranslation } from 'react-i18next'
import { NavLink as ReactRouterLink } from 'react-router-dom'

export const NavBar: FunctionComponent = (): ReactElement => {
	const { colorMode } = useColorMode();		
	const { t } = useTranslation();
	const activeStyle = {fontWeight: 'bold'};
	
	return (
		<Flex
			alignItems={'center'}
			justifyContent={'space-between'}
			borderBottom={1}
			borderStyle={'solid'}
			borderColor={colorMode === 'light' ? 'gray.100' : 'gray.900'}
			shadow={'sm'}
			px={[2,4,8]}
			py={[2,4]}
			fontSize={['xs', 'xs', 'xs', 'sm']}
			width="100%" overflowX={"auto"} >
			
			<HStack overflow={"auto"} spacing={[2,4]}>
				<ChakraLink as={ReactRouterLink} to="/dashboard" mr={[1,4,6,8]} _activeLink={activeStyle}>{t('Navigation.Home')}</ChakraLink>								
				<ChakraLink as={ReactRouterLink} to="/teams" mr={[1,4,6,8]} _activeLink={activeStyle}>{t('Navigation.Teams')}</ChakraLink>								
				<ChakraLink as={ReactRouterLink} to="/players" mr={[1,4,6,8]} _activeLink={activeStyle}>{t('Navigation.Players')}</ChakraLink>								
			</HStack>			
		</Flex>
	)
}