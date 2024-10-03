import { Heading, VStack } from '@chakra-ui/react';
import { FunctionComponent, ReactElement, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useLoaderData } from 'react-router-dom';
import { convertToGame, Game } from '../models/Game';

export const GamePage: FunctionComponent = (): ReactElement => {
	const data : any = useLoaderData();
	const [game, setGame] = useState<Game>({} as Game);
	const { t } = useTranslation();	

	useEffect(() => {
		setGame(convertToGame(data));		
	}, [data]);

	return (
		<VStack spacing={5} align="left">
			<Heading as="h2" size="md">{t("GameTitle", { team1: game.Team1?.Name, team2: game.Team2?.Name, goals1: game.Goals1, goals2: game.Goals2 })}</Heading>			
		</VStack>
	)
}