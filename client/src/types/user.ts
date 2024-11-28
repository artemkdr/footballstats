export enum UserStatus {
    Active = "Active",
    Deleted = "Deleted"
}

export interface User {
    Username: string,
    Status: UserStatus,
    Vars: any,
    CreateDate: Date,
    ModifyDate: Date
}

export const convertToUser = (data : any) => {
    const user = {} as User;    
    user.Username = data?.username?.toString();
    user.Status = data?.status as UserStatus;    
    user.Vars = data?.vars;
    user.CreateDate = new Date(data?.createDate);
    user.ModifyDate = new Date(data?.modifyDate);
    return user;
}

export const getUserStatusColor = (status: UserStatus) : string => {
    switch (status) {        
        case UserStatus.Active:
            return "green";        
        case UserStatus.Deleted:
            return "gray";				
    }
    return "";
}

export const convertDataToUserList = (listData : any) => {
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
}