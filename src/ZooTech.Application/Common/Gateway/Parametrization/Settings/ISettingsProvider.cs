using ZooTech.Domain.Configuration;

namespace ZooTech.Application.Common.Gateway.Parametrization.Settings;

public interface ISettingsProvider
{
    Task<T> GetAsync<T>(int tenantId, SettingDefinition<T> setting);

    Task RefreshAsync(int tenantId);
}
