using System.Globalization;
using ZooTech.Application.Common.Validator;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ListarVacunosReporte;

public sealed class ListarVacunosReporteQueryValidator : ICommandQueryValidator<ListarVacunosReporteQuery>
{
    public ModuleName ModuleName => ModuleName.Vacuno;

    public List<string> Validate(ListarVacunosReporteQuery request)
    {
        var errors = new List<string>();
        var formato = Normalize(request.Formato) ?? "json";

        if (formato is not ("json" or "excel" or "pdf"))
        {
            errors.Add("El formato debe ser json, excel o pdf.");
        }

        var fechaDesde = ParseDate(request.FechaDesde, "fechaDesde", errors);
        var fechaHasta = ParseDate(request.FechaHasta, "fechaHasta", errors);
        ParseDate(request.FechaRegistro, "fechaRegistro", errors);

        if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde > fechaHasta)
        {
            errors.Add("La fecha desde no puede ser posterior a la fecha hasta.");
        }

        return errors;
    }

    private static DateOnly? ParseDate(string? value, string fieldName, ICollection<string> errors)
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

        errors.Add($"El campo {fieldName} debe tener una fecha válida.");
        return null;
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();
}
