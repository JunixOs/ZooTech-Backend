using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

namespace ZooTech.Infrastructure.Tenant
{
    public class TenantParameterSynchronizer : ITenantParameterSynchronizer
    {
        private readonly ITenantDbContextFactory _tenantDbContextFactory;

        public TenantParameterSynchronizer(
            ITenantDbContextFactory tenantDbContextFactory
        )
        {
            _tenantDbContextFactory = tenantDbContextFactory;
        }

        public async Task SynchronizeAsync(int tenantId)
        {
            await using var tenantCatalogDb = _tenantDbContextFactory.CreateDbContextByTenantContext();

            var tenant = await tenantCatalogDb.tenants
                .Include(t => t.tenant_features)
                .Include(t => t.setting_values)
                .Include(t => t.tenant_business_rules)
                .FirstAsync(t => t.id == tenantId);

            await SynchronizeFeaturesAsync(tenantCatalogDb, tenant);
            await SynchronizeSettingsAsync(tenantCatalogDb, tenant);
            await SynchronizeRulesAsync(tenantCatalogDb, tenant);

            await tenantCatalogDb.SaveChangesAsync();
        }

        private static async Task SynchronizeFeaturesAsync(
            TenantCatalogDb db,
            tenant tenant)
        {
            var features = await db.features
                .Where(f => f.is_active && f.deleted_at == null)
                .ToListAsync();

            var assignedFeatures = tenant.tenant_features
                .Select(tf => tf.feature_id)
                .ToHashSet();

            foreach (var feature in features)
            {
                if (assignedFeatures.Contains(feature.id))
                    continue;

                tenant.tenant_features.Add(new tenant_feature
                {
                    feature_id = feature.id,
                    is_enabled = true,
                    enabled_at = DateTime.UtcNow
                });
            }
        }

        private static async Task SynchronizeSettingsAsync(
            TenantCatalogDb db,
            tenant tenant)
        {
            var definitions = await db.setting_definitions
                .Where(s =>
                    s.is_active &&
                    s.deleted_at == null &&
                    s.default_value != null)
                .ToListAsync();

            var assignedSettings = tenant.setting_values
                .Select(tf => tf.setting_definition_id)
                .ToHashSet();

            foreach (var definition in definitions)
            {
                if (assignedSettings.Contains(definition.id))
                    continue;

                tenant.setting_values.Add(new setting_value
                {
                    setting_definition_id = definition.id,
                    actor_type = "TENANT",
                    actor_id = null,
                    value = definition.default_value,
                    is_active = true
                });
            }
        }

        private static async Task SynchronizeRulesAsync(
            TenantCatalogDb db,
            tenant tenant)
        {
            var rules = await db.rule_definitions
                .Where(r =>
                    r.is_active &&
                    r.deleted_at == null)
                .ToListAsync();

            var assignedRules = tenant.tenant_business_rules
                .Select(tf => tf.rule_definition_id)
                .ToHashSet();

            foreach (var rule in rules)
            {
                if (assignedRules.Contains(rule.id))
                    continue;

                tenant.tenant_business_rules.Add(new tenant_business_rule
                {
                    rule_definition_id = rule.id,
                    is_active = true,
                    priority = 0,
                    execution_mode = "SYNC",
                    rule_version = 1
                });
            }
        }
    }
}