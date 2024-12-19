import callApi from '@/lib/api';

export const callGetTeam = async (id: any) => {
    return callApi(`teams/${id}`);
};
