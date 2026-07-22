using ZooTech.Domain.Configuration;

namespace ZooTech.Application.Common.Gateway.Parametrization.Settings;

[Obsolete("Use ITenantConfigurationProvider instead. Will be removed in a future version.")]
public interface ISettingsProvider
{
    Task<T> GetAsync<T>(int tenantId, SettingDefinition<T> setting);

    Task RefreshAsync(int tenantId);
}
