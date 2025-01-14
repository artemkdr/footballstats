import { CustomLink } from '@/components/custom-link';
import { callGetActivePlayers } from '@/features/players/api/get-players';
import { EditTeamModal } from '@/features/teams/modal-team';
import { convertToList, ListResponse } from '@/lib/types/list';
import { Team, TeamStatus } from '@/types/team';
import { convertToUserList, GetPlayerResponse, User } from '@/types/user';
import { Button, Heading, HStack, Text, VStack } from '@chakra-ui/react';
import { ReactElement, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useLoaderData } from 'react-router-dom';

export const Teams = (): ReactElement => {
    const { t } = useTranslation();    
    const teamsList = useLoaderData() as Team[];
    const [isTeamModalOpen, setIsTeamModalOpen] = useState(false);
    const [players, setPlayers] = useState<User[]>([] as User[]);
    const [playersLoaded, setPlayersLoaded] = useState(false);

    useEffect(() => {        
        if (isTeamModalOpen && !playersLoaded) {
            setPlayersLoaded(true);
            const loadPlayers = async () => {
                const response = await callGetActivePlayers<ListResponse<GetPlayerResponse>>();
                if (response.success) {                
                    setPlayers(
                        convertToUserList(convertToList(response.data)?.List)
                    );
                }
            };

            loadPlayers();
        }        
    }, [isTeamModalOpen, playersLoaded]);

    return (
        <VStack spacing={5} align="left">
            <Heading as="h2" size="md">
                {t('Teams.Title')}
            </Heading>
            <Button
                alignSelf={'start'}
                colorScheme="green"
                onClick={() => setIsTeamModalOpen(true)}
            >
                {t('Teams.AddNewTeam')}
            </Button>
            <VStack spacing={5} align="left" paddingLeft={3}>
                {teamsList?.map((item, index) => (
                    <HStack spacing={2} key={index}>
                        <CustomLink
                            link={`/team/${item.Id}`}
                            text={item.Name}
                        />
                        {item.Status !== TeamStatus.Active ? (
                            <Text>({t('UserStatus.' + item.Status)})</Text>
                        ) : (
                            ''
                        )}
                    </HStack>
                ))}
            </VStack>
            <EditTeamModal
                isOpen={isTeamModalOpen}
                onClose={() => setIsTeamModalOpen(false)}
                players={players}
            />
        </VStack>
    );
};
