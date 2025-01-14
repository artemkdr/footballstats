import callApi from '@/lib/net/api';

export const callCreatePlayer = async <T>(json: unknown) => {
    return await callApi<T>('users', {
        method: 'POST',
        body: JSON.stringify(json),
        headers: { 'Content-Type': 'application/json' },
    });
};
