import {
	Button,
	HStack,
	Heading,
	Text,
	VStack
} from '@chakra-ui/react';
import { FunctionComponent, ReactElement, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useLoaderData } from 'react-router-dom';
import { CustomLink } from '@/components/custom-link';
import { convertDataToTeamList, TeamStatus } from '@/types/team';
import { EditTeamModal } from '@/features/teams/modal-team';
import { convertDataToUserList, User } from '@/types/user';
import { convertDataToList } from '@/types/list';
import { callGetActivePlayers } from '@/features/players/api/get-players';

export const Teams: FunctionComponent = (): ReactElement => {
	const { t } = useTranslation();	
	const data = useLoaderData();	
	const teamsList = convertDataToTeamList(convertDataToList(data)?.List);
	const [ isTeamModalOpen, setIsTeamModalOpen ] = useState(false);
	const [ players, setPlayers ] = useState<User[]>([] as User[]);
	
	useEffect(() => { 
		const loadPlayers = async() => {
			const response = await callGetActivePlayers();
			if (response.ok) {
				var json = await response.json();			
				setPlayers(convertDataToUserList(convertDataToList(json)?.List));
			}
		}

        loadPlayers();	
    }, []);

	return (
		<VStack spacing={5} align="left">
			<Heading as="h2" size="md">{t("Teams.Title")}</Heading>
			<Button alignSelf={"start"} colorScheme="green" onClick={() => setIsTeamModalOpen(true)}>{t('Teams.AddNewTeam')}</Button>				
			<VStack spacing={5} align="left" paddingLeft={3}>
				{teamsList?.map((item, index) => (					
					<HStack spacing={2} key={index}>						
						<CustomLink link={`/team/${item.Id}`} text={item.Name} />
						{item.Status !== TeamStatus.Active ? 
							<Text>({t("UserStatus." + item.Status)})</Text>
							: ""}
					</HStack>								
				))}
			</VStack>
			<EditTeamModal
				isOpen={isTeamModalOpen} 				
				onClose={() => setIsTeamModalOpen(false)} 
				onCreate={() => {}}		
				players={players}		
				/>
		</VStack>
	)
}