using System.Globalization;
using Microsoft.Extensions.Options;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Reports;

namespace ZooTech.Infrastructure.Storage;

public sealed class VacunoReportFileStorage : IReportFileStorage
{
    private static readonly IReadOnlyDictionary<string, string> ContentTypes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [".xlsx"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            [".pdf"] = "application/pdf"
        };

    private readonly ReportStorageOptions _options;
    private readonly ITenantContext _tenantContext;

    public VacunoReportFileStorage(
        IOptions<ReportStorageOptions> options,
        ITenantContext tenantContext)
    {
        _options = options.Value;
        _tenantContext = tenantContext;
    }

    public async Task<StoredReportFile> SaveAsync(
        string filePrefix,
        string extension,
        string contentType,
        byte[] content,
        CancellationToken cancellationToken = default)
    {
        var normalizedExtension = NormalizeExtension(extension);
        if (!ContentTypes.TryGetValue(normalizedExtension, out var expectedContentType) ||
            !string.Equals(contentType, expectedContentType, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("El tipo de archivo de reporte no está permitido.");
        }

        var prefix = SanitizeFileNamePart(filePrefix);
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        var fileName = $"{prefix}_{timestamp}_{Guid.NewGuid():N}{normalizedExtension}";
        var outputDirectory = GetTenantDirectory();
        Directory.CreateDirectory(outputDirectory);

        await File.WriteAllBytesAsync(
            Path.Combine(outputDirectory, fileName),
            content,
            cancellationToken);

        return new StoredReportFile(
            fileName,
            $"/api/v1/vacunos/reportes/descargas/{Uri.EscapeDataString(fileName)}");
    }

    public async Task<StoredReportFileContent?> ReadAsync(
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileName) ||
            !string.Equals(fileName, Path.GetFileName(fileName), StringComparison.Ordinal))
        {
            return null;
        }

        var extension = Path.GetExtension(fileName);
        if (!ContentTypes.TryGetValue(extension, out var contentType))
        {
            return null;
        }

        var tenantDirectory = Path.GetFullPath(GetTenantDirectory());
        var filePath = Path.GetFullPath(Path.Combine(tenantDirectory, fileName));
        if (!filePath.StartsWith(tenantDirectory + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
            !File.Exists(filePath))
        {
            return null;
        }

        var content = await File.ReadAllBytesAsync(filePath, cancellationToken);
        return new StoredReportFileContent(fileName, contentType, content);
    }

    private string GetTenantDirectory()
        => Path.Combine(
            AppContext.BaseDirectory,
            _options.ReportesBasePath,
            _options.ReportesVacunosPath,
            _tenantContext.TenantId.ToString(CultureInfo.InvariantCulture));

    private static string NormalizeExtension(string extension)
        => extension.StartsWith('.') ? extension.ToLowerInvariant() : $".{extension.ToLowerInvariant()}";

    private static string SanitizeFileNamePart(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var safe = new string(value
            .Where(character => !invalidChars.Contains(character) && !char.IsWhiteSpace(character))
            .ToArray());

        return string.IsNullOrWhiteSpace(safe) ? "reporte_vacunos" : safe;
    }
}
