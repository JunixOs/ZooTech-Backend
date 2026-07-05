using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.Infrastructure.Exceptions;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

namespace ZooTech.Infrastructure.Tenant
{
    public class TenantProvisioningService : ITenantProvisioningService
    {
        private readonly ITenantDbContextFactory _tenantDbContextFactory;
        private readonly ITenantDatabaseMigrator _tenantDatabaseMigrator;
        private readonly ITenantDatabaseCreator _tenantDatabaseCreator;

        public TenantProvisioningService(
            ITenantDbContextFactory tenantDbContextFactory,
            ITenantDatabaseMigrator tenantDatabaseMigrator,
            ITenantDatabaseCreator tenantDatabaseCreator
        )
        {
            _tenantDbContextFactory = tenantDbContextFactory;
            _tenantDatabaseMigrator = tenantDatabaseMigrator;
            _tenantDatabaseCreator = tenantDatabaseCreator;
        }

        public async Task ProvisionAsync(CreateTenantCommand cmd)
        {
            var tenantDbName = SanitizeDbName(cmd.Code);

            var tenantDatabaseConnectionEntity = new tenant_database_connection
            {
                database_name = tenantDbName,
                is_active = cmd.TenantDatabaseConnection.IsActive
            };

            var tenant = new tenant
            {
                code = cmd.Code,
                subdomain = cmd.SubDomain,
                display_name = cmd.DisplayName,
                legal_name = cmd.LegalName,
                email = cmd.Email,
                phone = cmd.Phone,
                timezone = cmd.TimeZone,
                address = new address
                {
                    country = cmd.TenantAddress.Country,
                    state = cmd.TenantAddress.State,
                    province = cmd.TenantAddress.Province,
                    city = cmd.TenantAddress.City,
                    address_line_1 = cmd.TenantAddress.AddressLine_1,
                    address_line_2 = cmd.TenantAddress.AddressLine_2,
                    metadata = cmd.TenantAddress.Metadata,
                    created_at = DateTime.UtcNow
                },
                tenant_branding = new tenant_branding
                {
                    primary_color = cmd.TenantBranding.PrimaryColor,
                    secondary_color = cmd.TenantBranding.SecondaryColor,
                    logo_url = cmd.TenantBranding.LogoUrl,
                    metadata = cmd.TenantBranding.Metadata,
                    created_at = DateTime.UtcNow
                },
                tenant_database_connection = tenantDatabaseConnectionEntity,
                status = cmd.Status.ToString(),
                metadata = cmd.Metadata,
                created_at = DateTime.UtcNow
            };

            var tenantCatalogDb = await _tenantDbContextFactory.CreateDbContextByTenantContext();

            bool tenantSaved = false;
            bool databaseCreated = false;

            try
            {
                await tenantCatalogDb.tenants.AddAsync(tenant);

                await AssociateParametersInDbToTenant(tenantCatalogDb , tenant);

                await tenantCatalogDb.SaveChangesAsync();
                tenantSaved = true;

                await _tenantDatabaseCreator.CreateAsync(tenantDatabaseConnectionEntity.database_name);
                databaseCreated = true;

                await _tenantDatabaseMigrator.MigrateAsync(tenantDatabaseConnectionEntity.database_name);
            }
            catch
            {
                if (databaseCreated)
                {
                    await _tenantDatabaseCreator.DeleteAsync(tenantDatabaseConnectionEntity.database_name);
                }

                if (tenantSaved)
                {
                    tenantCatalogDb.tenants.Remove(tenant);
                    await tenantCatalogDb.SaveChangesAsync();
                }

                throw new TenantProvisioningException();
            }
        }

        private static string SanitizeDbName(string code)
        {
            var sanitized = Regex.Replace(code, @"[^a-zA-Z0-9_]", "_");
            return $"ZooTech_{sanitized}_Db";
        }

        private static async Task AssociateParametersInDbToTenant(TenantCatalogDb tenantCatalogDb, tenant tenant)
        {
            var features = await tenantCatalogDb.features
                .Where(f => f.is_active && f.deleted_at == null)
                .ToListAsync();

            foreach (var feature in features)
            {
                tenant.tenant_features.Add(new tenant_feature
                {
                    feature_id = feature.id,
                    is_enabled = true,
                    enabled_at = DateTime.UtcNow
                });
            }

            var settingDefinitions = await tenantCatalogDb.setting_definitions
                .Where(s => s.is_active &&
                            s.deleted_at == null &&
                            s.default_value != null)
                .ToListAsync();

            foreach (var settingDefinition in settingDefinitions)
            {
                tenant.setting_values.Add(new setting_value
                {
                    setting_definition_id = settingDefinition.id,
                    actor_type = "TENANT",
                    actor_id = null,
                    value = settingDefinition.default_value,
                    is_active = true
                });
            }

            var rules = await tenantCatalogDb.rule_definitions
                .Where(r => r.is_active &&
                            r.deleted_at == null)
                .ToListAsync();

            foreach (var rule in rules)
            {
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
