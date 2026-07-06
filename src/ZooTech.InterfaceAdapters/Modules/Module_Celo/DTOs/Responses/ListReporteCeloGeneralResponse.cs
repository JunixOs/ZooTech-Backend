using ZooTech.InterfaceAdapters.DTOs;

namespace ZooTech.InterfaceAdapters.Modules.Module_Celo.DTOs.Responses;

public sealed record ListReporteCeloGeneralResponse(
    IReadOnlyList<CeloReporteItemResponse> Data,
    PaginationResponse Pagination);
