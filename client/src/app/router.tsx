import { Router } from "@remix-run/router";
import { createBrowserRouter, Navigate, RouterProvider } from "react-router-dom";
import { Layout } from "@/features/layout/layout"
import { Dashboard } from "@/app/pages/dashboard";
import { NotFound } from "@/app/pages/errors/not-found";
import { RouterError } from "@/app/pages/errors/router-error";
import { ErrorBoundary } from "react-error-boundary";
import { Teams } from "@/app/pages/teams";
import { TeamPage } from "@/app/pages/team";
import { Players } from "@/app/pages/players";
import { PlayerPage } from "@/app/pages/player";
import { Games } from "@/app/pages/games";
import { GamePage } from "@/app/pages/game";
import { callGetStats } from "@/features/stats/api/get-stats";
import { callGetTeams } from "@/features/teams/api/get-teams";
import { callGetPlayers } from "@/features/players/api/get-players";
import { callGetGames } from "@/features/games/api/get-games";
import { callGetGame } from "@/features/games/api/get-game";
import { callGetPlayer } from "@/features/players/api/get-player";
import { callGetTeam } from "@/features/teams/api/get-team";

export const router: Router = createBrowserRouter([
	{
		path: '/',
		element: <ErrorBoundary FallbackComponent={RouterError}><Layout /></ErrorBoundary>,
		errorElement: <RouterError />,				
		children: [
			{
				index: true,
				path: '/',
				element: <Navigate to={'/dashboard'} replace={true} />
			},
			{				
				path: '/dashboard',
				element: <Dashboard />,
				loader: async({ params }) => {											
					const response = await callGetStats();
					if (response.ok) {
						var json = await response.json();
						return json;
					}
					throw new Error("LoaderError", { cause: response.status });
				}
			},
			{				
				path: '/teams',
				element: <Teams />,
				loader: async({ request, params }) => {																
					const response = await callGetTeams();
					if (response.ok) {
						var json = await response.json();
						return json;
					}
					throw new Error("LoaderError", { cause: response.status });
				}
			},
			{				
				path: '/team/:id',
				element: <TeamPage />,
				loader: async({ params }) => {											
					const response = await callGetTeam(params.id);
					if (response.ok) {
						var json = await response.json();
						return json;
					}
					throw new Error("LoaderError", { cause: response.status });
				}
			},
			{				
				path: '/players',
				element: <Players />,
				loader: async({ params }) => {											
					const response = await callGetPlayers();
					if (response.ok) {
						var json = await response.json();
						return json;
					}
					throw new Error("LoaderError", { cause: response.status });
				}
			},
			{				
				path: '/player/:id',
				element: <PlayerPage />,
				loader: async({ params }) => {											
					const response = await callGetPlayer(params.id);
					if (response.ok) {
						var json = await response.json();
						return json;
					}
					throw new Error("LoaderError", { cause: response.status });
				}
			},
			{				
				path: '/games',
				element: <Games />,
				loader: async({ params }) => {											
					const response = await callGetGames();
					if (response.ok) {
						var json = await response.json();
						return json;
					}
					throw new Error("LoaderError", { cause: response.status });
				}
			},
			{				
				path: '/game/:id',
				element: <GamePage />,
				loader: async({ params }) => {											
					const response = await callGetGame(params.id);
					if (response.ok) {
						var json = await response.json();
						return json;
					}
					throw new Error("LoaderError", { cause: response.status });
				}
			},
			{
				path: '*',
				element: <NotFound />,
			},
		],
	},
]);

export const AppRouter: React.FC = () => {
	return (
		<RouterProvider router={router} />	   
	)
}