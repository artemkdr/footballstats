import callApi from "@/lib/api";

export const callUpdatePlayer = async(username : string, json: any) => {
    return callApi(`user/${username}`, { method: 'POST', body: JSON.stringify(json), headers: { "Content-Type": "application/json" }});  
}