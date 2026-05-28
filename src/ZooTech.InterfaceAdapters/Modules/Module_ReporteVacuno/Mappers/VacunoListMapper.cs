using System.Globalization;
using ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;
using ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.DTOs.Requests;
using ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.DTOs.Responses;

namespace ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno.Mappers;

public static class VacunoListMapper
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
            "json",
            dto.Page,
            dto.Limit);
    }

    public static VacunoListResponseDto ToDto(
        ListadoVacunosReporteResponse response,
        ListadoVacunosReporteQueryDto query)
    {
        var page = ParsePositiveIntOrDefault(query.Page, 1);
        var limit = Math.Min(ParsePositiveIntOrDefault(query.Limit, 20), 100);
        var total = response.Resumen.TotalVacunos;
        var totalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit);

        return new VacunoListResponseDto(
            response.Data.Select(item => new VacunoListadoItemDto(
                item.Id,
                item.Codigo,
                item.FechaRegistro,
                item.Nombre,
                item.Raza,
                item.Procedencia,
                item.Estado)).ToList(),
            new PaginationDto(page, limit, total, totalPages),
            new ReporteVacunoFiltrosDto(
                response.Filtros.FechaDesde,
                response.Filtros.FechaHasta,
                response.Filtros.Q,
                response.Filtros.Raza,
                response.Filtros.Procedencia,
                response.Filtros.Estado,
                response.Filtros.AptoPara,
                response.Filtros.Formato));
    }

    private static int ParsePositiveIntOrDefault(string? value, int defaultValue)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        return int.TryParse(value.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var parsed) && parsed > 0
            ? parsed
            : defaultValue;
    }
}
