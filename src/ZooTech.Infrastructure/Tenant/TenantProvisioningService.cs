using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

namespace ZooTech.Infrastructure.Tenant
{
    // TODO: TenantProvisioningService Culminar implementacion
    public class TenantProvisioningService : ITenantProvisioningService
    {
        private readonly TenantCatalogDb _tenantCatalogDb;
        private readonly IConfiguration _config;
        private readonly ITenantDatabaseMigrator _tenantDatabaseMigrator;

        public TenantProvisioningService(
            TenantCatalogDb tenantCatalogDb, 
            IConfiguration config,
            ITenantDatabaseMigrator tenantDatabaseMigrator
        )
        {
            _tenantCatalogDb = tenantCatalogDb;
            _config = config;
            _tenantDatabaseMigrator = tenantDatabaseMigrator;
        }

        public async Task<bool> ProvisionAsync(CreateTenantCommand cmd)
        {
            var tenantDbName = $"ZooTech_{cmd.Code.Replace("-", "_")}_Db";

            tenant_database_connection tenantDatabaseConnectionEntity = new tenant_database_connection
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
                address = new address
                    {
                        country = cmd.TenantAddress.Country,
                        state = cmd.TenantAddress.State,
                        province = cmd.TenantAddress.Province,
                        city = cmd.TenantAddress.City,
                        address_line_1 = cmd.TenantAddress.AddressLine_1,
                        address_line_2 = cmd.TenantAddress.AddressLine_2,
                        metadata = cmd.TenantAddress.Metadata,
                        created_at = cmd.TenantAddress.CreatedAt
                    },
                tenant_database_connection = tenantDatabaseConnectionEntity,
                status = cmd.Status,
                metadata = cmd.Metadata,
                created_at = cmd.CreatedAt
            };

            try
            {
                _tenantCatalogDb.tenants.Add(tenant);
                await _tenantCatalogDb.SaveChangesAsync();

                // Crear Base de datos para el tenant
                var template = _config.GetConnectionString("TenantTemplate");
                var builder = new SqlConnectionStringBuilder(template);

                builder.InitialCatalog = tenantDbName; // Aqui va el nomobre de la base de datos

                var conn = builder.ConnectionString;

                await _tenantDatabaseMigrator.MigrateAsync(conn); // Se necesitan permisos para crear la BD

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}