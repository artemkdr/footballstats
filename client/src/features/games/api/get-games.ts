import callApi from '@/lib/api';

// pairs like "team1=1", "team2=2"
export const callGetGames = async (pairs?: string[]) => {
    if (pairs !== undefined) {
        return callApi(`game?${pairs.join('&')}`);
    }
    return callApi(`game`);
};

export const callGetGamesWithPlayers = async (players: string) => {
    return callApi(`game?players=${players}`);
};
