using ZooTech.Domain.Configuration;

namespace ZooTech.Application.Common.Gateway.Repositories.Parametrization;

public interface ITenantConfigurationRepository
{
    Task<TenantConfiguration> LoadTenantConfigAsync(int tenantId);
}
