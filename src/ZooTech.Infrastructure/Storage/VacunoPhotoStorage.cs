using System.Globalization;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Modules.Module_Vacuno.Services;

namespace ZooTech.Infrastructure.Storage;

public sealed class VacunoPhotoStorage : IVacunoPhotoStorage
{
    private static readonly IReadOnlyDictionary<string, PhotoFormat> Formats =
        new Dictionary<string, PhotoFormat>(StringComparer.OrdinalIgnoreCase)
        {
            ["image/jpeg"] = new(".jpg", IsJpeg),
            ["image/jpg"] = new(".jpg", IsJpeg),
            ["image/png"] = new(".png", IsPng)
        };

    private readonly string _mediaRoot;
    private readonly ITenantContext _tenantContext;

    public VacunoPhotoStorage(
        IOptions<ReportStorageOptions> options,
        ITenantContext tenantContext)
    {
        _mediaRoot = ResolveMediaRoot(options.Value.VacunoMediaRoot);
        _tenantContext = tenantContext;
    }

    public async Task<StoredVacunoPhoto> SaveAsync(
        VacunoPhotoUpload upload,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(upload);

        if (upload.Content.Length == 0 || upload.Content.LongLength > VacunoPhotoUpload.MaxBytes)
        {
            throw new ArgumentException(
                $"La foto debe pesar entre 1 byte y {VacunoPhotoUpload.MaxBytes} bytes.",
                nameof(upload));
        }

        if (!Formats.TryGetValue(upload.ContentType, out var format) ||
            !format.HasValidSignature(upload.Content))
        {
            throw new ArgumentException(
                "La foto debe ser un archivo JPG, JPEG o PNG valido.",
                nameof(upload));
        }

        var originalName = SanitizeOriginalName(upload.FileName, format.Extension);
        var storedName = $"{Guid.NewGuid():N}{format.Extension}";
        var tenantPath = Path.Combine(
            _tenantContext.TenantId.ToString(CultureInfo.InvariantCulture),
            "vacunos");
        var relativePath = Path.Combine(tenantPath, storedName).Replace('\\', '/');
        var directory = Path.Combine(_mediaRoot, tenantPath);

        Directory.CreateDirectory(directory);
        await File.WriteAllBytesAsync(
            Path.Combine(directory, storedName),
            upload.Content,
            cancellationToken);

        return new StoredVacunoPhoto(
            originalName,
            storedName,
            relativePath,
            format.Extension.TrimStart('.').ToUpperInvariant(),
            format.Extension == ".png" ? "image/png" : "image/jpeg",
            upload.Content.LongLength,
            Convert.ToHexString(SHA256.HashData(upload.Content)).ToLowerInvariant());
    }

    public async Task<VacunoPhotoContent?> ReadAsync(
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        var path = ResolveAllowedPath(relativePath);
        if (path is null || !File.Exists(path))
        {
            return null;
        }

        var content = await File.ReadAllBytesAsync(path, cancellationToken);
        var contentType = IsPng(content)
            ? "image/png"
            : IsJpeg(content)
                ? "image/jpeg"
                : null;

        return contentType is null ? null : new VacunoPhotoContent(contentType, content);
    }

    public Task DeleteAsync(
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var path = ResolveAllowedPath(relativePath);
        if (path is not null && File.Exists(path))
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }

    private string? ResolveAllowedPath(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath) || Path.IsPathRooted(relativePath))
        {
            return null;
        }

        var tenantRoot = Path.GetFullPath(Path.Combine(
            _mediaRoot,
            _tenantContext.TenantId.ToString(CultureInfo.InvariantCulture)));
        var fullPath = Path.GetFullPath(Path.Combine(_mediaRoot, relativePath));
        var prefix = tenantRoot.EndsWith(Path.DirectorySeparatorChar)
            ? tenantRoot
            : tenantRoot + Path.DirectorySeparatorChar;

        return fullPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
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

    private static string SanitizeOriginalName(string fileName, string expectedExtension)
    {
        var name = Path.GetFileName(fileName);
        var stem = Path.GetFileNameWithoutExtension(name);
        var safeStem = new string(stem
            .Where(character => !Path.GetInvalidFileNameChars().Contains(character))
            .Take(120)
            .ToArray());

        return $"{(string.IsNullOrWhiteSpace(safeStem) ? "vacuno" : safeStem)}{expectedExtension}";
    }

    private static bool IsPng(ReadOnlySpan<byte> content)
        => content.Length >= 8 &&
           content[..8].SequenceEqual(
               new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });

    private static bool IsJpeg(ReadOnlySpan<byte> content)
        => content.Length >= 3 &&
           content[0] == 0xFF &&
           content[1] == 0xD8 &&
           content[2] == 0xFF;

    private delegate bool SignatureValidator(ReadOnlySpan<byte> content);

    private sealed record PhotoFormat(
        string Extension,
        SignatureValidator HasValidSignature);
}
