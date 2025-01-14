import callApi from '@/lib/net/api';

export const callGetGame = async <T>(id: number | string) => {
    return await callApi<T>(`games/${id}`);
};

export const callGetGamesWithTeam = async <T>(id: number | string) => {
    return await callApi<T>(`games?team1=${id}`);
};
