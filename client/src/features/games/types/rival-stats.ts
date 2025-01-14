import { toInt } from "@/lib/utils/converters";

export interface RivalStatsResponse {
    team1: number;
    team2: number;
    wins1: number;
    wins2: number;
}

export interface RivalStats {
    Team1: number;
    Team2: number;
    Wins1: number;
    Wins2: number;
}

export const convertToRivalStats = (json: unknown) : RivalStats => {
    const data = json as RivalStatsResponse;
    return {
        Team1: toInt(data?.team1),
        Team2: toInt(data?.team2),
        Wins1: toInt(data?.wins1),
        Wins2: toInt(data?.wins2)
    };
};
