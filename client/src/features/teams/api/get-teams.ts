import callApi from '@/lib/net/api';

export const callGetTeams = async <T>(status?: string) => {
    if (status !== undefined) {
        return await callApi<T>(`teams?status=${status}`);
    }
    return await callApi<T>(`teams`);
};

export const callGetTeamsWithStatus = async <T>(status: string) => {
    return await callApi<T>(`teams?status=${status}`);
};

export const callGetActiveTeams = async <T>() => {
    return await callGetTeamsWithStatus<T>('Active');
};

export const callGetTeamsWithPlayers = async <T>(...players: string[]) => {
    return await callApi<T>(`teams?players=${players.join(',')}`);
};
