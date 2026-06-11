using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarReportesDisponibles;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.ReporteVacuno.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.ReporteVacuno.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers.ReporteVacuno;

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

