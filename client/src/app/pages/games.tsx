import {
	Button,
	Heading,
	HStack,
	IconButton,
	Text,
	useToast,
	UseToastOptions,
	VStack
} from '@chakra-ui/react';
import { FunctionComponent, ReactElement, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { MdClear } from "react-icons/md";
import { useLoaderData } from 'react-router-dom';
import { CreateNewGameModal } from '@/features/games/modal-game';
import { CustomLink } from '@/components/custom-link';
import { SelectTeam } from '@/features/games/select-team';
import { convertDataToGameList, Game, GameStatus } from '@/types/game';
import { convertDataToRivalStats, RivalStats } from '@/features/games/types/rival-stats';
import { convertDataToTeamList, Team } from '@/types/team';
import callApi from '@/lib/api';
import { Subheader } from '@/components/subheader';
import config from '@/config/config';
import { convertDataToList, List } from '@/types/list';

export const Games: FunctionComponent = (): ReactElement => {
	const { t } = useTranslation();	
	const data = useLoaderData();	
	const toast = useToast();    
	const [ games, setGames ] = useState<Game[]>([] as Game[]);
	const [ isNewGameModalOpen, setIsNewGameModalOpen ] = useState(false);
	const [ teams, setTeams ] = useState<Team[]>([] as Team[]);
	const [ team1, setTeam1] = useState<number>(-1);
	const [ team2, setTeam2] = useState<number>(-1);
	const [ rivalStats, setRivalStats ] = useState<RivalStats>({} as RivalStats);
	const [ list, setList] = useState<List>({} as List);

	useEffect(() => {
		const list = convertDataToList(data);
		setList(list);
		setGames(convertDataToGameList(list.List));		
	}, [data]);

	useEffect(() => { 
		const loadTeams = async() => {
			const response = await callApi(`team?status=Active`);
			if (response.ok) {
				var json = await response.json();			
				setTeams(convertDataToTeamList(convertDataToList(json)?.List));
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
				const list = convertDataToList(json);
				setList(list);
				setGames(convertDataToGameList(list.List));
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

	const simulateGames = async () => {
		const limit = config.SIMULATE_GAMES_LIMIT;
		let count = 0;
		const loadingToastProps = { status: "loading", isClosable: false, duration: 300000 } as UseToastOptions;
		const loadingToast = toast({ title: t("Message.SimulatingGames"), description: t("Message.SimulatingCount", {count: count}), ...loadingToastProps });
		for (let i = 0; i < teams.length; i++) {
			const team1 = teams[i];			
			for (let j = 0; j < teams.length; j++) {
				if (i === j) continue;
				if (count > limit) break;

				const team2 = teams[j];				
				let json : any = {           
					Goals1: Math.floor(Math.random() * 11),
					Goals2: Math.floor(Math.random() * 11),
					Team1: team1.Id,
					Team2: team2.Id,
					Status: GameStatus.Completed,
					CompleteDate: new Date()
				};				
				await callApi(`game`, { method: 'POST', body: JSON.stringify(json), headers: { "Content-Type": "application/json" }});								
				count++;
				if (count % 10 === 0) toast.update(loadingToast, { description: t("Message.SimulatingCount", {count: count}), ...loadingToastProps });				
			}
			if (count > limit) break;
		}
		toast.close(loadingToast);		
		toast({ title: t("Message.SimulateSuccess", {count: count}), status: "success" });
		window.location.reload();
	};
	
	return (
		<VStack spacing={5} align="start">
			<Heading as="h2" size="md">{t("Games.Title")} ({list.Total})</Heading>
			<HStack>
				<Button colorScheme="green" onClick={() => setIsNewGameModalOpen(true)}>{t('Games.AddNewGame')}</Button>
				{config.SIMULATE_MODE ? <Button colorScheme="gray" onClick={() => simulateGames()}>{t('Games.SimulateGames')}</Button> : ""}
			</HStack>
			<VStack spacing={2} align={"start"}>
				<Subheader text={t("Games.Filter")} />
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
							<Text textAlign={"right"} fontSize={"2rem"} width={"50%"} fontWeight={"bold"}>{rivalStats.Wins1}</Text>
							<Text width={10} textAlign={"center"}>{t("Games.ScoreDelimiter")}</Text>
							<Text textAlign={"left"} fontSize={"2rem"} width={"50%"} marginRight={12} fontWeight={"bold"}>{rivalStats.Wins2}</Text>
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