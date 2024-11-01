import { Router } from "@remix-run/router";
import { createBrowserRouter, Navigate, RouterProvider } from "react-router-dom";
import { Layout } from "../Layout";
import { Dashboard } from "../pages/Dashboard";
import { NotFound } from "../pages/NotFound";
import { RouterError } from "../pages/RouterError";
import { ErrorBoundary } from "react-error-boundary";
import { Teams } from "../pages/Teams";
import { TeamPage } from "../pages/TeamPage";
import { Players } from "../pages/Players";
import { PlayerPage } from "../pages/PlayerPage";
import { Games } from "../pages/Games";
import { GamePage } from "../pages/GamePage";
import callApi from "../net/api";

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
					const response = await callApi(`stats`);
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
					const response = await callApi(`team`);
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
					const response = await callApi(`team/${params.id}`);
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
					const response = await callApi(`user`);
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
					const response = await callApi(`user/${params.id}`);
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
					const response = await callApi(`game`);
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
					const response = await callApi(`game/${params.id}`);
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