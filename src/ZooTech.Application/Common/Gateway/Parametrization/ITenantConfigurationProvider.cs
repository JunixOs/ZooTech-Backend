using ZooTech.Domain.Configuration;

namespace ZooTech.Application.Common.Gateway.Parametrization;

public interface ITenantConfigurationProvider
{
    Task<T> GetSettingAsync<T>(SettingDefinition<T> setting);
    Task<bool> IsFeatureEnabledAsync(FeatureCode feature);
    Task<bool> IsRuleEnabledAsync(RuleCode rule);
    Task InvalidateTenantAsync();
}
