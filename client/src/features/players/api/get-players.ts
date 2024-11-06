import callApi from "@/lib/api"

export const callGetPlayers = async() => {
    return callApi(`user`);
}

export const callGetActivePlayers = async() => {
    return callApi(`user?status=Active`);
}