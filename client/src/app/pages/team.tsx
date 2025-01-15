import { CustomLink } from '@/components/custom-link';
import { Subheader } from '@/components/subheader';
import { callGetGamesWithTeam } from '@/features/games/api/get-game';
import { callGetTeamStats } from '@/features/stats/api/get-stats';
import { StatsTable } from '@/features/stats/stats-table';
import {
    convertToTeamStatList,
    GetTeamStatsResponse,
    TeamStat,
} from '@/features/stats/types/team-stat';
import {
    convertToGameList,
    Game,
    GameStatus,
    getGameColorForResult,
    GetGameResponse,
    getGameResultFor,
    getGameStatusColor,
} from '@/types/game';
import { convertToList, ListResponse } from '@/lib/types/list';
import {
    getTeamStatusColor,
    Team,
    TeamStatus,
} from '@/types/team';
import { Badge, Heading, HStack, VStack } from '@chakra-ui/react';
import { FunctionComponent, ReactElement, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useLoaderData } from 'react-router-dom';
import { SimpleSuspense } from '@/components/simple-suspense';

export const TeamPage: FunctionComponent = (): ReactElement => {
    const data = useLoaderData() as Team;
    const [team, setTeam] = useState<Team>({} as Team);
    const { t } = useTranslation();
    const [games, setGames] = useState<Game[] | undefined>(undefined);
    const [stats, setStats] = useState<TeamStat[] | undefined>(undefined);

    useEffect(() => {
        setTeam(data);
    }, [data]);

    useEffect(() => {
        const loadStats = async () => {
            const response = await callGetTeamStats<GetTeamStatsResponse[]>(data.Id);
            if (response.success) {            
                if (response.data)    
                    setStats(convertToTeamStatList(response.data));
            }
        };

        const loadGames = async () => {
            const response = await callGetGamesWithTeam<ListResponse<GetGameResponse>>(data.Id);
            if (response.success) {
                setGames(convertToGameList(convertToList(response.data)?.List));
            }
        };

        loadGames();
        loadStats();
    }, [data?.Id]);

    return (
        <VStack spacing={5} align="start">
            <HStack spacing={5}>
                <Heading as="h2" size="md">
                    {t('Team')} "{team.Name}"
                </Heading>
                {team.Status !== TeamStatus.Active ? (
                    <Badge
                        colorScheme={getTeamStatusColor(team.Status)}
                        padding={4}
                    >
                        {t('TeamStatus.' + team.Status)}
                    </Badge>
                ) : (
                    ''
                )}
            </HStack>
            <VStack spacing={5} align="start" paddingLeft={2}>
                <Subheader marginTop={0}>{t('Teams.Players')}</Subheader>
                <SimpleSuspense fallback={t('Loading')} emptyText={t('Empty')}>
                    {team.Players?.map((item, index) => (
                        <HStack spacing={2} key={index} paddingLeft={4}>
                            <CustomLink
                                link={`/player/${item.Username}`}
                                text={item.Username}
                            />
                        </HStack>
                    ))}
                </SimpleSuspense>

                <Subheader>{t('Teams.Stats')}</Subheader>
                <SimpleSuspense fallback={t('Loading')} emptyText={t('Empty')}>
                    {stats && <StatsTable stats={stats} />}
                </SimpleSuspense>                

                <Subheader>{t('Games.Title')}</Subheader>
                <SimpleSuspense fallback={t('Loading')} emptyText={t('Empty')}>
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
                                        getGameResultFor(item, team.Id)
                                    )}
                                >
                                    {t('Games.' + getGameResultFor(item, team.Id))}
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
