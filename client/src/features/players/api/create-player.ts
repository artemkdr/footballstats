import callApi from '@/lib/api';

export const callCreatePlayer = async (json: any) => {
    return callApi('users', {
        method: 'POST',
        body: JSON.stringify(json),
        headers: { 'Content-Type': 'application/json' },
    });
};
