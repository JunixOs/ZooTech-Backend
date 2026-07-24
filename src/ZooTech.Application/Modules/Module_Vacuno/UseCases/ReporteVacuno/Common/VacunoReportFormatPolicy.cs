using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Domain.Configuration;
using ZooTech.Domain.Shared.Enums;

namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.Common;

public interface IVacunoReportFormatPolicy
{
    Task<ReportFileFormat> EnsureAllowedAsync(string? format);
}

public sealed class VacunoReportFormatPolicy : IVacunoReportFormatPolicy
{
    private static readonly char[] Separators = [',', ';', '|'];
    private readonly ITenantConfigurationProvider _configurationProvider;

    public VacunoReportFormatPolicy(ITenantConfigurationProvider configurationProvider)
    {
        _configurationProvider = configurationProvider;
    }

    public async Task<ReportFileFormat> EnsureAllowedAsync(string? format)
    {
        var requestedFormat = Parse(format);
        var configuredValue = await _configurationProvider.GetSettingAsync(
            Settings.Vacunos.VacunosReporteFormatosDescarga);
        var allowedFormats = ParseAllowedFormats(configuredValue);

        if (!allowedFormats.Contains(requestedFormat))
        {
            throw CreateValidationException(
                $"El formato '{ToWireValue(requestedFormat)}' no está habilitado para el tenant.");
        }

        return requestedFormat;
    }

    public static ReportFileFormat Parse(string? format)
    {
        return format?.Trim().ToLowerInvariant() switch
        {
            "excel" => ReportFileFormat.Excel,
            "pdf" => ReportFileFormat.Pdf,
            _ => throw CreateValidationException("El formato debe ser excel o pdf.")
        };
    }

    public static string ToWireValue(ReportFileFormat format)
        => format == ReportFileFormat.Excel ? "excel" : "pdf";

    private static HashSet<ReportFileFormat> ParseAllowedFormats(string? configuredValue)
    {
        if (string.IsNullOrWhiteSpace(configuredValue))
        {
            throw CreateValidationException(
                "La configuración de formatos de reporte del tenant está vacía.");
        }

        var formats = new HashSet<ReportFileFormat>();
        foreach (var token in configuredValue.Split(
                     Separators,
                     StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var normalized = token.Trim('[', ']', '"', '\'', ' ').ToLowerInvariant();
            if (normalized == "xlsx")
            {
                normalized = "excel";
            }

            if (normalized is not ("excel" or "pdf"))
            {
                throw CreateValidationException(
                    $"La configuración contiene un formato de reporte inválido: '{normalized}'.");
            }

            formats.Add(Parse(normalized));
        }

        if (formats.Count == 0)
        {
            throw CreateValidationException(
                "La configuración de formatos de reporte del tenant no contiene formatos válidos.");
        }

        return formats;
    }

    private static ValidationException CreateValidationException(string message)
        => new(
            [message],
            ScopeName.Application,
            ModuleName.Vacuno,
            message: message);
}
