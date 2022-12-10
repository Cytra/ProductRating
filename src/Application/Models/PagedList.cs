namespace Application.Models;

public class PagedList<T>
{
    public IEnumerable<T>? Items { get; set; }
    public PagingModel? Paging { get; set; }
}

public class PagingModel
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}