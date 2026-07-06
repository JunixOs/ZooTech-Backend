using ZooTech.InterfaceAdapters.DTOs;

namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;

public sealed record ListCelosResponse(
    IReadOnlyList<CeloItemResponse> Data,
    PaginationResponse Pagination);
