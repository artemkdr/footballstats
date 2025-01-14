const UNKNOWN_ERROR_MESSAGE = 'An unknown error occurred';
const UNKNOWN_ERROR_STATUS = 500;
const API_ERROR = 'Failed to fetch data from the API';  

interface FetchJsonResponse<T> {
    data: T | undefined | null;
    success: boolean;
    error?: {
        status: number;
        message: string;
    };
}

const fetchJson = async <T>(
    endpoint: string,
    options: RequestInit = {} as RequestInit,
) : Promise<FetchJsonResponse<T>> => {      
    try {
        const response = await fetch(endpoint, options);        
        if (response.ok) {
            const json = await response.json();
            return {
                success: true,
                data: json as T,
            }
        } else {                        
            throw new Error(API_ERROR, { cause: response });
        }        
    } catch (error: unknown) {
        // extract the error message from the response or from the error object        
        const knownError = error as Error ?? new Error(UNKNOWN_ERROR_MESSAGE, { cause: {
            status: UNKNOWN_ERROR_STATUS
        }});
        const errorResponse = knownError.cause as Response ?? { text: () => knownError.message, status: UNKNOWN_ERROR_STATUS };
        // accepted bug: https://github.com/eslint/eslint/issues/19245
        // eslint-disable-next-line no-useless-assignment        
        let message = knownError.message;
        try {
            const errorJson = await errorResponse.json();
            // expects that the server returns a JSON object with a ('message' | 'error' | 'reason') field
            message = errorJson.message || errorJson.error || errorJson.reason || errorJson as string;
        } catch {
            message = await errorResponse.text();
        }
        return {
            success: false,
            data: null,
            error: {
                status: errorResponse.status,
                message: message
            }
        };
    }
};

export default fetchJson;