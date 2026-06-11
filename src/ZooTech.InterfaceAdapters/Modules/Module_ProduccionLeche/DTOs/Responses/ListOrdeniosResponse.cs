namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Responses;

public sealed record ListOrdeniosResponse(
    IReadOnlyList<OrdenioResponse> Data,
    PaginationResponse Pagination);
