import { Button, Input, Modal, ModalBody, ModalCloseButton, ModalContent, ModalFooter, ModalHeader, ModalOverlay, Select, useToast, VStack } from "@chakra-ui/react";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { isValidTeam, Team, TeamStatus } from "../models/Team";
import { User } from "../models/User";
import callApi from "../net/api";
import { InputInlineLabel } from "./InputInlineLabel";
import { SelectPlayer } from "./SelectPlayer";

interface EditTeamModalProps {    
    isOpen: boolean;
    onClose: () => void;
    onCreate: () => void;
    players: User[];
}

export const EditTeamModal: React.FC<EditTeamModalProps> = ({ isOpen, onClose, onCreate, players }) => {
    const { t } = useTranslation();    
    const [team, setTeam] = useState<Team>({
        Id: -1, // needs not null value for validation        
        Status: TeamStatus.Active
    } as Team);

    const nav = useNavigate();
    const toast = useToast();    
    const [isValid, setIsValid] = useState(false);
    
    useEffect(() => {
        setIsValid(isValidTeam(team));        
    }, [team]);

    const createTeam = async () => {
        let json : any = {           
            Status: team.Status,
            Name: team.Name,
            Players: team.Players?.filter(x => x.Username != null).map(x => x.Username)
        };
        
		const response = await callApi(`team`, { method: 'POST', body: JSON.stringify(json), headers: { "Content-Type": "application/json" }});
        const responseJson = await response.json();
        let error = false;
		if (response.ok) {
            if (responseJson?.id > 0) {
                toast({ title: t('Message.CreateTeamSuccess'), status: 'success' });
                nav("/team/" + responseJson.id);
            } else {
                error = true;                
            }
		} else {
            error = true;
        }
        if (error) {                          
			toast({ title: t('Message.CreateTeamError'), status: 'error' });            
		}
	};
    
    const handleChange = (event: any) => {
		const { name, value } = event.target;        
        
        let newName = name;
        let newValue = value;

        if (name === "Player1" || name === "Player2") {
            var players = team.Players ?? [{} as User, {} as User];
            if (name === "Player1") players[0] = { Username: value } as User;
            if (name === "Player2") players[1] = { Username: value } as User;
            newName = "Players";
            newValue = players;
        }        

		setTeam(prevTeam => ({
			...prevTeam, 
			[newName]: newValue 
		}));
	};    

    return (
        <Modal isOpen={isOpen} onClose={onClose} closeOnOverlayClick={false}>
            <ModalOverlay />
            <ModalContent>
                <ModalHeader>{t('Teams.NewTeam')}</ModalHeader>
                <ModalCloseButton />
                <ModalBody>                    
                    <VStack spacing={5}>
                        <InputInlineLabel 
                            label={t("Teams.Name")} 
                            input={
                                <Input name={"Name"} placeholder={t("Teams.Placeholder.Name")} onChange={handleChange} />
                            }
                            />
                        <InputInlineLabel 
                            label={t("Teams.Status")} 
                            input={
                                <Select name={"Status"} placeholder={t("Teams.Status")} value={team.Status} onChange={handleChange}>
                                    {Object.values(TeamStatus).map((type) => (
                                        <option key={type} value={type}>{t('TeamStatus.' + type)}</option>
                                    ))}
                                </Select>
                            }
                            />
                        <InputInlineLabel 
                            label={t("Teams.Player1")} 
                            input={
                                <SelectPlayer placeholder={t("Teams.Placeholder.Player1")} players={players} onChange={handleChange} name={"Player1"} />
                            }
                            />
                        <InputInlineLabel 
                            label={t("Teams.Player2")} 
                            input={
                                <SelectPlayer placeholder={t("Teams.Placeholder.Player2")} players={players} onChange={handleChange} name={"Player2"} />
                            }
                            />
                        
                    </VStack>
                </ModalBody>
                <ModalFooter>
                    <Button colorScheme="green" mr={3} onClick={() => { createTeam() }} isDisabled={!isValid}>{t('Create')}</Button>
                    <Button variant="ghost" onClick={onClose}>{t('Cancel')}</Button>
                </ModalFooter>
            </ModalContent>
        </Modal>
    )
}
