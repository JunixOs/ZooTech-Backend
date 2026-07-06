using ZooTech.Domain.Configuration;

namespace ZooTech.Application.Common.Gateway.Parametrization.Features;

[Obsolete("Use ITenantConfigurationProvider instead. Will be removed in a future version.")]
public interface IFeatureProvider
{
    Task<bool> IsEnabledAsync(int tenantId, FeatureCode feature);
}
