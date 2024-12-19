import callApi from '@/lib/api';

export const callGetGame = async (id: any) => {
    return callApi(`games/${id}`);
};

export const callGetGamesWithTeam = async (id: number) => {
    return callApi(`games?team1=${id}`);
};
