export type User = {
    Username: string,
    Status: UserStatus,
    Vars: any,
    CreateDate: Date,
    ModifyDate: Date
}

export enum UserStatus {
    Enabled = "Active",
    Deleted = "Deleted"
}

export const convertToUser = (data : any) => {
    var user = {} as User;    
    user.Username = data.username?.toString();
    user.Status = data.status as UserStatus;    
    user.Vars = data.vars;
    user.CreateDate = new Date(data.createDate);
    user.ModifyDate = new Date(data.modifyDate);
    return user;
}

export const convertDataToUserList = (listData : any) => {
    let list = [] as User[];
    if (listData instanceof Array) {
        for (let i = 0; i < listData.length; i++) {				
            const u = convertToUser(listData[i]);
            if (u != null && u.Username != null) {
                list.push(u);
            }
        }
    }
    return list;
};

export const isValidUser = (user: User) => {    
    if (user == null || user.Username == null) return false;    
    return true;
}