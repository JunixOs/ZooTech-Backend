using ZooTech.Application.Common.Exceptions;

namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte;

public sealed class ObtenerRegistroVacunoReporteUseCase : IObtenerRegistroVacunoReporteUseCase
{
    private static readonly string[] FormatosPermitidos = ["json", "pdf", "excel"];

    private readonly IRegistroVacunoReadRepository _repository;

    public ObtenerRegistroVacunoReporteUseCase(IRegistroVacunoReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<RegistroVacunoReporteResponse> HandleAsync(
        ObtenerRegistroVacunoReporteQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.VacunoId <= 0)
        {
            throw new ApplicationRuleException(
                "VALIDATION_ERROR",
                "Los datos enviados no son validos.",
                [new ApplicationErrorDetail("vacunoId", "El ID del vacuno debe ser mayor que cero.")]);
        }

        var formato = Normalize(query.Formato) ?? "json";
        EnsureFormatoValido(formato);

        if (formato is "pdf" or "excel")
        {
            throw new ApplicationRuleException(
                "REPORT_FORMAT_NOT_IMPLEMENTED",
                "La generacion de PDF y Excel pertenece a TK04/TK05.",
                [new ApplicationErrorDetail("formato", "Para TK03 solo esta implementado formato=json.")],
                501);
        }

        var detalle = await _repository.ObtenerRegistroAsync(query.VacunoId, cancellationToken);

        if (detalle is null)
        {
            throw new ApplicationRuleException(
                "VACUNO_NOT_FOUND",
                "No existe un vacuno con el ID enviado.",
                [],
                404);
        }

        return new RegistroVacunoReporteResponse(
            detalle,
            Array.Empty<object>(),
            null);
    }

    private static string? Normalize(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized.ToLowerInvariant();
    }

    private static void EnsureFormatoValido(string formato)
    {
        if (!FormatosPermitidos.Contains(formato, StringComparer.OrdinalIgnoreCase))
        {
            throw new ApplicationRuleException(
                "INVALID_REPORT_FORMAT",
                "El formato debe ser json, pdf o excel.",
                [new ApplicationErrorDetail("formato", "Formato no permitido.")]);
        }
    }
}
