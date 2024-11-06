import callApi from "@/lib/api";

export const callUpdateGame = async(id: number, json: any) => {
    return callApi(`game/${id}`, { method: 'POST', body: JSON.stringify(json), headers: { "Content-Type": "application/json" }});
}