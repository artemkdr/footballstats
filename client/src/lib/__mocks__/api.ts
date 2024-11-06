import moment from "moment";

export const callApi = async (endpoint: string, options: any = {}, token : string | null = null, toast : any = null) => {
    return Promise.resolve({ status: 200, ok: true, json: async() => ({  }) });            
    //return Promise.resolve({status: 404, message: "mock error", ok: false, json: async () => ({}) });    
}

export default callApi;