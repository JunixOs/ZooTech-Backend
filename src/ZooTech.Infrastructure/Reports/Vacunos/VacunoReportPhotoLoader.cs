using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZooTech.Application.Modules.Module_Vacuno.UseCases.ReporteVacuno.ObtenerRegistroVacunoReporte;
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
    private readonly string _mediaRoot;
    private readonly ILogger<VacunoReportPhotoLoader> _logger;

    public VacunoReportPhotoLoader(
        IOptions<ReportStorageOptions> options,
        ILogger<VacunoReportPhotoLoader> logger)
    {
        _mediaRoot = ResolveMediaRoot(options.Value.VacunoMediaRoot);
        _logger = logger;
    }

    public async Task<byte[]?> LoadAsync(
        RegistroVacunoDetalle vacuno,
        CancellationToken cancellationToken = default)
    {
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

            var file = new FileInfo(path);
            if (!file.Exists || file.Length <= 0 || file.Length > MaxImageBytes)
            {
                continue;
            }

            var content = await File.ReadAllBytesAsync(path, cancellationToken);
            if (IsSupportedImage(content))
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

    private static bool IsSupportedImage(ReadOnlySpan<byte> content)
    {
        var isPng = content.Length >= 8 &&
                    content[..8].SequenceEqual(
                        new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
        var isJpeg = content.Length >= 3 &&
                     content[0] == 0xFF &&
                     content[1] == 0xD8 &&
                     content[2] == 0xFF;
        return isPng || isJpeg;
    }
}
