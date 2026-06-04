using System.Globalization;
using ZooTech.Application.Common.Exceptions;

namespace ZooTech.Application.Modules.Module_ReporteVacuno.Common;

public sealed record ReporteVacunoDateRange(DateOnly FechaDesde, DateOnly FechaHasta);

public static class ReporteVacunoDateRangeResolver
{
    public static ReporteVacunoDateRange Resolve(
        string? fechaDesde,
        string? fechaHasta,
        DateOnly serverToday)
    {
        var hasta = ParseDate(fechaHasta, "fechaHasta") ?? serverToday;
        var desde = ParseDate(fechaDesde, "fechaDesde") ?? hasta.AddDays(-30);

        if (desde > hasta)
        {
            throw new ApplicationRuleException(
                "VALIDATION_ERROR",
                "Los datos enviados no son validos.",
                [new ApplicationErrorDetail("fechaDesde", "fechaDesde no puede ser mayor que fechaHasta.")]);
        }

        return new ReporteVacunoDateRange(desde, hasta);
    }

    private static DateOnly? ParseDate(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (DateOnly.TryParseExact(
            value.Trim(),
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var date))
        {
            return date;
        }

        throw new ApplicationRuleException(
            "VALIDATION_ERROR",
            "Los datos enviados no son validos.",
            [new ApplicationErrorDetail(field, "La fecha debe usar formato YYYY-MM-DD.")]);
    }
}
