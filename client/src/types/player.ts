import { toString } from "@/lib/utils/converters";

export enum PlayerStatus {
    Active = 'Active',
    Deleted = 'Deleted',
}

export interface CreatePlayerResponse {
    username: string;
}

export interface UpdatePlayerResponse {
    username: string;
}

export interface GetPlayerResponse {
    username: string;
    status: PlayerStatus;
    vars: unknown;
    createDate: string;
    modifyDate: string;
}

export interface Player {
    Username: string;
    Status: PlayerStatus;
    Vars: unknown;
    CreateDate: Date;
    ModifyDate: Date;
}

export const convertToPlayer = (json: unknown): Player => {    
    const data = json as GetPlayerResponse;
    return {
        Username: toString(data.username),
        Status: data.status as PlayerStatus,
        Vars: data.vars,
        CreateDate: new Date(data.createDate),
        ModifyDate: new Date(data.modifyDate),
    };
};

export const getPlayerStatusColor = (status: PlayerStatus): string => {
    switch (status) {
        case PlayerStatus.Active:
            return 'green';
        case PlayerStatus.Deleted:
            return 'gray';
    }
    return '';
};

export const convertToPlayerList = (listData: unknown[]) => {
    const list = [] as Player[];
    if (listData instanceof Array) {
        for (const ol of listData) {
            const u = convertToPlayer(ol);
            if (u != null && u.Username != null) {
                list.push(u);
            }
        }
    }
    return list;
};

export const USERNAME_PATTERN = /^[a-zA-Z0-9_-]{3,20}$/;

export const isValidPlayer = (player: Player) => {
    if (player == null || player.Username == null) return false;
    if (!USERNAME_PATTERN.test(player.Username)) return false;
    return true;
};
