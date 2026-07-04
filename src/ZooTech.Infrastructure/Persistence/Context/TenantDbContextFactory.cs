using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Infrastructure.Exceptions;
using ZooTech.Infrastructure.Persistence.Context;


namespace ZooTech.Infrastructure.Tenant
{
    public class TenantDbContextFactory : ITenantDbContextFactory
    {
        private readonly ITenantContext _tenantContext;
        private readonly IConfiguration _config;

        public TenantDbContextFactory(ITenantContext tenantContext, IConfiguration config)
        {
            _tenantContext = tenantContext;
            _config = config;
        }

        private DbContextOptions<TenantCatalogDb> GetConnectionOptions(string databaseName)
        {
            var template = _config.GetConnectionString("TenantTemplate");

            if (string.IsNullOrWhiteSpace(template))
                throw new UndefinedConfigurationValue(
                    message: "ConnectionStrings:TenantTemplate not defined"
                );

            var builder = new SqlConnectionStringBuilder(template);

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new EmptyTenantContextValues();

            builder.InitialCatalog = databaseName;

            var conn = builder.ConnectionString;

            return new DbContextOptionsBuilder<TenantCatalogDb>()
                .UseSqlServer(conn)
                .Options;
        }

        public async Task<TenantCatalogDb> CreateDbContextByTenantContext()
        {
            if(_tenantContext.Type != TenantType.Admin)
            {
                throw new InvalidDbContextAccess();
            }

            var options = GetConnectionOptions(_tenantContext.DatabaseName);

            var tenantDbContext = new TenantCatalogDb(options);

            try
            {
                if (!await tenantDbContext.Database.CanConnectAsync())
                    throw new DatabaseConnectionException(_tenantContext.DatabaseName);
            }
            catch (SqlException)
            {
                throw new DatabaseConnectionException(_tenantContext.DatabaseName);
            }

            return tenantDbContext;
        }

        public async Task<TenantCatalogDb> CreateDbContextBySettingsValue()
        {
            var adminDatabaseName = _config["MultiTenant:AdminDatabaseName"] ?? 
                throw new UndefinedConfigurationValue(
                    message: "Missing Configuration: MultiTenant:AdminDatabaseName"
                );
            
            var options = GetConnectionOptions(adminDatabaseName);

            var tenantDbContext = new TenantCatalogDb(options);

            try
            {
                if (!await tenantDbContext.Database.CanConnectAsync())
                    throw new DatabaseConnectionException(adminDatabaseName);
            }
            catch (SqlException)
            {
                throw new DatabaseConnectionException(adminDatabaseName);
            }

            return tenantDbContext;
        }
    }
}