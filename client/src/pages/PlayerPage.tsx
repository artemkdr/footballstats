import { Badge, Link as ChakraLink, Heading, HStack, Text, VStack } from '@chakra-ui/react';
import { FunctionComponent, ReactElement, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link as ReactRouterLink, useLoaderData } from 'react-router-dom';
import { convertDataToGameList, Game, GameStatus, getGameColorForResult, getGameResultForUser, getGameStatusColor } from '../models/Game';
import { convertDataToTeamList, Team } from '../models/Team';
import { convertToUser, getUserStatusColor, User, UserStatus } from '../models/User';
import callApi from '../net/api';

export const PlayerPage: FunctionComponent = (): ReactElement => {
	const userData : any = useLoaderData();
	const [user, setUser] = useState<User>({} as User);
	const { t } = useTranslation();	
	const [games, setGames] = useState<Game[]>([] as Game[]);
	const [teams, setTeams] = useState<Team[]>([] as Team[]);

	useEffect(() => {
		setUser(convertToUser(userData));		
	}, [userData]);

	useEffect(() => {		
		const loadTeams = async() => {
			const response = await callApi(`team?players=${userData.username}`);
			if (response.ok) {
				var json = await response.json();			
				setTeams(convertDataToTeamList(json));
			}
		}
		loadTeams();		
		
		const loadGames = async() => {
			const response = await callApi(`game?players=${userData.username}`);
			if (response.ok) {
				var json = await response.json();			
				setGames(convertDataToGameList(json));
			}
		}		
		loadGames();		
	}, [userData?.username])

	return (
		<VStack spacing={5} align="left">			
			<HStack spacing={5}>
				<Heading as="h2" size="md">{t("Player")} {user.Username}</Heading>		
				{user.Status !== UserStatus.Active ? <Badge colorScheme={getUserStatusColor(user.Status)} padding={4}>{t("UserStatus." + user.Status)}</Badge> : ""}
			</HStack>
			<VStack spacing={5} align="start" paddingLeft={2}>
				<Heading as="h3" size="sm">{t("Teams.Title")}</Heading>
				{teams?.map((item, index) => (
					<HStack spacing={2} key={index} paddingLeft={4}>
						<ChakraLink as={ReactRouterLink} to={`/team/${item.Id}`}>
							<Text>{item.Name}</Text>
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
								<Badge colorScheme={getGameColorForResult(getGameResultForUser(item, user.Username))}>{t("Games." + getGameResultForUser(item, user.Username))}</Badge>
								:
								<Badge colorScheme={getGameStatusColor(item.Status)}>{t("GameStatus." + item.Status)}</Badge>
						}
					</HStack>
				))}
			</VStack>
		</VStack>
	)
}