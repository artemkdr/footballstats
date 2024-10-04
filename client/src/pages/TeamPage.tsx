import { Badge, Heading, HStack, VStack } from '@chakra-ui/react';
import { FunctionComponent, ReactElement, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useLoaderData } from 'react-router-dom';
import { CustomLink } from '../components/CustomLink';
import { StatsTable } from '../components/StatsTable';
import { Subheader } from '../components/Subheader';
import { convertDataToGameList, Game, GameStatus, getGameColorForResult, getGameResultFor, getGameStatusColor } from '../models/Game';
import { convertToTeam, getTeamStatusColor, Team, TeamStatus } from '../models/Team';
import { convertDataToTeamStatList, TeamStat } from '../models/TeamStat';
import callApi from '../net/api';


export const TeamPage: FunctionComponent = (): ReactElement => {
	const data : any = useLoaderData();
	const [team, setTeam] = useState<Team>({} as Team);
	const { t } = useTranslation();	
	const [games, setGames] = useState<Game[]>([] as Game[]);
	const [stats, setStats] = useState<TeamStat[]>([] as TeamStat[]);

	useEffect(() => {
		setTeam(convertToTeam(data));		
	}, [data]);

	useEffect(() => {				
		const loadStats = async() => {
			const response = await callApi(`stats?team=${data.id}`);
			if (response.ok) {
				var json = await response.json();			
				setStats(convertDataToTeamStatList(json));
			}
		}
	
		const loadGames = async() => {
			const response = await callApi(`game?team1=${data.id}`);
			if (response.ok) {
				var json = await response.json();			
				setGames(convertDataToGameList(json));
			}
		}
		
		loadGames();
		loadStats();		
	}, [data?.id])
	

	return (
		<VStack spacing={5} align="start">
			<HStack spacing={5}>
				<Heading as="h2" size="md">{t("Team")} "{team.Name}"</Heading>		
				{team.Status !== TeamStatus.Active ? <Badge colorScheme={getTeamStatusColor(team.Status)} padding={4}>{t("TeamStatus." + team.Status)}</Badge> : ""}
			</HStack>
			<VStack spacing={5} align="start" paddingLeft={2}>								
				<Subheader text={t("Teams.Players")} marginTop={0} />
				{team.Players?.map((item, index) => (
					<HStack spacing={2} key={index} paddingLeft={4}>
						<CustomLink link={`/player/${item.Username}`} text={item.Username} />						
					</HStack>
				))}
				
				<Subheader text={t("Teams.Stats")} />
				<StatsTable stats={stats} />				
				
				<Subheader text={t("Games.Title")} />
				{games?.map((item, index) => (
					<HStack spacing={2} key={index} paddingLeft={4}>						
						<CustomLink link={`/game/${item.Id}`} text={t("GameTitle", { team1: item.Team1?.Name, team2: item.Team2?.Name, goals1: item.Goals1, goals2: item.Goals2 })} />						
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