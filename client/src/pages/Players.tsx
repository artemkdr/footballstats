import {
	Link as ChakraLink,
	HStack,
	Heading,
	Text,
	VStack
} from '@chakra-ui/react';
import { FunctionComponent, ReactElement } from 'react';
import { useTranslation } from 'react-i18next';
import { Link as ReactRouterLink, useLoaderData, useNavigate } from 'react-router-dom';
import { convertDataToUserList } from '../models/User';


export const Players: FunctionComponent = (): ReactElement => {
	const { t } = useTranslation();	
	const data = useLoaderData();
	const navigate = useNavigate();	
	const usersList = convertDataToUserList(data);
	
	return (
		<VStack spacing={5} align="left">
			<Heading as="h2" size="md">{t("Players.Title")}</Heading>
			<VStack spacing={5} align="left" paddingLeft={3}>
				{usersList?.map((item, index) => (					
					<HStack spacing={2} key={index}>
						<ChakraLink as={ReactRouterLink} to={`/player/${item.Username}`}>														
							<Text>{item.Username}</Text>																
						</ChakraLink>
					</HStack>								
				))}
			</VStack>
		</VStack>
	)
}