import { Button, HStack, Input, Modal, ModalBody, ModalCloseButton, ModalContent, ModalFooter, ModalHeader, ModalOverlay, Select, Text, useToast, VStack } from "@chakra-ui/react";
import moment from "moment";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { Game, GameStatus, isValidGame } from "../models/Game";
import { Team } from "../models/Team";
import callApi from "../net/api";
import { SelectTeam } from "./SelectTeam";

interface CreateNewGameModalProps {    
    isOpen: boolean;
    onClose: () => void;
    onCreate: () => void;
    teams: Team[];
}

export const CreateNewGameModal: React.FC<CreateNewGameModalProps> = ({ isOpen, onClose, onCreate, teams }) => {
    const { t } = useTranslation();    
    const [game, setGame] = useState<Game>({        
        Id: -1, // needs not null value for validation
        Goals1: 0,
        Goals2: 0,        
        Status: GameStatus.Completed,
        CompleteDate: new Date()
    } as Game);

    const nav = useNavigate();
    const toast = useToast();    
    const [isValid, setIsValid] = useState(false);
    
    useEffect(() => {
        setIsValid(isValidGame(game));        
    }, [game]);

    const createGame = async () => {
        let json : any = {           
            Goals1: game.Goals1,
            Goals2: game.Goals2,
            Team1: game.Team1?.Id,
            Team2: game.Team2?.Id,
            Status: game.Status.toString()
        };
        if (game.Status === GameStatus.Completed) {
            json["CompleteDate"] = game.CompleteDate ? game.CompleteDate : new Date();            
        }		
		const response = await callApi(`game`, { method: 'POST', body: JSON.stringify(json), headers: { "Content-Type": "application/json" }});
        const responseJson = await response.json();
        let error = false;
		if (response.ok) {
            if (responseJson?.id > 0) {
                toast({ title: t('Message.CreateGameSuccess'), status: 'success' });
                nav("/game/" + responseJson.id);
            } else {
                error = true;                
            }
		} else {
            error = true;
        }
        if (error) {                          
			toast({ title: t('Message.CreateGameError'), status: 'error' });            
		}
	};
    
    const handleChange = (event: any) => {
		const { name, value } = event.target;
        let newValue = value;

        switch (name) {
            case "Team1":
            case "Team2":
                newValue = parseInt(value) > 0 ? {
                    Id: parseInt(value)                    
                } as Team : null;
                break;
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
	};    

    return (
        <Modal isOpen={isOpen} onClose={onClose} closeOnOverlayClick={false}>
            <ModalOverlay />
            <ModalContent>
                <ModalHeader>{t('Games.NewGame')}</ModalHeader>
                <ModalCloseButton />
                <ModalBody>                    
                    <VStack spacing={5}>
                        <HStack width={"100%"}>                            
                            <SelectTeam 
                                teams={teams} name={"Team1"} 
                                placeholder={t("Games.Placeholder.Team1")}
                                value={game.Team1?.Id} textAlign={"right"} onChange={handleChange} />                            
                            <Text width={10} textAlign={"center"}>{t("Games.TeamsDelimiter")}</Text>                            
                            <SelectTeam 
                                teams={teams} name={"Team2"} 
                                placeholder={t("Games.Placeholder.Team2")}
                                value={game.Team2?.Id} textAlign={"left"} onChange={handleChange} />                                                        
                        </HStack>
                        <HStack width={"100%"}>                            
                            <Input name={"Goals1"} type={"number"} value={game.Goals1} placeholder={t("Games.Placeholder.Goals1")} textAlign={"right"} onChange={handleChange} />                            
                            <Text width={10} textAlign={"center"}>{t("Games.ScoreDelimiter")}</Text>                            
                            <Input name={"Goals2"} type={"number"} value={game.Goals2} placeholder={t("Games.Placeholder.Goals2")} onChange={handleChange} />                            
                        </HStack>
                        <HStack width={"100%"}>
                            <Select name={"Status"} placeholder={t("Games.Placeholder.Status")} value={game.Status} onChange={handleChange}>
                                {Object.values(GameStatus).map((type) => (
                                    <option key={type} value={type}>{t('GameStatus.' + type)}</option>
                                ))}
                            </Select>                                      
                            <Input name={"CompleteDate"} type={"date"} value={game.CompleteDate ? moment(game.CompleteDate).toISOString(true).split('T')[0] : ""} onChange={handleChange} />
                        </HStack>
                    </VStack>
                </ModalBody>
                <ModalFooter>
                    <Button colorScheme="green" mr={3} onClick={() => { createGame() }} isDisabled={!isValid}>{t('Create')}</Button>
                    <Button variant="ghost" onClick={onClose}>{t('Cancel')}</Button>
                </ModalFooter>
            </ModalContent>
        </Modal>
    )
}
