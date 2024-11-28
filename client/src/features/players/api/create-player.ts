import callApi from '@/lib/api';

export const callCreatePlayer = async (json: any) => {
    return callApi('user', {
        method: 'POST',
        body: JSON.stringify(json),
        headers: { 'Content-Type': 'application/json' },
    });
};
