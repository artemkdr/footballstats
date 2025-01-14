import config from '@/config/config';
import fetchJson from '@/lib/net/fetch-json';

const callApi = async <T>(
    endpoint: string,
    options: RequestInit = {}    
) => {        
    if (endpoint != null && !endpoint.startsWith('http')) {
        endpoint = `${config.API_URL}/${endpoint}`;
    }    
    return fetchJson<T>(endpoint, { ...options });
};

export default callApi;
