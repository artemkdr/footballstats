import callApi from '@/lib/net/api';

/**
 * 
 * @example
 * ```
 * callGetGames<GameResponse[]>("1", "2"); // team ids
 * callGetGames<GameResponse[]>(1, 2); // team ids
 * ```
 */
export const callGetGames = async <T>(...teams : number[]) => {
    const props = [];
    for (const team of teams) {        
        props.push(`team${props.length + 1}=${team}`);        
        if (props.length === 2) break;
    }
    return await callApi<T>(`games?${props.join('&')}`);
};

export const callGetGamesWithPlayers = async <T>(...players: string[]) => {
    return await callApi<T>(`games?players=${players.join(',')}`);
};
