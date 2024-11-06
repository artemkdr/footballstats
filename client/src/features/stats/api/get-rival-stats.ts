import callApi from "@/lib/api";

export const callGetRivalStats = async (team1 : number, team2 : number) => {
    return callApi(`statsrivals?team1=${team1}&team2=${team2}`);
}