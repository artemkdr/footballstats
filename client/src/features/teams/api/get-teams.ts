import callApi from "@/lib/api";

export const callGetTeams = async(status? : string) => {
    if (status !== undefined) {
        return callApi(`team?status=${status}`);
    }
    return callApi(`team`);
}

export const callGetTeamsWithStatus = async(status : string) => {    
    return callApi(`team?status=${status}`);    
}

export const callGetActiveTeams = async() => {
    return callGetTeamsWithStatus("Active");
}

export const callGetTeamsWithPlayers = async(players : string) => {
    return callApi(`team?players=${players}`);
}