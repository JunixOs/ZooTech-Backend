namespace ZooTech.InterfaceAdapters.DTOs;

public class PagedResponse<T> : GeneralResponseDTO<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Limit { get; set; }
    public int TotalCount { get; set; }

    public static PagedResponse<T> OkPaged(T data, int page, int pageSize, int totalCount)
    {
        return new PagedResponse<T>
        {
            Success = true,
            Data = data,
            Page = page,
            PageSize = pageSize,
            Limit = pageSize,
            TotalCount = totalCount
        };
    }
}
