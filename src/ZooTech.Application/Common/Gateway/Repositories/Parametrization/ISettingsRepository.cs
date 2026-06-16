using ZooTech.Application.Common.Models;

namespace ZooTech.Application.Common.Gateway.Repositories.Parametrization;

public interface ISettingsRepository
{
    Task<IReadOnlyCollection<SettingValueDto>> GetTenantSettingsAsync(int tenantId);
}
