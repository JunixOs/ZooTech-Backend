using System.Globalization;
using Microsoft.Extensions.Logging;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Time;

namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ListarReporteVacunos;

public sealed class ListarReporteVacunosUseCase : IListarReporteVacunosUseCase
{
    private static readonly string[] FormatosPermitidos = ["json", "pdf", "excel"];
    private static readonly string[] EstadosPermitidos = ["vivo", "muerto"];
    private static readonly string[] AptosPermitidos = ["produccion_leche", "carne", "reproduccion"];

    private readonly IReporteVacunoReadRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<ListarReporteVacunosUseCase> _logger;

    public ListarReporteVacunosUseCase(
        IReporteVacunoReadRepository repository,
        IDateTimeProvider dateTimeProvider,
        ILogger<ListarReporteVacunosUseCase> logger)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<ListadoVacunosReporteResponse> HandleAsync(
        ListarReporteVacunosQuery query,
        CancellationToken cancellationToken = default)
    {
        var formato = Normalize(query.Formato) ?? "json";
        EnsureAllowed("formato", formato, FormatosPermitidos, "INVALID_REPORT_FORMAT", "El formato debe ser json, pdf o excel.", "Formato no permitido.");

        if (formato is "pdf" or "excel")
        {
            throw new ApplicationRuleException(
                "REPORT_FORMAT_NOT_IMPLEMENTED",
                "La generacion de PDF y Excel pertenece a TK04/TK05.",
                [new ApplicationErrorDetail("formato", "Para TK02 solo esta implementado formato=json.")],
                501);
        }

        var fechaHasta = ParseDate(query.FechaHasta, "fechaHasta") ?? _dateTimeProvider.Today;
        var fechaDesde = ParseDate(query.FechaDesde, "fechaDesde") ?? fechaHasta.AddDays(-30);

        if (fechaDesde > fechaHasta)
        {
            throw new ApplicationRuleException(
                "VALIDATION_ERROR",
                "Los datos enviados no son validos.",
                [new ApplicationErrorDetail("fechaDesde", "fechaDesde no puede ser mayor que fechaHasta.")]);
        }

        var page = ParsePositiveIntOrDefault(query.Page, "page", 1);
        var limit = ParsePositiveIntOrDefault(query.Limit, "limit", 20);
        limit = Math.Min(limit, 100);

        var estado = Normalize(query.Estado);
        if (estado is not null)
        {
            EnsureAllowed("estado", estado, EstadosPermitidos, "VALIDATION_ERROR", "Los datos enviados no son validos.", "El estado debe ser vivo o muerto.");
        }

        var aptoPara = Normalize(query.AptoPara);
        if (aptoPara is not null)
        {
            EnsureAllowed("aptoPara", aptoPara, AptosPermitidos, "VALIDATION_ERROR", "Los datos enviados no son validos.", "Valor de aptoPara no permitido.");
        }

        _logger.LogInformation(
            "[ReporteVacuno] rango aplicado: [{FechaDesde} - {FechaHasta}]",
            fechaDesde,
            fechaHasta);

        var criteria = new ReporteVacunoListadoCriteria(
            fechaDesde,
            fechaHasta,
            Normalize(query.Q),
            Normalize(query.Raza),
            Normalize(query.Procedencia),
            estado,
            aptoPara,
            page,
            limit);

        var pageResult = await _repository.ListarAsync(criteria, cancellationToken);

        return new ListadoVacunosReporteResponse(
            pageResult.Items,
            new ReporteVacunoResumen(pageResult.TotalRegistros),
            new ReporteVacunoFiltros(fechaDesde, fechaHasta, criteria.Q, formato),
            null);
    }

    private static string? Normalize(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized.ToLowerInvariant();
    }

    private static DateOnly? ParseDate(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (DateOnly.TryParseExact(value.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }

        throw new ApplicationRuleException(
            "VALIDATION_ERROR",
            "Los datos enviados no son validos.",
            [new ApplicationErrorDetail(field, "La fecha debe usar formato YYYY-MM-DD.")]);
    }

    private static int ParsePositiveIntOrDefault(string? value, string field, int defaultValue)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        if (!int.TryParse(value.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var parsed))
        {
            throw new ApplicationRuleException(
                "VALIDATION_ERROR",
                "Los datos enviados no son validos.",
                [new ApplicationErrorDetail(field, $"El parametro {field} debe ser numerico.")]);
        }

        return parsed < 1 ? defaultValue : parsed;
    }

    private static void EnsureAllowed(
        string field,
        string value,
        IReadOnlyCollection<string> allowed,
        string code,
        string message,
        string detail)
    {
        if (allowed.Contains(value))
        {
            return;
        }

        throw new ApplicationRuleException(
            code,
            message,
            [new ApplicationErrorDetail(field, detail)]);
    }
}
