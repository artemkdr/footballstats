import callApi from '@/lib/net/api';

export const callGetTeam = async <T>(id: string | number) => {
    return callApi<T>(`teams/${id}`);
};
