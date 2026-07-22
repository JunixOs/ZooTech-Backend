using ZooTech.Application.Common.Pagination;
using ZooTech.Application.Modules.Module_Celo.UseCases.GetReporteCelos;

namespace ZooTech.Application.Modules.Module_Celo.UseCases.ListReporteCeloGeneral;

public sealed record ListReporteCeloGeneralOutput(PagedResult<CeloReporteItemDto> Result);
