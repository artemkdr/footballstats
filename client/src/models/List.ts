
export type List = {
    Page: number;
    PageSize: number;
    TotalPages: number;
    Total: number;
    List: any[];
}

export const convertDataToList = (data : any) => {
    var list = {} as List;
    list.Page = parseInt(data?.page);
    list.PageSize = parseInt(data?.pageSize);
    list.TotalPages = parseInt(data?.totalPages);
    list.Total = parseInt(data?.total);
    list.List = data?.list instanceof Array ? data?.list : [] as any[];
    return list;
}