using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Application.Common.Gateway.Reports;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.Infrastructure.Reports.Vacunos;

public sealed class TenantReportBrandingProvider : ITenantReportBrandingProvider
{
    private const int MaxLogoBytes = 2 * 1024 * 1024;
    private static readonly TimeSpan ValidBrandingCacheDuration = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan MissingBrandingCacheDuration = TimeSpan.FromMinutes(1);
    private readonly TenantCatalogDb _tenantCatalogDb;
    private readonly ITenantContext _tenantContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<TenantReportBrandingProvider> _logger;

    public TenantReportBrandingProvider(
        ITenantDbContextFactory tenantDbContextFactory,
        ITenantContext tenantContext,
        IHttpClientFactory httpClientFactory,
        IMemoryCache memoryCache,
        ILogger<TenantReportBrandingProvider> logger)
    {
        _tenantCatalogDb = tenantDbContextFactory.CreateDbContextBySettingsValue();
        _tenantContext = tenantContext;
        _httpClientFactory = httpClientFactory;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<TenantReportBranding> GetAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = $"vacunos:report-branding:{_tenantContext.TenantId}";
        if (_memoryCache.TryGetValue<TenantReportBranding>(cacheKey, out var cachedBranding) &&
            cachedBranding is not null)
        {
            return cachedBranding;
        }

        var displayName = string.IsNullOrWhiteSpace(_tenantContext.DisplayName)
            ? "ZooTech"
            : _tenantContext.DisplayName;

        try
        {
            var branding = await _tenantCatalogDb.tenants
                .AsNoTracking()
                .Where(tenant => tenant.id == _tenantContext.TenantId)
                .Select(tenant => new
                {
                    tenant.display_name,
                    LogoUrl = tenant.tenant_branding != null
                        ? tenant.tenant_branding.logo_url
                        : null
                })
                .FirstOrDefaultAsync(cancellationToken);

            var logoContent = await TryLoadLogoAsync(branding?.LogoUrl, cancellationToken);
            var result = new TenantReportBranding(
                string.IsNullOrWhiteSpace(branding?.display_name) ? displayName : branding.display_name,
                logoContent);
            _memoryCache.Set(
                cacheKey,
                result,
                logoContent is null
                    ? MissingBrandingCacheDuration
                    : ValidBrandingCacheDuration);
            return result;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogWarning(
                exception,
                "No se pudo obtener el branding del tenant {TenantId}; se usará el encabezado predeterminado.",
                _tenantContext.TenantId);

            var fallback = new TenantReportBranding(displayName, null);
            _memoryCache.Set(cacheKey, fallback, MissingBrandingCacheDuration);
            return fallback;
        }
    }

    private async Task<byte[]?> TryLoadLogoAsync(
        string? logoUrl,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(logoUrl))
        {
            return null;
        }

        try
        {
            if (Uri.TryCreate(logoUrl, UriKind.Absolute, out var uri))
            {
                if (uri.IsFile)
                {
                    return await ReadLocalLogoAsync(uri.LocalPath, cancellationToken);
                }

                if (uri.Scheme != Uri.UriSchemeHttps)
                {
                    return null;
                }

                var client = _httpClientFactory.CreateClient("TenantReportBranding");
                using var response = await client.GetAsync(
                    uri,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

                if (!response.IsSuccessStatusCode ||
                    response.Content.Headers.ContentLength > MaxLogoBytes ||
                    response.Content.Headers.ContentType?.MediaType?.StartsWith(
                        "image/",
                        StringComparison.OrdinalIgnoreCase) != true)
                {
                    return null;
                }

                await using var source = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var destination = new MemoryStream();
                var buffer = new byte[81920];
                int bytesRead;
                while ((bytesRead = await source.ReadAsync(buffer, cancellationToken)) > 0)
                {
                    if (destination.Length + bytesRead > MaxLogoBytes)
                    {
                        return null;
                    }

                    await destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                }

                return IsSupportedImage(destination.GetBuffer().AsSpan(0, (int)destination.Length))
                    ? destination.ToArray()
                    : null;
            }

            var relativePath = logoUrl.TrimStart('/', '\\');
            var wwwrootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", relativePath);
            if (File.Exists(wwwrootPath))
            {
                return await ReadLocalLogoAsync(wwwrootPath, cancellationToken);
            }

            return await ReadLocalLogoAsync(
                Path.Combine(AppContext.BaseDirectory, relativePath),
                cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogWarning(
                exception,
                "No se pudo cargar el logotipo del tenant {TenantId}; el reporte se generará sin logotipo.",
                _tenantContext.TenantId);
            return null;
        }
    }

    private static async Task<byte[]?> ReadLocalLogoAsync(
        string path,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(path) || new FileInfo(path).Length > MaxLogoBytes)
        {
            return null;
        }

        var content = await File.ReadAllBytesAsync(path, cancellationToken);
        return IsSupportedImage(content) ? content : null;
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
