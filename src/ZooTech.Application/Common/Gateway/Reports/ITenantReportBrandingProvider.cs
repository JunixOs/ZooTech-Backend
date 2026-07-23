namespace ZooTech.Application.Common.Gateway.Reports;

public interface ITenantReportBrandingProvider
{
    Task<TenantReportBranding> GetAsync(CancellationToken cancellationToken = default);
}

public sealed record TenantReportBranding(
    string DisplayName,
    byte[]? LogoContent);
