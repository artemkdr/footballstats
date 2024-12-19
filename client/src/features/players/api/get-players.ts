import callApi from '@/lib/api';

export const callGetPlayers = async () => {
    return callApi(`users`);
};

export const callGetActivePlayers = async () => {
    return callApi(`users?status=Active`);
};
