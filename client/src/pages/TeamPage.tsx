import { Badge, Link as ChakraLink, Heading, HStack, Text, VStack } from '@chakra-ui/react';
import { FunctionComponent, ReactElement, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link as ReactRouterLink, useLoaderData } from 'react-router-dom';
import { convertToTeam, getTeamStatusColor, Team, TeamStatus } from '../models/Team';
import { convertDataToGameList, Game, GameStatus, getGameColorForResult, getGameResultFor, getGameStatusColor } from '../models/Game';
import callApi from '../net/api';


export const TeamPage: FunctionComponent = (): ReactElement => {
	const data : any = useLoaderData();
	const [team, setTeam] = useState<Team>({} as Team);
	const { t } = useTranslation();	
	const [games, setGames] = useState<Game[]>([] as Game[]);

	useEffect(() => {
		setTeam(convertToTeam(data));		
	}, [data]);

	useEffect(() => {
		
		const loadGames = async() => {
			const response = await callApi(`game?team1=${data.id}`);
			if (response.ok) {
				var json = await response.json();			
				setGames(convertDataToGameList(json));
			}
		}
		loadGames();
		
	}, [data?.id])
	

	return (
		<VStack spacing={5} align="start">
			<HStack spacing={5}>
				<Heading as="h2" size="md">{t("Team")} {team.Name}</Heading>		
				{team.Status !== TeamStatus.Active ? <Badge colorScheme={getTeamStatusColor(team.Status)} padding={4}>{t("TeamStatus." + team.Status)}</Badge> : ""}
			</HStack>
			<VStack spacing={5} align="start" paddingLeft={2}>
				<Heading as="h3" size="sm">{t("Teams.Players")}</Heading>		
				{team.Players?.map((item, index) => (
					<HStack spacing={2} key={index} paddingLeft={4}>
						<ChakraLink as={ReactRouterLink} to={`/player/${item.Username}`}>														
							<Text>{item.Username}</Text>																
						</ChakraLink>
					</HStack>
				))}
				<Heading as="h3" size="sm">{t("Games.Title")}</Heading>
				{games?.map((item, index) => (
					<HStack spacing={2} key={index} paddingLeft={4}>
						<ChakraLink as={ReactRouterLink} to={`/game/${item.Id}`}>
							<Text>{t("GameTitle", { team1: item.Team1?.Name, team2: item.Team2?.Name, goals1: item.Goals1, goals2: item.Goals2 })}</Text>
						</ChakraLink>
						{
							item.Status === GameStatus.Completed 
								?
								<Badge colorScheme={getGameColorForResult(getGameResultFor(item, team.Id))}>{t("Games." + getGameResultFor(item, team.Id))}</Badge>
								:
								<Badge colorScheme={getGameStatusColor(item.Status)}>{t("GameStatus." + item.Status)}</Badge>
						}
					</HStack>
				))}
			</VStack>
		</VStack>
	)
}