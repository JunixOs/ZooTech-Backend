using ZooTech.InterfaceAdapters.DTOs;

namespace ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Responses;

public sealed record ListOrdeniosResponse(
    IReadOnlyList<OrdenioListResponse> Data,
    PaginationResponse Pagination);
