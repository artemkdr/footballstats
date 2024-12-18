import { convertDataToUserList, User } from '@/types/user';

export enum TeamStatus {
    Active = 'Active',
    Deleted = 'Deleted',
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

export const convertToTeam = (data: any) => {
    const team = {} as Team;
    team.Id = parseInt(data?.id);
    team.Name = data?.name?.toString();
    team.Players = convertDataToUserList(data?.playerDetails ?? data?.players);
    team.CreateDate = new Date(data?.createDate);
    team.ModifyDate = new Date(data?.modifyDate);
    team.Status = data?.status as TeamStatus;
    return team;
};

export const convertDataToTeamList = (listData: any) => {
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
