import { convertToTeam, Team, teamHasPlayer } from "@/types/team";

export type Game = {
    Id: number;        
    Team1: Team;
    Team2: Team;
    Goals1: number;
    Goals2: number;    
    Vars: any;
    CreateDate: Date;
    ModifyDate: Date;
    CompleteDate: Date;
    Status: GameStatus
}

export enum GameStatus {
    NotStarted = "NotStarted",
    Playing = "Playing", 
    Completed = "Completed",
    Cancelled = "Cancelled"
}

export const convertToGame = (data : any) => {
    var game = {} as Game;    
    game.Id = parseInt(data?.id);
    game.Team1 = data?.team1 != null ? convertToTeam(data?.team1Detail ?? data?.team1) : {} as Team;
    game.Team2 = data?.team2 != null ? convertToTeam(data?.team2Detail ?? data?.team2) : {} as Team;
    game.Goals1 = parseInt(data?.goals1);
    game.Goals2 = parseInt(data?.goals2);
    game.Vars = data?.vars;    
    game.CreateDate = new Date(data?.createDate);
    game.ModifyDate = new Date(data?.modifyDate);
    game.CompleteDate = new Date(data?.completeDate);
    game.Status = data?.status as GameStatus;    
    return game;
}

export const convertDataToGameList = (listData : any) => {
    let list = [] as Game[];
    if (listData instanceof Array) {
        for (let i = 0; i < listData.length; i++) {				
            const o = convertToGame(listData[i]);
            if (o != null && o.Id != null) {
                list.push(o);
            }
        }
    }
    return list;
};

export const getGameStatusColor = (status: GameStatus) : string => {
    switch (status) {
        case GameStatus.NotStarted:
            return "yellow";				
        case GameStatus.Playing:
            return "green";
        case GameStatus.Completed:
            return "blue";
        case GameStatus.Cancelled:
            return "gray";				
    }
    return "";
}

export enum GameResult {
    Win = "Win",
    Loss = "Loss", 
    Draw = "Draw",
    None = "None"
}

export const getGameColorForResult = (result: GameResult) : string => {
    switch (result) {
        case GameResult.Win:
            return "green";
        case GameResult.Loss:
            return "red";        
    }
    return "gray";
}

export const getGameResultFor = (game: Game, teamId: number) : GameResult => {
    if (game.Status === GameStatus.Completed) {
        if ((game.Team1?.Id === teamId && game.Goals1 > game.Goals2) || (game.Team2?.Id === teamId && game.Goals2 > game.Goals1))
            return GameResult.Win;    
        
        if ((game.Team1?.Id === teamId && game.Goals1 < game.Goals2) || (game.Team2?.Id === teamId && game.Goals2 < game.Goals1)) 
            return GameResult.Loss;    

        return GameResult.Draw;
    }
    return GameResult.None;
}

export const getGameResultForUser = (game: Game, username: string) : GameResult => {
    if (game.Status === GameStatus.Completed) {
        if ((teamHasPlayer(game.Team1, username) && game.Goals1 > game.Goals2) || (teamHasPlayer(game.Team2, username) && game.Goals2 > game.Goals1))
            return GameResult.Win;    
        
        if ((teamHasPlayer(game.Team1, username) && game.Goals1 < game.Goals2) || (teamHasPlayer(game.Team2, username) && game.Goals2 < game.Goals1)) 
            return GameResult.Loss;    

        return GameResult.Draw;
    }
    return GameResult.None;
}


export const isValidGame = (game: Game) => {    
    if (game == null || game.Id == null || game.Team1 == null || game.Team2 == null || game.Team1?.Id === game.Team2?.Id) return false;    
    return true;
}