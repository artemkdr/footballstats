import {
	Button,
	Link as ChakraLink,
	HStack,
	Heading,
	Text,
	VStack
} from '@chakra-ui/react';
import { FunctionComponent, ReactElement, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link as ReactRouterLink, useLoaderData } from 'react-router-dom';
import { CreateNewGameModal } from '../components/CreateNewGameModal';
import { convertDataToGameList } from '../models/Game';

export const Games: FunctionComponent = (): ReactElement => {
	const { t } = useTranslation();	
	const data = useLoaderData();	
	const gamesList = convertDataToGameList(data);
	const [ isNewGameModalOpen, setIsNewGameModalOpen ] = useState(false);
	
	return (
		<VStack spacing={5} align="left">
			<Heading as="h2" size="md">{t("Games.Title")}</Heading>
			<Button alignSelf={"start"} colorScheme="green" onClick={() => setIsNewGameModalOpen(true)}>{t('Games.AddNewGame')}</Button>				
			<VStack spacing={5} align="left" paddingLeft={3}>
				{gamesList?.map((item, index) => (					
					<HStack spacing={2} key={index}>
						<ChakraLink as={ReactRouterLink} to={`/game/${item.Id}`}>														
							<Text>{t("GameTitle", { team1: item.Team1?.Name, team2: item.Team2?.Name, goals1: item.Goals1, goals2: item.Goals2 })}</Text>
						</ChakraLink>
						<Text>({item.Status})</Text>
					</HStack>								
				))}
			</VStack>
			<CreateNewGameModal
				isOpen={isNewGameModalOpen} 				
				onClose={() => setIsNewGameModalOpen(false)} 
				onCreate={() => {}}
				/>
		</VStack>
	)
}