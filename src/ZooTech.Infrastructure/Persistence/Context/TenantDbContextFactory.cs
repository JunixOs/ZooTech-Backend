using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Infrastructure.Caching.ConcurrentCache;
using ZooTech.Infrastructure.Exceptions;
using ZooTech.Infrastructure.Persistence.Context;


namespace ZooTech.Infrastructure.Tenant
{
    public class TenantDbContextFactory : ITenantDbContextFactory
    {
        private readonly ITenantContext _tenantContext;
        private readonly IConfiguration _config;

        private readonly IConcurrentCache<string, DbContextOptions<TenantCatalogDb>> _cache;

        public TenantDbContextFactory(
            ITenantContext tenantContext, 
            IConfiguration config,
            IConcurrentCache<string, DbContextOptions<TenantCatalogDb>> cache
        )
        {
            _tenantContext = tenantContext;
            _config = config;
            
            _cache = cache;
        }

        private DbContextOptions<TenantCatalogDb> GetConnectionOptions(string databaseName)
        {
            var key = $"admin:{databaseName}";

            return _cache.GetOrAdd(key, _ =>
            {

                var template = _config.GetConnectionString("TenantTemplate")!;

                if (string.IsNullOrWhiteSpace(template))
                    throw new UndefinedConfigurationValue(
                        message: $"Missing configuration: ConnectionStrings:TenantTemplate"
                    );

                var builder = new SqlConnectionStringBuilder(template)
                {
                    InitialCatalog = databaseName
                };

                return new DbContextOptionsBuilder<TenantCatalogDb>()
                    .UseSqlServer(builder.ConnectionString)
                    .Options;
            });
        }

        public TenantCatalogDb CreateDbContextByTenantContext()
        {
            if(_tenantContext.Type != TenantType.Admin)
            {
                throw new InvalidDbContextAccess();
            }

            var options = GetConnectionOptions(_tenantContext.DatabaseName);

            return new TenantCatalogDb(options);
        }

        public TenantCatalogDb CreateDbContextBySettingsValue()
        {
            var adminDatabaseName = _config["MultiTenant:AdminDatabaseName"] ?? 
                throw new UndefinedConfigurationValue(
                    message: "Missing Configuration: MultiTenant:AdminDatabaseName"
                );
            
            var options = GetConnectionOptions(adminDatabaseName);

            return new TenantCatalogDb(options);
        }
    }
}