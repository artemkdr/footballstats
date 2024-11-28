import callApi from '@/lib/api';

export const callCreateTeam = async (json: any) => {
    return callApi(`team`, {
        method: 'POST',
        body: JSON.stringify(json),
        headers: { 'Content-Type': 'application/json' },
    });
};
