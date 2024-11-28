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
import { callGetTeam } from '@/features/teams/api/get-team';
import { callGetTeams } from '@/features/teams/api/get-teams';
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
                    loader: async () => {
                        const response = await callGetStats();
                        if (response.ok) {
                            const json = await response.json();
                            return json;
                        }
                        throw new Error('LoaderError', {
                            cause: response.status,
                        });
                    },
                },
                {
                    path: '/teams',
                    element: <Teams />,
                    loader: async () => {
                        const response = await callGetTeams();
                        if (response.ok) {
                            const json = await response.json();
                            return json;
                        }
                        throw new Error('LoaderError', {
                            cause: response.status,
                        });
                    },
                },
                {
                    path: '/team/:id',
                    element: <TeamPage />,
                    loader: async ({ params }) => {
                        const response = await callGetTeam(params.id);
                        if (response.ok) {
                            const json = await response.json();
                            return json;
                        }
                        throw new Error('LoaderError', {
                            cause: response.status,
                        });
                    },
                },
                {
                    path: '/players',
                    element: <Players />,
                    loader: async () => {
                        const response = await callGetPlayers();
                        if (response.ok) {
                            const json = await response.json();
                            return json;
                        }
                        throw new Error('LoaderError', {
                            cause: response.status,
                        });
                    },
                },
                {
                    path: '/player/:id',
                    element: <PlayerPage />,
                    loader: async ({ params }) => {
                        const response = await callGetPlayer(params.id);
                        if (response.ok) {
                            const json = await response.json();
                            return json;
                        }
                        throw new Error('LoaderError', {
                            cause: response.status,
                        });
                    },
                },
                {
                    path: '/games',
                    element: <Games />,
                    loader: async () => {
                        const response = await callGetGames();
                        if (response.ok) {
                            const json = await response.json();
                            return json;
                        }
                        throw new Error('LoaderError', {
                            cause: response.status,
                        });
                    },
                },
                {
                    path: '/game/:id',
                    element: <GamePage />,
                    loader: async ({ params }) => {
                        const response = await callGetGame(params.id);
                        if (response.ok) {
                            const json = await response.json();
                            return json;
                        }
                        throw new Error('LoaderError', {
                            cause: response.status,
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
