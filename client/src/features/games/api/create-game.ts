import callApi from '@/lib/net/api';

export const callCreateGame = async <T>(json: unknown) => {
    return await callApi<T>(`games`, {
        method: 'POST',
        body: JSON.stringify(json),
        headers: { 'Content-Type': 'application/json' },
    });
};
