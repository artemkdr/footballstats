const UNKNOWN_ERROR_MESSAGE = 'An unknown error occurred';
const UNKNOWN_ERROR_STATUS = 500;
const API_ERROR = 'Failed to fetch data from the API';  


interface FetchJsonResponse<T> {
    data: T | undefined;
    success: boolean;
    error?: {
        status: number;
        message: string;
    };
}

/**
 * Fetches JSON data from the specified endpoint.
 * Cacthes any errors that may occur during the fetch request and returns an error in the response object as FetchJsonResponse.
 * It expects the server to return a JSON object like `{"error": "User not found"}` in case of an error, "error" field could be also named "message" or "reason".
 *
 * @template T - The expected type of the response data.
 * @param {string} endpoint - The URL endpoint to fetch data from.
 * @param {RequestInit} [options={}] - Optional configuration for the fetch request.
 * @returns {Promise<FetchJsonResponse<T>>} A promise that resolves to a FetchJsonResponse object containing the data, success status, and error information if any.
 *
 * @example
 * ```typescript
 * interface User {
 *   id: number;
 *   name: string;
 * }
 *
 * const response = await fetchJson<User>('/api/user/1', { method: 'GET' });
 * if (response.success) {
 *   console.log(response.data);
 * } else {
 *   console.error(response.error?.status, response.error?.message);
 * }
 * ```
 */
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
        let errorMessage = knownError.message;
        try {
            const errorJson = await errorResponse.json();
            // expects that the server returns a JSON object with a ('message' | 'error' | 'reason') field
            errorMessage = errorJson.message || errorJson.error || errorJson.reason || errorJson as string;
        } catch {
            errorMessage = await errorResponse.text();
        }
        if (errorMessage?.length === 0)
            errorMessage = UNKNOWN_ERROR_MESSAGE;
        return {
            success: false,
            data: undefined,
            error: {
                status: errorResponse.status,
                message: errorMessage
            }
        };
    }
};

export default fetchJson;