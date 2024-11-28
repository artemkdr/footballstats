import callApi from '@/lib/api';

export const callGetTeam = async (id: any) => {
    return callApi(`team/${id}`);
};
