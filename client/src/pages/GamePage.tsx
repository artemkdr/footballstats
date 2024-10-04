import { Badge, Card, CardBody, CardHeader, HStack, VStack } from '@chakra-ui/react';
import moment from 'moment';
import { FunctionComponent, ReactElement, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useLoaderData } from 'react-router-dom';
import { CustomLink } from '../components/CustomLink';
import { convertToGame, Game, GameStatus, getGameColorForResult, getGameResultFor, getGameStatusColor } from '../models/Game';

export const GamePage: FunctionComponent = (): ReactElement => {
	const data : any = useLoaderData();
	const [game, setGame] = useState<Game>({} as Game);
	const { t } = useTranslation();	

	useEffect(() => {
		setGame(convertToGame(data));		
	}, [data]);

	return (
		<VStack spacing={5}>
			<Badge colorScheme={getGameStatusColor(game.Status)} padding={4}>
				{t("GameStatus." + game.Status)}
				{game.Status === GameStatus.Completed && moment(game.CompleteDate).isValid() ? " : " + moment(game.CompleteDate).format("DD.MM.YYYY") : ""}
			</Badge>
			<HStack>
				<Card minWidth={100} maxWidth={"50%"}>
					<CardHeader textAlign={"center"} color={getGameColorForResult(getGameResultFor(game, game.Team1?.Id))} fontWeight={"bold"}>						
						<CustomLink link={`/team/${game.Team1?.Id}`} text={game.Team1?.Name} textDecoration={"underline"} />
					</CardHeader>
					<CardBody textAlign={"center"} fontSize={"xl"}>{game.Goals1}</CardBody>
				</Card>
				<Card minWidth={100} maxWidth={"50%"}>
					<CardHeader textAlign={"center"} color={getGameColorForResult(getGameResultFor(game, game.Team2?.Id))} fontWeight={"bold"}>						
						<CustomLink link={`/team/${game.Team2?.Id}`} text={game.Team2?.Name} textDecoration={"underline"} />						
					</CardHeader>
					<CardBody textAlign={"center"} fontSize={"xl"}>{game.Goals2}</CardBody>
				</Card>
			</HStack>						
		</VStack>
	)
}