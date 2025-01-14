import callApi from '@/lib/net/api';

export const callGetPlayers = async <T>() => {
    return await callApi<T>(`users`);
};

export const callGetActivePlayers = async <T>() => {
    return await callApi<T>(`users?status=Active`);
};
