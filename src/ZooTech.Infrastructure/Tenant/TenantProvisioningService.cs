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

            AddressEntity addressEntity = new AddressEntity
            {
                Country = cmd.TenantAddress.Country,
                State = cmd.TenantAddress.State,
                Province = cmd.TenantAddress.Province,
                City = cmd.TenantAddress.City,
                AddressLine1 = cmd.TenantAddress.AddressLine_1,
                AddressLine2 = cmd.TenantAddress.AddressLine_2,
                Metadata = cmd.TenantAddress.Metadata,
                CreatedAt = DateTime.Now
            };

            TenantDatabaseConnectionEntity tenantDatabaseConnectionEntity = new TenantDatabaseConnectionEntity
            {
                DatabaseName = tenantDbName,
                IsActive = cmd.TenantDatabaseConnection.IsActive
            };

            var tenant = new TenantEntity
            {
                Code = cmd.Code,
                SubDomain = cmd.SubDomain,
                DisplayName = cmd.DisplayName,
                LegalName = cmd.LegalName,
                Email = cmd.Email,
                Phone = cmd.Phone,
                Address = addressEntity,
                TenantDatabaseConnection = tenantDatabaseConnectionEntity,
                Status = cmd.Status,
                Metadata = cmd.Metadata,
                CreatedAt = DateTime.Now
            };

            try
            {
                _tenantCatalogDb.TenantEntity.Add(tenant);
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