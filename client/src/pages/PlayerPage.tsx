import { Badge, Button, Heading, HStack, useToast, VStack } from '@chakra-ui/react';
import { FunctionComponent, ReactElement, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useLoaderData } from 'react-router-dom';
import { CustomLink } from '../components/CustomLink';
import { Subheader } from '../components/Subheader';
import { convertDataToGameList, Game, GameStatus, getGameColorForResult, getGameResultForUser, getGameStatusColor } from '../models/Game';
import { convertDataToList } from '../models/List';
import { convertDataToTeamList, Team } from '../models/Team';
import { convertToUser, getUserStatusColor, User, UserStatus } from '../models/User';
import callApi from '../net/api';

export const PlayerPage: FunctionComponent = (): ReactElement => {
	const userData : any = useLoaderData();
	const [user, setUser] = useState<User>({} as User);
	const { t } = useTranslation();	
	const toast = useToast();    
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
				setTeams(convertDataToTeamList(convertDataToList(json)?.List));
			}
		}
	
		const loadGames = async() => {
			const response = await callApi(`game?players=${userData.username}`);
			if (response.ok) {
				var json = await response.json();			
				setGames(convertDataToGameList(convertDataToList(json)?.List));
			}
		}
		
		loadTeams();						
		loadGames();		
	}, [userData?.username]);

	const deleteUser = () => {
		setUser(prevUser => ({
			...prevUser, 
			Status: UserStatus.Deleted
		}));
		updateUser({ Status: UserStatus.Deleted } as User);
	}

	const activateUser = () => {
		setUser(prevUser => ({
			...prevUser, 
			Status: UserStatus.Active
		}));
		updateUser({ Status: UserStatus.Active } as User);
	}
	
	const updateUser = async (props: User = {} as User) => {		
		let json : any = {           
			Username: user.Username,
			Status: user.Status	
		};
		if (props.Status != null) {
			json["Status"] = props.Status.toString();			
		}		
		const response = await callApi(`user/${user.Username}`, { method: 'POST', body: JSON.stringify(json), headers: { "Content-Type": "application/json" }});
		const responseJson = await response.json();
		let error = false;
		if (response.ok) {
			if (responseJson?.username != null) {
				toast({ title: t('Message.UpdateUserSuccess'), status: 'success' });				
			} else {
				error = true;                
			}
		} else {
			error = true;
		}
		if (error) {                          
			toast({ title: t('Message.UpdateUserError'), status: 'error' });            
		}		
	}

	return (
		<VStack spacing={5} align="left">			
			<HStack spacing={5}>
				<Heading as="h2" size="md">{t("Player")} "{user.Username}"</Heading>		
				{user.Status !== UserStatus.Active ? <Badge colorScheme={getUserStatusColor(user.Status)} padding={4}>{t("UserStatus." + user.Status)}</Badge> : ""}
			</HStack>
			<HStack>
				{user.Status === UserStatus.Deleted ? 
					<Button alignSelf={"start"} colorScheme="green" onClick={() => activateUser()}>{t('Players.ActivatePlayer')}</Button>				
					: ""}
				{user.Status === UserStatus.Active ?
					<Button alignSelf={"start"} colorScheme="gray" onClick={() => deleteUser()}>{t('Players.DeletePlayer')}</Button>				
					: ""}
			</HStack>
			<VStack spacing={5} align="start" paddingLeft={2}>				
				<Subheader text={t("Teams.Title")} marginTop={0} />
				{teams?.map((item, index) => (
					<HStack spacing={2} key={index} paddingLeft={4}>
						<CustomLink link={`/team/${item.Id}`} text={item.Name} />
					</HStack>
				))}

				<Subheader text={t("Games.Title")} />				
				{games?.map((item, index) => (					
					<HStack spacing={2} key={index} paddingLeft={4}>						
						<CustomLink link={`/game/${item.Id}`} text={t("GameTitle", { team1: item.Team1?.Name, team2: item.Team2?.Name, goals1: item.Goals1, goals2: item.Goals2 })} />	
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