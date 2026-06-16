namespace ZooTech.Application.Common.Gateway.Repositories.Parametrization;

[Obsolete("Use ITenantConfigurationRepository instead. Will be removed in a future version.")]
public interface IFeatureRepository
{
    Task<HashSet<string>> GetEnabledFeatureCodesAsync(int tenantId);
}
