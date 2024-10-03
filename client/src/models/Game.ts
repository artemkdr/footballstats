import { convertToTeam, Team } from "./Team";
import { convertDataToUserList, User } from "./User";

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
    game.Id = parseInt(data.id);
    game.Team1 = data.team1 != null ? convertToTeam(data.team1) : {} as Team;
    game.Team2 = data.team2 != null ? convertToTeam(data.team2) : {} as Team;
    game.Goals1 = parseInt(data.goals1);
    game.Goals2 = parseInt(data.goals2);
    game.Vars = data.vars;    
    game.CreateDate = new Date(data.createDate);
    game.ModifyDate = new Date(data.modifyDate);
    game.CompleteDate = new Date(data.completeDate);
    game.Status = data.status as GameStatus;    
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

export const isValidGame = (game: Game) => {    
    if (game == null || game.Id == null || game.Team1 == null || game.Team2 == null) return false;    
    return true;
}