using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.Infrastructure.Exceptions;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

namespace ZooTech.Infrastructure.Tenant
{
    public class TenantProvisioningService : ITenantProvisioningService
    {
        private readonly TenantCatalogDb _tenantCatalogDb;
        private readonly IConfiguration _config;
        private readonly ITenantDatabaseMigrator _tenantDatabaseMigrator;

        public TenantProvisioningService(
            ITenantDbContextFactory tenantDbContextFactory,
            IConfiguration config,
            ITenantDatabaseMigrator tenantDatabaseMigrator
        )
        {
            _tenantCatalogDb = tenantDbContextFactory.CreateDbContext();
            _config = config;
            _tenantDatabaseMigrator = tenantDatabaseMigrator;
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

            try
            {
                _tenantCatalogDb.tenants.Add(tenant);
                await _tenantCatalogDb.SaveChangesAsync();

                await _tenantDatabaseMigrator.MigrateAsync();
            }
            catch (Exception)
            {
                throw new TenantProvisioningException();
            }
        }

        private static string SanitizeDbName(string code)
        {
            var sanitized = Regex.Replace(code, @"[^a-zA-Z0-9_]", "_");
            return $"ZooTech_{sanitized}_Db";
        }
    }
}
