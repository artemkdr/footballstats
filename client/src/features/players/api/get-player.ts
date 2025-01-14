import callApi from '@/lib/net/api';

export const callGetPlayer = async <T>(id: unknown) => {
    return await callApi<T>(`users/${id}`);
};
