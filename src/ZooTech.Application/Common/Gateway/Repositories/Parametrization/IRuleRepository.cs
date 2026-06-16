namespace ZooTech.Application.Common.Gateway.Repositories.Parametrization;

[Obsolete("Use ITenantConfigurationRepository instead. Will be removed in a future version.")]
public interface IRuleRepository
{
    Task<HashSet<string>> GetEnabledRuleCodesAsync(int tenantId);
}
