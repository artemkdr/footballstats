
export type RivalStats = {
    Team1: number,   
    Team2: number,   
    Wins1: number,   
    Wins2: number,   
}

export const convertDataToRivalStats = (data : any) => {
    var stat = {} as RivalStats; 
    stat.Team1 = parseInt(data.team1);
    stat.Team2 = parseInt(data.team2);
    stat.Wins1 = parseInt(data.wins1);
    stat.Wins2 = parseInt(data.wins2);    
    return stat;
}