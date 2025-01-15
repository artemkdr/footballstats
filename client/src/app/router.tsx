import { Dashboard } from '@/app/pages/dashboard';
import { NotFound } from '@/app/pages/errors/not-found';
import { RouterError } from '@/app/pages/errors/router-error';
import { GamePage } from '@/app/pages/game';
import { Games } from '@/app/pages/games';
import { PlayerPage } from '@/app/pages/player';
import { Players } from '@/app/pages/players';
import { TeamPage } from '@/app/pages/team';
import { Teams } from '@/app/pages/teams';
import { callGetGame } from '@/features/games/api/get-game';
import { callGetGames } from '@/features/games/api/get-games';
import { Layout } from '@/features/layout/layout';
import { callGetPlayer } from '@/features/players/api/get-player';
import { callGetPlayers } from '@/features/players/api/get-players';
import { callGetStats } from '@/features/stats/api/get-stats';
import { convertToTeamStatList, GetTeamStatsResponse, TeamStat } from '@/features/stats/types/team-stat';
import { callGetTeam } from '@/features/teams/api/get-team';
import { callGetTeams } from '@/features/teams/api/get-teams';
import { convertToList, List, ListResponse } from '@/lib/types/list';
import { toInt } from '@/lib/utils/converters';
import { convertToGame, Game, GetGameResponse } from '@/types/game';
import { convertToTeam, convertToTeamList, GetTeamResponse, Team } from '@/types/team';
import { convertToPlayer, convertToPlayerList, GetPlayerResponse, Player } from '@/types/player';
import { Router } from '@remix-run/router';
import { ErrorBoundary } from 'react-error-boundary';
import {
    createBrowserRouter,
    Navigate,
    RouterProvider,
} from 'react-router-dom';

export const AppRouter: React.FC = () => {
    const router: Router = createBrowserRouter([
        {
            path: '/',
            element: (
                <ErrorBoundary FallbackComponent={RouterError}>
                    <Layout />
                </ErrorBoundary>
            ),
            errorElement: <RouterError />,
            children: [
                {
                    index: true,
                    path: '/',
                    element: <Navigate to={'/dashboard'} replace={true} />,
                },
                {
                    path: '/dashboard',
                    element: <Dashboard />,
                    loader: async () : Promise<TeamStat[]> => {
                        const response = await callGetStats<GetTeamStatsResponse[]>();
                        if (response.success) {
                            return convertToTeamStatList(response.data || []);
                        }
                        throw new Error('LoaderError', {
                            cause: response.error?.status,
                        });                        
                    },
                },
                {
                    path: '/teams',
                    element: <Teams />,
                    loader: async () : Promise<Team[]> => {
                        const response = await callGetTeams<ListResponse<GetTeamResponse>>();
                        if (response.success) {
                            return convertToTeamList(convertToList(response.data).List);
                        }
                        throw new Error('LoaderError', {
                            cause: response.error?.status,
                        });
                    },
                },
                {
                    path: '/team/:id',
                    element: <TeamPage />,
                    loader: async ({ params }) : Promise<Team> => {                        
                        const response = await callGetTeam<GetTeamResponse>(params.id);
                        if (response.success) {
                            return convertToTeam(response.data);
                        }
                        throw new Error('LoaderError', {
                            cause: response.error?.status,
                        });
                    },
                },
                {
                    path: '/players',
                    element: <Players />,
                    loader: async () : Promise<Player[]> => {
                        const response = await callGetPlayers<ListResponse<GetPlayerResponse>>();
                        if (response.success) {
                            return convertToPlayerList(convertToList(response.data)?.List);
                        }
                        throw new Error('LoaderError', {
                            cause: response.error?.status,
                        });
                    },
                },
                {
                    path: '/player/:id',
                    element: <PlayerPage />,
                    loader: async ({ params }) : Promise<Player> => {
                        const response = await callGetPlayer<GetPlayerResponse>(params.id);
                        if (response.success) {
                            return convertToPlayer(response.data);
                        }
                        throw new Error('LoaderError', {
                            cause: response.error?.status,
                        });
                    },
                },
                {
                    path: '/games',
                    element: <Games />,
                    loader: async ({ request }) : Promise<List<Game>> => {
                        const url = new URL(request.url);
                        const team1 = url.searchParams.get('team1');
                        const team2 = url.searchParams.get('team2');
                        const teams : number[] = [];
                        if (toInt(team1) > 0) teams.push(toInt(team1));
                        if (toInt(team2) > 0) teams.push(toInt(team2));
                        const response = await callGetGames<ListResponse<GetGameResponse>>(...teams);
                        if (response.success) {                            
                            return convertToList<Game>(response.data, convertToGame);
                        }
                        throw new Error('LoaderError', {
                            cause: response.error?.status,
                        });                        
                    },
                },
                {
                    path: '/game/:id',
                    element: <GamePage />,
                    loader: async ({ params }) : Promise<Game> => {
                        const response = await callGetGame<GetGameResponse>(params.id as string);
                        if (response.success) {
                            return convertToGame(response.data);
                        }
                        throw new Error('LoaderError', {
                            cause: response.error?.status,
                        });
                    },
                },
                {
                    path: '*',
                    element: <NotFound />,
                },
            ],
        },
    ]);
    return <RouterProvider router={router} />;
};
