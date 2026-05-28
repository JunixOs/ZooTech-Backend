using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;
using ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.Mappers;

public static class ListadoVacunosReporteMapper
{
    public static ListarReporteVacunosQuery ToApplicationQuery(ListadoVacunosReporteQueryDto dto)
    {
        return new ListarReporteVacunosQuery(
            dto.FechaDesde,
            dto.FechaHasta,
            dto.Q,
            dto.Raza,
            dto.Procedencia,
            dto.Estado,
            dto.AptoPara,
            dto.Formato,
            dto.Page,
            dto.Limit);
    }

    public static ListadoVacunosReporteResponseDto ToDto(ListadoVacunosReporteResponse response)
    {
        return new ListadoVacunosReporteResponseDto(
            response.Data.Select(item => new VacunoListadoItemDto(
                item.Id,
                item.Codigo,
                item.FechaRegistro,
                item.Nombre,
                item.Raza,
                item.Procedencia,
                item.Estado)).ToList(),
            new ReporteVacunoResumenDto(response.Resumen.TotalVacunos),
            new ReporteVacunoFiltrosDto(
                response.Filtros.FechaDesde,
                response.Filtros.FechaHasta,
                response.Filtros.Q,
                response.Filtros.Raza,
                response.Filtros.Procedencia,
                response.Filtros.Estado,
                response.Filtros.AptoPara,
                response.Filtros.Formato),
            response.DownloadUrl);
    }
}
