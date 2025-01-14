import callApi from '@/lib/net/api';

export const callCreateTeam = async <T>(json: unknown) => {
    return await callApi<T>(`teams`, {
        method: 'POST',
        body: JSON.stringify(json),
        headers: { 'Content-Type': 'application/json' },
    });
};
