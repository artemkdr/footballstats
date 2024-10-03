import { convertDataToUserList, User } from "./User";

export type Team = {
    Id: number,   
    Name: string,
    Players: User[],
    CreateDate: Date,
    ModifyDate: Date,
    Status: TeamStatus    
}

export enum TeamStatus {
    Enabled = "Active",
    Deleted = "Deleted"
}

export const convertToTeam = (data : any) => {
    var team = {} as Team;    
    team.Id = parseInt(data.id);
    team.Name = data.name?.toString();
    team.Players = convertDataToUserList(data.players);
    team.CreateDate = new Date(data.createDate);
    team.ModifyDate = new Date(data.modifyDate);
    team.Status = data.status as TeamStatus;    
    return team;
}

export const convertDataToTeamList = (listData : any) => {
    let list = [] as Team[];
    if (listData instanceof Array) {
        for (let i = 0; i < listData.length; i++) {				
            const o = convertToTeam(listData[i]);
            if (o != null && o.Id != null) {
                list.push(o);
            }
        }
    }
    return list;
};

export const isValidTeam = (team: Team) => {    
    if (team == null || team.Id == null || team.Players == null || team.Name == null) return false;    
    return true;
}