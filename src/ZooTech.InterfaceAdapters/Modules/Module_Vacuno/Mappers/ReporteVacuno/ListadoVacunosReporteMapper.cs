using ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.ReporteVacuno.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.DTOs.ReporteVacuno.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Mappers.ReporteVacuno;

public static class ListadoVacunosReporteMapper
{
    public static ListadoVacunosReporteResponseDto ToDto(ListarVacunosOutput response)
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

