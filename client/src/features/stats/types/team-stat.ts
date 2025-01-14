import { toInt, toString } from "@/lib/utils/converters";

export interface TeamStat {
    Id: number;
    Name: string;
    Games: number;
    Wins: number;
    Losses: number;
    GF: number;
    GA: number;
}

export interface GetTeamStatsResponse {
    id: number;
    name: string;
    games: number;
    wins: number;
    losses: number;
    gf: number;
    ga: number;    
}


export const convertToTeamStat = (json: unknown): TeamStat => {
    const data = json as GetTeamStatsResponse;
    return {
        Id: toInt(data?.id),
        Name: toString(data?.name),
        Games: toInt(data?.games),
        Wins: toInt(data?.wins),
        Losses: toInt(data?.losses),
        GF: toInt(data?.gf),
        GA: toInt(data?.ga),
    };
};

export const convertToTeamStatList = (listData: unknown[]) => {
    const list = [] as TeamStat[];
    if (listData instanceof Array) {
        for (const ol of listData) {
            const o = convertToTeamStat(ol);
            if (o != null && o.Id != null) {
                list.push(o);
            }
        }
    }
    return list;
};
