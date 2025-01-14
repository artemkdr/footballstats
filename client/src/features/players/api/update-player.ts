import callApi from '@/lib/net/api';

export const callUpdatePlayer = async <T>(username: string, json: unknown) => {
    return await callApi<T>(`users/${username}`, {
        method: 'PATCH',
        body: JSON.stringify(json),
        headers: { 'Content-Type': 'application/json' },
    });
};
