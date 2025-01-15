import { CustomLink } from '@/components/custom-link';
import { Subheader } from '@/components/subheader';
import { callGetGamesWithPlayers } from '@/features/games/api/get-games';
import { callUpdatePlayer } from '@/features/players/api/update-player';
import { callGetTeamsWithPlayers } from '@/features/teams/api/get-teams';
import {
    convertToGameList,
    Game,
    GameStatus,
    getGameColorForResult,
    getGameResultForPlayer,
    getGameStatusColor,
} from '@/types/game';
import { convertToList } from '@/lib/types/list';
import { convertToTeamList, Team } from '@/types/team';
import {
    getPlayerStatusColor,
    UpdatePlayerResponse,
    Player,
    PlayerStatus,
} from '@/types/player';
import {
    Badge,
    Button,
    Heading,
    HStack,    
    useToast,
    VStack,
} from '@chakra-ui/react';
import { ReactElement, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useLoaderData } from 'react-router-dom';
import { SimpleSuspense } from '@/components/simple-suspense';

export const PlayerPage = (): ReactElement => {
    const data = useLoaderData() as Player;
    const [player, setPlayer] = useState<Player>({} as Player);
    const { t } = useTranslation();
    const toast = useToast();
    const [games, setGames] = useState<Game[] | undefined>(undefined);
    const [teams, setTeams] = useState<Team[] | undefined>(undefined);

    useEffect(() => {
        setPlayer(data);
    }, [data]);

    useEffect(() => {
        const loadTeams = async () => {
            const response = await callGetTeamsWithPlayers(data.Username);
            if (response.success) {                
                setTeams(convertToTeamList(convertToList(response.data)?.List) ?? []);
            }
        };

        const loadGames = async () => {
            const response = await callGetGamesWithPlayers(data.Username);
            if (response.success) {                
                setGames(convertToGameList(convertToList(response.data)?.List) ?? []);
            }
        };

        loadTeams();
        loadGames();
    }, [data?.Username]);

    const updatePlayer = async (props: Player = {} as Player) => {
        const json = { ...player, ...props };
        const response = await callUpdatePlayer<UpdatePlayerResponse>(player.Username, json);        
        let error = false;
        if (response.success) {
            if (response.data?.username != null) {
                toast({
                    title: t('Message.UpdateUserSuccess'),
                    status: 'success',
                });
            } else {
                error = true;
            }
        } else {
            error = true;
        }
        if (error) {
            toast({ title: t('Message.UpdateUserError'), status: 'error' });
        }
    };

    const deletePlayer = () => {
        setPlayer((prevPlayer) => ({
            ...prevPlayer,
            Status: PlayerStatus.Deleted,
        }));
        updatePlayer({ Status: PlayerStatus.Deleted } as Player);
    };

    const activatePlayer = () => {
        setPlayer((prevPlayer) => ({
            ...prevPlayer,
            Status: PlayerStatus.Active,
        }));
        updatePlayer({ Status: PlayerStatus.Active } as Player);
    };

    return (
        <VStack spacing={5} align="left">
            <HStack spacing={5}>
                <Heading as="h2" size="md">
                    {t('Player')} "{player.Username}"
                </Heading>
                {player.Status !== PlayerStatus.Active ? (
                    <Badge
                        colorScheme={getPlayerStatusColor(player.Status)}
                        padding={4}
                    >
                        {t('PlayerStatus.' + player.Status)}
                    </Badge>
                ) : (
                    ''
                )}
            </HStack>
            <HStack>
                {player.Status === PlayerStatus.Deleted ? (
                    <Button
                        alignSelf={'start'}
                        colorScheme="green"
                        onClick={() => activatePlayer()}
                    >
                        {t('Players.ActivatePlayer')}
                    </Button>
                ) : (
                    ''
                )}
                {player.Status === PlayerStatus.Active ? (
                    <Button
                        alignSelf={'start'}
                        colorScheme="gray"
                        onClick={() => deletePlayer()}
                    >
                        {t('Players.DeletePlayer')}
                    </Button>
                ) : (
                    ''
                )}
            </HStack>
            <VStack spacing={5} align="start" paddingLeft={2}>
                <Subheader marginTop={0}>{t('Teams.Title')}</Subheader>
                <SimpleSuspense 
                    fallback={t('Loading')}
                    emptyText={t('Empty', { name: t('Game').toLowerCase() })}>
                    {teams?.map((item, index) => (
                        <HStack spacing={2} key={index} paddingLeft={4}>
                            <CustomLink
                                link={`/team/${item.Id}`}
                                text={item.Name}
                            />
                        </HStack>
                    ))}
                </SimpleSuspense>

                <Subheader>{t('Games.Title')}</Subheader>
                <SimpleSuspense 
                    fallback={t('Loading')}
                    emptyText={t('Empty', { name: t('Game').toLowerCase() })}>
                    {games?.map((item, index) => (
                        <HStack spacing={2} key={index} paddingLeft={4}>
                            <CustomLink
                                link={`/game/${item.Id}`}
                                text={t('GameTitle', {
                                    team1: item.Team1?.Name,
                                    team2: item.Team2?.Name,
                                    goals1: item.Goals1,
                                    goals2: item.Goals2,
                                })}
                            />
                            {item.Status === GameStatus.Completed ? (
                                <Badge
                                    colorScheme={getGameColorForResult(
                                        getGameResultForPlayer(item, player.Username)
                                    )}
                                >
                                    {t(
                                        'Games.' +
                                            getGameResultForPlayer(
                                                item,
                                                player.Username
                                            )
                                    )}
                                </Badge>
                            ) : (
                                <Badge
                                    colorScheme={getGameStatusColor(item.Status)}
                                >
                                    {t('GameStatus.' + item.Status)}
                                </Badge>
                            )}
                        </HStack>
                    ))}
                </SimpleSuspense>                
            </VStack>
        </VStack>
    );
};
