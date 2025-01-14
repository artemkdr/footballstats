import callApi from '@/lib/net/api';

export const callGetRivalStats = async <T>(team1: number, team2: number) => {
    return await callApi<T>(`statsrivals?team1=${team1}&team2=${team2}`);
};
