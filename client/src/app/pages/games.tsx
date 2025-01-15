import { CustomLink } from '@/components/custom-link';
import { Subheader } from '@/components/subheader';
import config from '@/config/config';
import { CreateNewGameModal } from '@/features/games/components/modal-game';
import { SelectTeam } from '@/features/games/components/select-team';
import {
    convertToRivalStats,
    RivalStats,
} from '@/features/games/types/rival-stats';
import { callGetRivalStats } from '@/features/stats/api/get-rival-stats';
import { callGetActiveTeams } from '@/features/teams/api/get-teams';
import { Game, GameStatus } from '@/types/game';
import { convertToList, List } from '@/lib/types/list';
import { convertToTeamList, Team } from '@/types/team';
import {
    Button,
    Heading,
    HStack,
    IconButton,
    Text,
    VStack,
} from '@chakra-ui/react';
import { ChangeEvent, ReactElement, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { MdClear } from 'react-icons/md';
import { useLoaderData, useNavigate, useSearchParams } from 'react-router-dom';
import { simulateGames } from '@/features/games/utils/simulate-games';
import { SimpleSuspense } from '@/components/simple-suspense';

export const Games = (): ReactElement => {
    const { t } = useTranslation();
    const data = useLoaderData() as List<Game>;
    const navigate = useNavigate();    
    const [isNewGameModalOpen, setIsNewGameModalOpen] = useState(false);
    const [teams, setTeams] = useState<Team[]>([] as Team[]);
    
    const [searchParams, setSearchParams] = useSearchParams();
    const [team1, setTeam1] = useState<number>(Number(searchParams.get('team1')) || -1);
    const [team2, setTeam2] = useState<number>(Number(searchParams.get('team2')) || -1);
    
    const [rivalStats, setRivalStats] = useState<RivalStats>({} as RivalStats);
    const [gamesList, setGamesList] = useState<List<Game> | undefined>(undefined);

    useEffect(() => {      
        setGamesList(data);    
        
        if (team1 > 0 && team2 > 0) {            
            const loadRivalStats = async () => {
                const response = await callGetRivalStats(team1, team2);
                if (response.success) {                    
                    setRivalStats(convertToRivalStats(response.data));
                }
            };
            loadRivalStats();
        }
    }, [data]);

    useEffect(() => {
        const loadTeams = async () => {
            const response = await callGetActiveTeams();
            if (response.success) {                
                setTeams(convertToTeamList(convertToList(response.data)?.List));
            }
        };

        loadTeams();
    }, []);

    useEffect(() => {
        const query = [];
        if (team1 > 0) query.push(`team1=${team1}`);
        if (team2 > 0) query.push(`team2=${team2}`);
        setGamesList(undefined);
        setRivalStats({} as RivalStats);
        navigate(query.length > 0 ? `?${query.join('&')}` : '');
    }, [team1, team2]);

    const handleChange = (event: ChangeEvent<HTMLSelectElement>) => {
        const { name, value } = event.target;
        switch (name) {
            case 'Team1':
                setTeam1(Number(value));                
                break;
            case 'Team2':
                setTeam2(Number(value));                
                break;
        }
    };

    

    return (
        <VStack spacing={5} align="start">
            <Heading as="h2" size="md">
                {t('Games.Title')} ({gamesList?.Total})
            </Heading>
            <HStack>
                <Button
                    colorScheme="green"
                    onClick={() => setIsNewGameModalOpen(true)}
                >
                    {t('Games.AddNewGame')}
                </Button>
                {config.SIMULATE_MODE ? (
                    <Button colorScheme="gray" onClick={() => simulateGames(teams)}>
                        {t('Games.SimulateGames')}
                    </Button>
                ) : (
                    ''
                )}
            </HStack>
            <VStack spacing={2} align={'start'}>
                <Subheader>{t('Games.Filter')}</Subheader>
                <HStack width={'100%'}>
                    <SelectTeam
                        teams={teams}
                        value={team1}
                        name={'Team1'}
                        placeholder={t('Games.Placeholder.Team1')}
                        textAlign={'right'}
                        onChange={handleChange}
                    />
                    <Text width={10} textAlign={'center'}>
                        {t('Games.TeamsDelimiter')}
                    </Text>
                    <SelectTeam
                        teams={teams}
                        value={team2}
                        name={'Team2'}
                        placeholder={t('Games.Placeholder.Team2')}
                        textAlign={'left'}
                        onChange={handleChange}
                    />
                    {team1 > 0 || team2 > 0 ? (
                        <IconButton
                            icon={<MdClear />}
                            aria-label="Clear teams"
                            onClick={() => {
                                setTeam1(-1);
                                setTeam2(-1);
                                setSearchParams({});
                            }}
                        />
                    ) : (
                        ''
                    )}
                </HStack>
                {team1 > 0 && team2 > 0 ? (
                    <HStack width={'100%'}>
                        <Text
                            textAlign={'right'}
                            fontSize={'2rem'}
                            width={'50%'}
                            fontWeight={'bold'}
                        >
                            {rivalStats.Wins1}
                        </Text>
                        <Text width={10} textAlign={'center'}>
                            {t('Games.ScoreDelimiter')}
                        </Text>
                        <Text
                            textAlign={'left'}
                            fontSize={'2rem'}
                            width={'50%'}
                            marginRight={12}
                            fontWeight={'bold'}
                        >
                            {rivalStats.Wins2}
                        </Text>
                    </HStack>
                ) : (
                    ''
                )}
            </VStack>
            <VStack spacing={5} align="left" paddingLeft={3}>
                <SimpleSuspense fallback={t('Loading')} emptyText={t('Empty')}>
                    {gamesList?.List?.map((item, index) => (
                        <HStack spacing={2} key={index}>
                            <CustomLink link={`/game/${item.Id}`}>
                                {t('GameTitle', {
                                    team1: item.Team1?.Name,
                                    team2: item.Team2?.Name,
                                    goals1: item.Goals1,
                                    goals2: item.Goals2,
                                })}
                            </CustomLink>
                            {item.Status !== GameStatus.Completed ? (
                                <Text>({t('GameStatus.' + item.Status)})</Text>
                            ) : (
                                ''
                            )}
                        </HStack>
                    ))}
                </SimpleSuspense>                
            </VStack>
            <CreateNewGameModal
                isOpen={isNewGameModalOpen}
                onClose={() => setIsNewGameModalOpen(false)}
                teams={teams}
            />
        </VStack>
    );
};
