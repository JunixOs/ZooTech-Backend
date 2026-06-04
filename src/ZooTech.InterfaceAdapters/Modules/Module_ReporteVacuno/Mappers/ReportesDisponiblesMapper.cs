using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReportesDisponibles;
using ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.Mappers;

public static class ReportesDisponiblesMapper
{
    public static ListarReportesDisponiblesQuery ToApplicationQuery(ReportesDisponiblesQueryDto dto)
    {
        return new ListarReportesDisponiblesQuery(
            dto.FechaDesde,
            dto.FechaHasta,
            dto.Q);
    }

    public static ReportesDisponiblesResponseDto ToDto(ReportesDisponiblesResponse response)
    {
        return new ReportesDisponiblesResponseDto(
            response.Data.Select(item => new ReporteDisponibleDto(
                item.Tipo,
                item.Nombre,
                item.Descripcion)).ToList(),
            new ReportesDisponiblesFiltrosDto(
                response.Filtros.FechaDesde,
                response.Filtros.FechaHasta,
                response.Filtros.Q));
    }
}
