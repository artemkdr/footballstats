import {
	Button,
	Heading,
	HStack,
	IconButton,
	Text,
	VStack
} from '@chakra-ui/react';
import { FunctionComponent, ReactElement, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { MdClear } from "react-icons/md";
import { useLoaderData } from 'react-router-dom';
import { CreateNewGameModal } from '../components/CreateNewGameModal';
import { CustomLink } from '../components/CustomLink';
import { SelectTeam } from '../components/SelectTeam';
import { convertDataToGameList, Game, GameStatus } from '../models/Game';
import { convertDataToRivalStats, RivalStats } from '../models/RivalStats';
import { convertDataToTeamList, Team } from '../models/Team';
import callApi from '../net/api';

export const Games: FunctionComponent = (): ReactElement => {
	const { t } = useTranslation();	
	const data = useLoaderData();	
	const [ games, setGames ] = useState<Game[]>([] as Game[]);
	const [ isNewGameModalOpen, setIsNewGameModalOpen ] = useState(false);
	const [ teams, setTeams ] = useState<Team[]>([] as Team[]);
	const [ team1, setTeam1] = useState<number>(-1);
	const [ team2, setTeam2] = useState<number>(-1);
	const [ rivalStats, setRivalStats ] = useState<RivalStats>({} as RivalStats);

	useEffect(() => {
		setGames(convertDataToGameList(data));		
	}, [data]);

	useEffect(() => { 
		const loadTeams = async() => {
			const response = await callApi(`team?status=Active`);
			if (response.ok) {
				var json = await response.json();			
				setTeams(convertDataToTeamList(json));
			}
		}

        loadTeams();	
    }, []);   

	useEffect(() => {		
		const loadGames = async() => {
			let params : string[] = [];
			if (team1 > 0)
				params.push(`team1=${team1}`);
			if (team2 > 0)
				params.push(`team2=${team2}`);
			const response = await callApi("game?" + params.join("&"));
			if (response.ok) {
				var json = await response.json();			
				setGames(convertDataToGameList(json));
			}
		}
		loadGames();
		
		if (team1 > 0 && team2 > 0) {
			const loadRivalStats = async() => {
				const response = await callApi(`statsrivals?team1=${team1}&team2=${team2}`);
				if (response.ok) {
					var json = await response.json();			
					setRivalStats(convertDataToRivalStats(json));
				}
			}
			loadRivalStats();
		}
	}, [team1, team2]);

	const handleChange = (event: any) => {
		const { name, value } = event.target;
		switch (name) {
			case "Team1":
				setTeam1(value);
				break;
			case "Team2":
				setTeam2(value);
				break;
		}
	};
	
	return (
		<VStack spacing={5} align="start">
			<Heading as="h2" size="md">{t("Games.Title")}</Heading>
			<Button alignSelf={"start"} colorScheme="green" onClick={() => setIsNewGameModalOpen(true)}>{t('Games.AddNewGame')}</Button>				
			<VStack spacing={2} align={"start"}>
				<HStack width={"100%"}>
					<SelectTeam
						teams={teams} value={team1} name={"Team1"}
						placeholder={t("Games.Placeholder.Team1")}
						textAlign={"right"} onChange={handleChange} />
					<Text width={10} textAlign={"center"}>{t("Games.TeamsDelimiter")}</Text>                            
					<SelectTeam
						teams={teams} value={team2} name={"Team2"}
						placeholder={t("Games.Placeholder.Team2")}
						textAlign={"left"} onChange={handleChange} />
					{team1 > 0 || team2 > 0 ?
						<IconButton icon={<MdClear />} aria-label="Clear teams" onClick={() => { setTeam1(-1); setTeam2(-1); }} />
						: ""}
				</HStack>
				{
					team1 > 0 && team2 > 0 ? 
						<HStack width={"100%"}>
							<Text textAlign={"right"} fontSize={"xl"} width={"50%"}>{rivalStats.Wins1}</Text>
							<Text width={10} textAlign={"center"}>{t("Games.ScoreDelimiter")}</Text>
							<Text textAlign={"left"} fontSize={"xl"} width={"50%"} marginRight={12}>{rivalStats.Wins2}</Text>
						</HStack>
						:
						""
				}
			</VStack>			
			<VStack spacing={5} align="left" paddingLeft={3}>
				{games?.map((item, index) => (					
					<HStack spacing={2} key={index}>						
						<CustomLink link={`/game/${item.Id}`} text={t("GameTitle", { team1: item.Team1?.Name, team2: item.Team2?.Name, goals1: item.Goals1, goals2: item.Goals2 })} />
						{item.Status !== GameStatus.Completed ? 
							<Text>({t("GameStatus." + item.Status)})</Text>
							: ""}
					</HStack>								
				))}
			</VStack>
			<CreateNewGameModal
				isOpen={isNewGameModalOpen} 				
				onClose={() => setIsNewGameModalOpen(false)} 
				onCreate={() => {}}
				teams={teams}
				/>
		</VStack>
	)
}