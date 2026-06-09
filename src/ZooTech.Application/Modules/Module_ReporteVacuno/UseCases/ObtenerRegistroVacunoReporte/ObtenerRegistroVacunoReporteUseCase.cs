using ZooTech.Application.Common.Exceptions;

using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Configuration;

namespace ZooTech.Application.Modules.Module_ReporteVacuno.UseCases.ObtenerRegistroVacunoReporte;

public sealed class ObtenerRegistroVacunoReporteUseCase : IObtenerRegistroVacunoReporteUseCase
{
    private readonly IRegistroVacunoReadRepository _repository;
    private readonly IRegistroVacunoExcelReportService _excelReportService;
    private readonly IRegistroVacunoPdfReportService _pdfReportService;
    private readonly ISettingProvider _settingProvider;
    private readonly ITenantContext _tenantContext;

    public ObtenerRegistroVacunoReporteUseCase(
        IRegistroVacunoReadRepository repository,
        IRegistroVacunoExcelReportService excelReportService,
        IRegistroVacunoPdfReportService pdfReportService,
        ISettingProvider settingProvider,
        ITenantContext tenantContext)
    {
        _repository = repository;
        _excelReportService = excelReportService;
        _pdfReportService = pdfReportService;
        _settingProvider = settingProvider;
        _tenantContext = tenantContext;
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

        var allowedFormatsRaw = await _settingProvider.GetSettingAsync<string[]>("REPORTS_ALLOWED_FORMATS", _tenantContext.TenantId);
        var allowedFormats = allowedFormatsRaw is { Length: > 0 } ? allowedFormatsRaw : ["json", "pdf", "excel"];

        var formato = Normalize(query.Formato) ?? "json";
        EnsureFormatoValido(formato, allowedFormats);

        var detalle = await _repository.ObtenerRegistroAsync(query.VacunoId, cancellationToken);

        if (detalle is null)
        {
            throw new ApplicationRuleException(
                "VACUNO_NOT_FOUND",
                "No existe un vacuno con el ID enviado.",
                [],
                404);
        }

        if (formato is "excel")
        {
            var excel = await _excelReportService.GenerateAsync(detalle, cancellationToken);

            return new RegistroVacunoReporteResponse(
                detalle,
                Array.Empty<object>(),
                excel.DownloadUrl);
        }

        if (formato is "pdf")
        {
            var pdf = await _pdfReportService.GenerateAsync(detalle, cancellationToken);

            return new RegistroVacunoReporteResponse(
                detalle,
                Array.Empty<object>(),
                pdf.DownloadUrl);
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

    private static void EnsureFormatoValido(string formato, string[] allowedFormats)
    {
        if (!allowedFormats.Contains(formato, StringComparer.OrdinalIgnoreCase))
        {
            throw new ApplicationRuleException(
                "INVALID_REPORT_FORMAT",
                $"El formato debe ser uno de los permitidos: {string.Join(", ", allowedFormats)}.",
                [new ApplicationErrorDetail("formato", "Formato no permitido.")]);
        }
    }
}
