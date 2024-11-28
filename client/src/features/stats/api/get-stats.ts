import callApi from '@/lib/api';

export const callGetStats = async () => {
    return callApi(`stats`);
};

export const callGetTeamStats = async (id: number) => {
    return callApi(`stats?team=${id}`);
};
