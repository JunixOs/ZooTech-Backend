namespace ZooTech.Application.Common.Pagination;

public sealed record PagedResult<T>(IReadOnlyList<T> Data, int TotalCount, int Page, int PageSize);
