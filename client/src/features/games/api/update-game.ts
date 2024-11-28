import callApi from "@/lib/api";

export const callUpdateGame = async(id: number, json: any) => {
    return callApi(`game/${id}`, { method: 'PATCH', body: JSON.stringify(json), headers: { "Content-Type": "application/json" }});
}