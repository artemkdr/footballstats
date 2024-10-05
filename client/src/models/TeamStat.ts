
export type TeamStat = {
    Id: number,   
    Name: string,
    Games: number,
    Wins: number,
    Losses: number,    
    GF: number,
    GA: number    
}

export const convertToTeamStat = (data : any) => {
    var stat = {} as TeamStat; 
    stat.Id = parseInt(data?.id);
    stat.Name = data?.name?.toString();
    stat.Games = parseInt(data?.games);
    stat.Wins = parseInt(data?.wins);
    stat.Losses = parseInt(data?.losses);
    stat.GF = parseInt(data?.gf);
    stat.GA = parseInt(data?.ga);
    return stat;
}

export const convertDataToTeamStatList = (listData : any) => {
    let list = [] as TeamStat[];
    if (listData instanceof Array) {
        for (let i = 0; i < listData.length; i++) {				
            const o = convertToTeamStat(listData[i]);
            if (o != null && o.Id != null) {
                list.push(o);
            }
        }
    }
    return list;
};