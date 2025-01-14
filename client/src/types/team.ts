import { toInt, toString } from '@/lib/utils/converters';
import { convertToUserList as convertToUserList, User } from '@/types/user';

export enum TeamStatus {
    Active = 'Active',
    Deleted = 'Deleted',
}


export interface CreateTeamResponse {
    id: number;
}

export interface UpdateTeamResponse {
    id: number;
}

export interface GetTeamResponse {
    id: number;
    name: string;
    players: number[];
    playerDetails: unknown[];
    status: string;
    createDate: string;
    modifyDate: string;
}

export interface Team {
    Id: number;
    Name: string;
    Players: User[];
    CreateDate: Date;
    ModifyDate: Date;
    Status: TeamStatus;
}

export const teamHasPlayer = (team: Team, username: string): boolean => {
    return team?.Players?.find((x) => x.Username === username) != null;
};

export const getTeamStatusColor = (status: TeamStatus): string => {
    switch (status) {
        case TeamStatus.Active:
            return 'green';
        case TeamStatus.Deleted:
            return 'gray';
    }
    return '';
};

export const convertToTeam = (json: unknown): Team => {
    const data = json as GetTeamResponse;
    return {
        Id: toInt(data.id),
        Name: toString(data.name),
        Players: convertToUserList(data.playerDetails ?? data.players),
        CreateDate: new Date(data.createDate),
        ModifyDate: new Date(data.modifyDate),
        Status: data.status as TeamStatus,
    };
};

export const convertToTeamList = (listData: unknown[]) => {
    const list = [] as Team[];
    if (listData instanceof Array) {
        for (const ol of listData) {
            const o = convertToTeam(ol);
            if (o != null && o.Id != null) {
                list.push(o);
            }
        }
    }
    return list;
};

export const TEAMNAME_PATTERN = /^[a-zA-Z0-9_\-\s]{3,20}$/;

export const isValidTeam = (team: Team) => {
    if (
        team == null ||
        team.Id == null ||
        team.Players == null ||
        team.Name == null
    )
        return false;
    if (!TEAMNAME_PATTERN.test(team.Name)) return false;
    return true;
};
