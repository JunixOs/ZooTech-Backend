namespace ZooTech.Application.Common.Models;

public class PaginationModel
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int Skip => (Page - 1) * PageSize;
}

public class PagedResult<T>
{
    public List<T> Data { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)Total / PageSize);
}
