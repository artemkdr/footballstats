import callApi from '@/lib/api';

export const callGetTeams = async (status?: string) => {
    if (status !== undefined) {
        return callApi(`teams?status=${status}`);
    }
    return callApi(`teams`);
};

export const callGetTeamsWithStatus = async (status: string) => {
    return callApi(`teams?status=${status}`);
};

export const callGetActiveTeams = async () => {
    return callGetTeamsWithStatus('Active');
};

export const callGetTeamsWithPlayers = async (players: string) => {
    return callApi(`teams?players=${players}`);
};
