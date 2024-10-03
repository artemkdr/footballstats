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
import { convertDataToGameList } from '../models/Game';

export const Games: FunctionComponent = (): ReactElement => {
	const { t } = useTranslation();	
	const data = useLoaderData();
	const navigate = useNavigate();	
	const gamesList = convertDataToGameList(data);
	
	return (
		<VStack spacing={5} align="left">
			<Heading as="h2" size="md">{t("Games.Title")}</Heading>
			<VStack spacing={5} align="left" paddingLeft={3}>
				{gamesList?.map((item, index) => (					
					<HStack spacing={2} key={index}>
						<ChakraLink as={ReactRouterLink} to={`/game/${item.Id}`}>														
							<Text>{t("GameTitle", { team1: item.Team1?.Name, team2: item.Team2?.Name, goals1: item.Goals1, goals2: item.Goals2 })}</Text>
						</ChakraLink>
					</HStack>								
				))}
			</VStack>
		</VStack>
	)
}