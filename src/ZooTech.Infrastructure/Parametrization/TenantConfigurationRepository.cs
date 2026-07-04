using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Repositories.Parametrization;
using ZooTech.Domain.Configuration;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.Infrastructure.Parametrization;

public sealed class TenantConfigurationRepository : ITenantConfigurationRepository
{
    private readonly ITenantDbContextFactory _tenantDbContextFactory;

    public TenantConfigurationRepository(
        ITenantDbContextFactory tenantDbContextFactory
    )
    {
        _tenantDbContextFactory = tenantDbContextFactory;
    }

    public async Task<TenantConfiguration> LoadTenantConfigAsync(int tenantId)
    {
        var tenantDbContext = await _tenantDbContextFactory.CreateDbContextBySettingsValue();

        var settings = await tenantDbContext.setting_definitions
            .Where(sd => 
                sd.is_active &&
                sd.deleted_at == null &&
                sd.setting_group.is_active &&
                sd.setting_group.deleted_at == null
            )
            .Select(sd => new
                {
                    sd.code,
                    Value = sd.setting_values
                        .Where(v => 
                            v.tenant_id == tenantId &&
                            v.actor_type == "TENANT" &&
                            v.actor_id == null &&
                            v.deleted_at == null
                        )
                        .Select(v => v.value)
                        .FirstOrDefault() ?? sd.default_value
                }
            )
            .ToDictionaryAsync(x => x.code, x => x.Value);

        var features = await tenantDbContext.tenant_features
            .Where(tf =>
                tf.tenant_id == tenantId &&
                tf.is_enabled &&
                tf.feature.is_active &&
                tf.feature.deleted_at == null)
            .Select(tf => tf.feature.code)
            .ToHashSetAsync();

        var rules = await tenantDbContext.tenant_business_rules
            .Where(r =>
                r.tenant_id == tenantId &&
                r.is_active &&
                r.rule_definition.is_active &&
                r.rule_definition.deleted_at == null)
            .Select(r => r.rule_definition.code)
            .ToHashSetAsync();

        return new TenantConfiguration
        {
            TenantId = tenantId,
            Settings = settings,
            EnabledFeatures = features,
            EnabledRules = rules,
            LoadedAt = DateTime.UtcNow
        };
    }
}
