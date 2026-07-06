using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;

public sealed record ListCelosResponse(
    IReadOnlyList<CeloItemResponse> Data,
    PaginationResponse Pagination);
