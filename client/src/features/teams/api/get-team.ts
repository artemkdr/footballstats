import callApi from '@/lib/net/api';

export const callGetTeam = async <T>(id: unknown) => {
    return callApi<T>(`teams/${id}`);
};
