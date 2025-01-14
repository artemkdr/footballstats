import callApi from '@/lib/net/api';

export const callGetStats = async <T>() => {
    return await callApi<T>(`stats`);
};

export const callGetTeamStats = async <T>(id: number) => {
    return await callApi<T>(`stats?team=${id}`);
};
