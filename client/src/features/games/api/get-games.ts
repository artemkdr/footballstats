import callApi from '@/lib/net/api';

// pairs like "team1=1", "team2=2"
export const callGetGames = async <T>(pairs?: string[]) => {
    if (pairs !== undefined) {
        return await callApi<T>(`games?${pairs.join('&')}`);
    }
    return await callApi<T>(`games`);
};

export const callGetGamesWithPlayers = async <T>(players: string) => {
    return await callApi<T>(`games?players=${players}`);
};
