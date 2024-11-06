import { Badge, Button, Card, CardBody, CardHeader, HStack, Input, useToast, VStack } from '@chakra-ui/react';
import moment from 'moment';
import { FunctionComponent, ReactElement, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useLoaderData } from 'react-router-dom';
import { CustomLink } from '@/components/custom-link';
import { convertToGame, Game, GameStatus, getGameColorForResult, getGameResultFor, getGameStatusColor, isValidGame } from '@/types/game';
import { callUpdateGame } from '@/features/games/api/update-game';

export const GamePage: FunctionComponent = (): ReactElement => {
	const data : any = useLoaderData();
	const [game, setGame] = useState<Game>({
		Goals1: 0,
		Goals2: 0,
		Status: GameStatus.NotStarted
	} as Game);
	const { t } = useTranslation();		
    const toast = useToast();    
	const [isValid, setIsValid] = useState(false);

	useEffect(() => {
		setGame(convertToGame(data));		
	}, [data]);

	useEffect(() => {
        setIsValid(isValidGame(game));        
    }, [game]);

	const startGame = () => {
		setGame(prevGame => ({
			...prevGame, 
			Status: GameStatus.Playing
		}));
		updateGame({ Status: GameStatus.Playing } as Game);
	}

	const endGame = () => {
		const date = new Date();
		setGame(prevGame => ({
			...prevGame, 
			CompleteDate: date,
			Status: GameStatus.Completed
		}));
		updateGame({ Status: GameStatus.Completed, CompleteDate: date } as Game);
	}

	const deleteGame = () => {
		setGame(prevGame => ({
			...prevGame, 
			Status: GameStatus.Cancelled
		}));
		updateGame({ Status: GameStatus.Cancelled } as Game);
	}

	const updateGame = async (props: Game = {} as Game) => {		
		let json : any = {           
			Id: game.Id,
			Goals1: game.Goals1,
			Goals2: game.Goals2			
		};
		if (props.Status != null) {
			json["Status"] = props.Status.toString();			
		}
		if (props.CompleteDate != null) {
			json["CompleteDate"] = props.CompleteDate;
		}		
		const response = await callUpdateGame(game.Id, json);
		const responseJson = await response.json();
		let error = false;
		if (response.ok) {
			if (responseJson?.id > 0) {
				toast({ title: t('Message.UpdateGameSuccess'), status: 'success' });				
			} else {
				error = true;                
			}
		} else {
			error = true;
		}
		if (error) {                          
			toast({ title: t('Message.UpdateGameError'), status: 'error' });            
		}		
	}

	const handleChange = (event: any) => {
		const { name, value } = event.target;
        let newValue = value;

        switch (name) {            
            case "Goals1":
            case "Goals2":
                newValue = parseInt(value);
                break;
            case "CompleteDate":
                newValue = new Date(value);
                break;
        }

		setGame(prevGame => ({
			...prevGame, // Copy all other properties
			[name]: newValue // Update the specific property dynamically
		}));
	}

	return (
		<VStack spacing={5}>
			<Badge colorScheme={getGameStatusColor(game.Status)} padding={4}>
				{t("GameStatus." + game.Status)}
				{game.Status === GameStatus.Completed && moment(game.CompleteDate).isValid() ? " " + t("OnDate", {date: moment(game.CompleteDate).format("DD.MM.YYYY")}) : ""}
			</Badge>
			<HStack>
				<Card flex={1} minWidth={200} maxWidth={"50%"}>
					<CardHeader height={"4em"} textAlign={"center"} color={getGameColorForResult(getGameResultFor(game, game.Team1?.Id))} fontWeight={"bold"}>						
						<CustomLink link={`/team/${game.Team1?.Id}`} text={game.Team1?.Name} />
					</CardHeader>
					<CardBody textAlign={"center"}>
						<Input type="number" name="Goals1" value={game.Goals1} width={"2em"} padding={"0.2em 0.2em"} textAlign={"center"} onChange={handleChange} fontSize={"2rem"} fontWeight={"bold"} />
					</CardBody>
				</Card>
				<Card flex={1} minWidth={200} maxWidth={"50%"}>
					<CardHeader height={"4em"} textAlign={"center"} color={getGameColorForResult(getGameResultFor(game, game.Team2?.Id))} fontWeight={"bold"}>						
						<CustomLink link={`/team/${game.Team2?.Id}`} text={game.Team2?.Name}  />
					</CardHeader>
					<CardBody textAlign={"center"}>
						<Input type="number" name="Goals2" value={game.Goals2} width={"2em"} padding={"0.2em 0.2em"} textAlign={"center"} onChange={handleChange} fontSize={"2rem"} fontWeight={"bold"} />
					</CardBody>
				</Card>
			</HStack>	
			<HStack>
				{game.Status === GameStatus.Completed || game.Status === GameStatus.Playing ?
					<Button colorScheme="green" onClick={() => updateGame()} isDisabled={!isValid}>{t("Save")}</Button> : ""}
				{game.Status === GameStatus.NotStarted ?
					<Button colorScheme="green" onClick={() => startGame()}>{t("Games.Start")}</Button> : ""}
				{game.Status === GameStatus.Playing ?
					<Button colorScheme="blue" onClick={() => endGame()}>{t("Games.Finish")}</Button> : ""}
				{game.Status === GameStatus.Completed || game.Status === GameStatus.NotStarted ? 
					<Button colorScheme="gray" onClick={() => deleteGame()}>{t("Games.Cancel")}</Button> : ""}
			</HStack>							
		</VStack>
	)
}