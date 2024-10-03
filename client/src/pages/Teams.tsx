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
import { convertDataToTeamList } from '../models/Team';

export const Teams: FunctionComponent = (): ReactElement => {
	const { t } = useTranslation();	
	const data = useLoaderData();
	const navigate = useNavigate();	
	const teamsList = convertDataToTeamList(data);
	
	return (
		<VStack spacing={5} align="left">
			<Heading as="h2" size="md">{t("Teams.Title")}</Heading>
			<VStack spacing={5} align="left" paddingLeft={3}>
				{teamsList?.map((item, index) => (					
					<HStack spacing={2} key={index}>
						<ChakraLink as={ReactRouterLink} to={`/team/${item.Id}`}>														
							<Text>{item.Name}</Text>																
						</ChakraLink>
					</HStack>								
				))}
			</VStack>
		</VStack>
	)
}