namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Responses;

public sealed record PaginationResponse(
    int Page,
    int Limit,
    int Total,
    int TotalPages);
