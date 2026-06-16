using ZooTech.Domain.Configuration;

namespace ZooTech.Application.Common.Gateway.Parametrization;

public interface ITenantConfigurationProvider
{
    Task<T> GetSettingAsync<T>(int tenantId, SettingDefinition<T> setting);
    Task<bool> IsFeatureEnabledAsync(int tenantId, FeatureCode feature);
    Task<bool> IsRuleEnabledAsync(int tenantId, RuleCode rule);
    Task InvalidateTenantAsync(int tenantId);
}
