using System.Globalization;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.Common;

public sealed record ReporteVacunoDateRange(DateOnly FechaDesde, DateOnly FechaHasta);

public static class ReporteVacunoDateRangeResolver
{
    public static ReporteVacunoDateRange Resolve(
        string? fechaDesde,
        string? fechaHasta,
        DateOnly serverToday,
        int defaultDays,
        string dateFormat)
    {
        var hasta = ParseDate(fechaHasta, "fechaHasta", dateFormat) ?? serverToday;
        var desde = ParseDate(fechaDesde, "fechaDesde", dateFormat) ?? hasta.AddDays(-defaultDays);

        if (desde > hasta)
        {
            throw new ArgumentException("fechaDesde no puede ser mayor que fechaHasta.", "fechaDesde");
        }

        return new ReporteVacunoDateRange(desde, hasta);
    }

    private static DateOnly? ParseDate(string? value, string field, string dateFormat)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (DateOnly.TryParseExact(
            value.Trim(),
            dateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var date))
        {
            return date;
        }

        throw new ArgumentException($"La fecha debe usar formato {dateFormat}.", field);
    }
}

