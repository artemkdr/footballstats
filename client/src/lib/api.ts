import config from '@/config/config';

export const callApi = async (
    endpoint: string,
    options: object = {},
    token: string | null = null,
    toast: any = null
) => {
    const toastId =
        toast != null ? toast({ title: '...', status: 'info' }) : null;

    const apiToken = token || localStorage.getItem(config.TOKEN_KEY);
    if (endpoint != null && !endpoint.startsWith('http')) {
        endpoint = `${config.API_URL}/${endpoint}`;
    }
    const authData: object = apiToken
        ? { headers: { Authorization: `Bearer ${apiToken}` } }
        : {};
    const data: object = { ...authData, ...options };
    try {
        const response = await fetch(endpoint, data);
        if (toast != null && typeof toast.close == 'function')
            toast.close(toastId);
        if (response.ok) {
            return response;
        }
        if (response.status === 401) {
            // Unauthorized (token likely expired)
            // Optionally, redirect to login or handle token refresh here
            console.error('Token expired, please log in again');
            console.error(response);
        }
        return response;
    } catch (error: any) {
        return {
            status: 501,
            message: error?.message,
            error: error,
            ok: false,
            json: () => {
                return JSON.parse('{}');
            },
        };
    }
};

export default callApi;
