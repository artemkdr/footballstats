import { CustomLink } from '@/components/custom-link';
import { SimpleSuspense } from '@/components/simple-suspense';
import config from '@/config/config';
import { EditPlayerModal } from '@/features/players/components/edit-player-modal';
import { generatePlayers } from '@/features/players/utils/generate-players';
import { Player, PlayerStatus } from '@/types/player';
import {
    Button,
    Heading,
    HStack,
    Text,
    VStack,
} from '@chakra-ui/react';
import { FunctionComponent, ReactElement, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useLoaderData } from 'react-router-dom';

export const Players: FunctionComponent = (): ReactElement => {
    const { t } = useTranslation();
    const playersList = useLoaderData() as Player[];    
    const [isPlayerModalOpen, setIsPlayerModalOpen] = useState(false);

    return (
        <VStack spacing={5} align="left">
            <Heading as="h2" size="md">
                {t('Players.Title')}
            </Heading>
            <HStack>
                <Button
                    alignSelf={'start'}
                    colorScheme="green"
                    onClick={() => setIsPlayerModalOpen(true)}
                >
                    {t('Players.AddNewPlayer')}
                </Button>
                {config.SIMULATE_MODE ? (
                    <Button colorScheme="gray" onClick={() => generatePlayers()}>
                        {t('Players.GeneratePlayers', {
                            count: config.SIMULATE_PLAYERS_NUM,
                        })}
                    </Button>
                ) : (
                    ''
                )}
            </HStack>
            <VStack spacing={5} align="left" paddingLeft={3}>
                <SimpleSuspense fallback={t('Loading')} emptyText={t('Empty')}>
                    {playersList?.map((item, index) => (
                        <HStack spacing={2} key={index}>
                            <CustomLink
                                link={`/player/${item.Username}`}
                                text={item.Username}
                            />
                            {item.Status !== PlayerStatus.Active ? (
                                <Text>({t('PlayerStatus.' + item.Status)})</Text>
                            ) : (
                                ''
                            )}
                        </HStack>
                    ))}
                </SimpleSuspense>
            </VStack>
            <EditPlayerModal
                isOpen={isPlayerModalOpen}
                onClose={() => setIsPlayerModalOpen(false)}
            />
        </VStack>
    );
};
