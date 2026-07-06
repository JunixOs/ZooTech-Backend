using ZooTech.Application.Common.Gateway.Repositories.Parametrization;
using ZooTech.Application.Common.Models;

namespace ZooTech.Infrastructure.Parametrization.Settings;

public sealed class SettingsRepository : ISettingsRepository
{
    public Task<IReadOnlyCollection<SettingValueDto>> GetTenantSettingsAsync(int tenantId)
    {
        throw new NotImplementedException("SettingsRepository must be configured with a database provider.");
    }
}
