import callApi from '@/lib/api';

export const callCreateGame = async (gameJson: any) => {
    return callApi(`game`, {
        method: 'POST',
        body: JSON.stringify(gameJson),
        headers: { 'Content-Type': 'application/json' },
    });
};
