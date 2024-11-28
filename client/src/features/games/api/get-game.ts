import callApi from '@/lib/api';

export const callGetGame = async (id: any) => {
    return callApi(`game/${id}`);
};

export const callGetGamesWithTeam = async (id: number) => {
    return callApi(`game?team1=${id}`);
};
