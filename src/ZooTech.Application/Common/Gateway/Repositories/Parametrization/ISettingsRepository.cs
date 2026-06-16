using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Common.Gateway.Repositories.Parametrization;

[Obsolete("Use ITenantConfigurationRepository instead. Will be removed in a future version.")]
public interface ISettingsRepository
{
    Task<IReadOnlyCollection<SettingValueDto>> GetTenantSettingsAsync(int tenantId);
}
