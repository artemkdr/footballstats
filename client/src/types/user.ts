import { toString } from "@/lib/utils/converters";

export enum UserStatus {
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
    status: UserStatus;
    vars: unknown;
    createDate: string;
    modifyDate: string;
}

export interface User {
    Username: string;
    Status: UserStatus;
    Vars: unknown;
    CreateDate: Date;
    ModifyDate: Date;
}

export const convertToUser = (json: unknown): User => {    
    const data = json as GetPlayerResponse;
    return {
        Username: toString(data.username),
        Status: data.status as UserStatus,
        Vars: data.vars,
        CreateDate: new Date(data.createDate),
        ModifyDate: new Date(data.modifyDate),
    };
};

export const getUserStatusColor = (status: UserStatus): string => {
    switch (status) {
        case UserStatus.Active:
            return 'green';
        case UserStatus.Deleted:
            return 'gray';
    }
    return '';
};

export const convertToUserList = (listData: unknown[]) => {
    const list = [] as User[];
    if (listData instanceof Array) {
        for (const ol of listData) {
            const u = convertToUser(ol);
            if (u != null && u.Username != null) {
                list.push(u);
            }
        }
    }
    return list;
};

export const USERNAME_PATTERN = /^[a-zA-Z0-9_-]{3,20}$/;

export const isValidUser = (user: User) => {
    if (user == null || user.Username == null) return false;
    if (!USERNAME_PATTERN.test(user.Username)) return false;
    return true;
};
