import callApi from '@/lib/net/api';

export const callUpdateGame = async <T>(id: number, json: unknown) => {
    return await callApi<T>(`games/${id}`, {
        method: 'PATCH',
        body: JSON.stringify(json),
        headers: { 'Content-Type': 'application/json' },
    });
};
