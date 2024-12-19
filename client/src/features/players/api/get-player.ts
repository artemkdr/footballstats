import callApi from '@/lib/api';

export const callGetPlayer = async (id: any) => {
    return callApi(`users/${id}`);
};
