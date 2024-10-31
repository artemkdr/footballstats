namespace API.Data;

public class PagedList<T> {
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
    public List<T>? List { get; set; }
}