import callApi from '@/lib/api';

export const callUpdatePlayer = async (username: string, json: any) => {
    return callApi(`users/${username}`, {
        method: 'PATCH',
        body: JSON.stringify(json),
        headers: { 'Content-Type': 'application/json' },
    });
};
