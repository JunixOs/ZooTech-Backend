using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZooTech.Application.Common.Gateway.Parametrization;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;
using ZooTech.Domain.Configuration;
using ZooTech.Infrastructure.Storage;

namespace ZooTech.Infrastructure.Reports.Vacunos;

public interface IVacunoReportPhotoLoader
{
    Task<byte[]?> LoadAsync(
        RegistroVacunoDetalle vacuno,
        CancellationToken cancellationToken = default);
}

public sealed class VacunoReportPhotoLoader : IVacunoReportPhotoLoader
{
    private const long MaxImageBytes = 5L * 1024 * 1024;
    private static readonly char[] FormatSeparators = [',', ';', '|'];
    private readonly string _mediaRoot;
    private readonly ITenantConfigurationProvider _configurationProvider;
    private readonly ILogger<VacunoReportPhotoLoader> _logger;

    public VacunoReportPhotoLoader(
        IOptions<ReportStorageOptions> options,
        ITenantConfigurationProvider configurationProvider,
        ILogger<VacunoReportPhotoLoader> logger)
    {
        _mediaRoot = ResolveMediaRoot(options.Value.VacunoMediaRoot);
        _configurationProvider = configurationProvider;
        _logger = logger;
    }

    public async Task<byte[]?> LoadAsync(
        RegistroVacunoDetalle vacuno,
        CancellationToken cancellationToken = default)
    {
        var allowedFormats = await GetAllowedFormatsAsync(vacuno.Id, cancellationToken);
        if (allowedFormats is null)
        {
            return null;
        }

        var candidates = new[]
        {
            vacuno.FotoRuta,
            vacuno.FotoUrl,
            vacuno.FotoNombreAlmacenado,
            vacuno.FotoNombreOriginal
        };
        var hasCandidate = false;

        foreach (var candidate in candidates.Where(value => !string.IsNullOrWhiteSpace(value)))
        {
            hasCandidate = true;
            var path = ResolveAllowedPath(candidate!);
            if (path is null)
            {
                continue;
            }

            var candidateFormat = NormalizeFormat(Path.GetExtension(path));
            if (candidateFormat is null)
            {
                candidateFormat = NormalizeFormat(vacuno.FotoExtension);
            }

            if (candidateFormat is null || !allowedFormats.Contains(candidateFormat))
            {
                continue;
            }

            var file = new FileInfo(path);
            if (!file.Exists || file.Length <= 0 || file.Length > MaxImageBytes)
            {
                continue;
            }

            var content = await File.ReadAllBytesAsync(path, cancellationToken);
            var detectedFormat = DetectImageFormat(content);
            if (detectedFormat == candidateFormat &&
                allowedFormats.Contains(detectedFormat))
            {
                return content;
            }
        }

        if (hasCandidate)
        {
            _logger.LogWarning(
                "La fotografía del vacuno {VacunoId} no pertenece al directorio permitido o no es una imagen válida; el reporte se generará sin fotografía.",
                vacuno.Id);
        }

        return null;
    }

    private async Task<HashSet<string>?> GetAllowedFormatsAsync(
        long vacunoId,
        CancellationToken cancellationToken)
    {
        try
        {
            var configuredFormats = await _configurationProvider.GetSettingAsync(
                Settings.Vacunos.VacunosFotoFormatosPermitidos);
            var formats = ParseAllowedFormats(configuredFormats);
            if (formats is not null)
            {
                return formats;
            }
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogWarning(
                exception,
                "No se pudo obtener la configuración de fotografías del vacuno {VacunoId}; el reporte se generará sin fotografía.",
                vacunoId);
            return null;
        }

        _logger.LogWarning(
            "La configuración de formatos de fotografía no es válida para el vacuno {VacunoId}; el reporte se generará sin fotografía.",
            vacunoId);
        return null;
    }

    private static HashSet<string>? ParseAllowedFormats(string? configuredFormats)
    {
        if (string.IsNullOrWhiteSpace(configuredFormats))
        {
            return null;
        }

        var formats = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var token in configuredFormats.Split(
                     FormatSeparators,
                     StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var format = NormalizeFormat(token.Trim('[', ']', '"', '\'', ' '));
            if (format is null)
            {
                return null;
            }

            formats.Add(format);
        }

        return formats.Count == 0 ? null : formats;
    }

    private string? ResolveAllowedPath(string candidate)
    {
        string candidatePath;
        if (Uri.TryCreate(candidate, UriKind.Absolute, out var uri))
        {
            if (!uri.IsFile)
            {
                return null;
            }

            candidatePath = uri.LocalPath;
        }
        else
        {
            candidatePath = Path.IsPathRooted(candidate)
                ? candidate
                : Path.Combine(_mediaRoot, candidate.TrimStart('/', '\\'));
        }

        string fullPath;
        try
        {
            fullPath = Path.GetFullPath(candidatePath);
        }
        catch (Exception exception) when (
            exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return null;
        }

        var rootPrefix = _mediaRoot.EndsWith(Path.DirectorySeparatorChar)
            ? _mediaRoot
            : _mediaRoot + Path.DirectorySeparatorChar;

        return fullPath.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase)
            ? fullPath
            : null;
    }

    private static string ResolveMediaRoot(string configuredRoot)
    {
        var root = string.IsNullOrWhiteSpace(configuredRoot) ? "wwwroot" : configuredRoot.Trim();
        return Path.GetFullPath(
            Path.IsPathRooted(root)
                ? root
                : Path.Combine(AppContext.BaseDirectory, root));
    }

    private static string? NormalizeFormat(string? format)
        => format?.Trim().TrimStart('.').ToLowerInvariant() switch
        {
            "png" => "png",
            "jpg" or "jpeg" => "jpeg",
            _ => null
        };

    private static string? DetectImageFormat(ReadOnlySpan<byte> content)
    {
        var isPng = content.Length >= 8 &&
                    content[..8].SequenceEqual(
                        new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
        var isJpeg = content.Length >= 3 &&
                     content[0] == 0xFF &&
                     content[1] == 0xD8 &&
                     content[2] == 0xFF;
        return isPng ? "png" : isJpeg ? "jpeg" : null;
    }
}
