import { toInt } from "@/lib/utils/converters";

export interface List<T = unknown> {
    Page: number;
    PageSize: number;
    TotalPages: number;
    Total: number;
    List: T[];
}

export interface ListResponse<T = unknown> {
    page: number;
    pageSize: number;
    totalPages: number;
    total: number;
    list: T[];
}

export const convertToList = <T = unknown>(json: unknown, itemConverter?: <T>(item: unknown) => T): List<T> => {    
    const data = json as ListResponse; // it doesn't cast the json to ListResponse<T> type, it just tells the compiler that json is of type ListResponse<T>
    return {
        Page: toInt(data?.page),
        PageSize: toInt(data?.pageSize),
        TotalPages: toInt(data?.totalPages),
        Total: toInt(data?.total),
        List: (itemConverter ? data?.list?.map(x => itemConverter(x)) : (data?.list) as T[]),
    } as List<T>;
};
