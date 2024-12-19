import callApi from '@/lib/api';

// pairs like "team1=1", "team2=2"
export const callGetGames = async (pairs?: string[]) => {
    if (pairs !== undefined) {
        return callApi(`games?${pairs.join('&')}`);
    }
    return callApi(`games`);
};

export const callGetGamesWithPlayers = async (players: string) => {
    return callApi(`games?players=${players}`);
};
