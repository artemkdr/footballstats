import { toInt } from "@/lib/utils/converters";

/**
 * Represents a list of items for a paginated data model.
 *
 * @template T - The type of the items in the list.
 * 
 * @property {number} Page - The current page number.
 * @property {number} PageSize - The number of items per page.
 * @property {number} TotalPages - The total number of pages.
 * @property {number} Total - The total number of items.
 * @property {T[]} List - The list of items.
 */
export interface List<T = unknown> {
    Page: number;
    PageSize: number;
    TotalPages: number;
    Total: number;
    List: T[];
}

/**
 * Represents a list of items for a paginated response.
 *
 * @template T - The type of the items in the list.
 * 
 * @property {number} page - The current page number.
 * @property {number} pageSize - The number of items per page.
 * @property {number} totalPages - The total number of pages.
 * @property {number} total - The total number of items.
 * @property {T[]} list - The list of items.
 */
export interface ListResponse<T = unknown> {
    page: number;
    pageSize: number;
    totalPages: number;
    total: number;
    list: T[];
}

/**
 * Converts a JSON object to a `List` object.
 *
 * @template T - The type of the items in the list.
 * @param {unknown} json - The JSON object to convert.
 * @param {<T>(item: unknown) => T} [itemConverter] - An optional function to convert each item in the list.
 * @returns {List<T>} The converted `List` object.
 */
export const convertToList = <T = unknown>(json: unknown, itemConverter?: <T>(item: unknown) => T): List<T> => {    
    // it doesn't cast the json to ListResponse<T> type, it just tells the compiler that json is of type ListResponse<T>,
    // ideally we have to check if the json is of type ListResponse<T> and then cast it to ListResponse<T> type
    const data = json as ListResponse<T>; 
    return {
        Page: toInt(data?.page),
        PageSize: toInt(data?.pageSize),
        TotalPages: toInt(data?.totalPages),
        Total: toInt(data?.total),
        List: (itemConverter ? data?.list?.map(x => itemConverter(x)) : (data?.list) as T[]),
    } as List<T>;
};
