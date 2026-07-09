using ZooTech.Domain.Configuration;

namespace ZooTech.Application.Common.Gateway.Parametrization.Rules;

[Obsolete("Use ITenantConfigurationProvider instead. Will be removed in a future version.")]
public interface IRuleProvider
{
    Task<bool> IsEnabledAsync(int tenantId, RuleCode rule);
}
