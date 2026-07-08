using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Infrastructure.Caching.ConcurrentCache;
using ZooTech.Infrastructure.Exceptions;

namespace ZooTech.Infrastructure.Persistence.Context
{
    public class GanaderiaDbContextFactory
        : IGanaderiaDbContextFactory
    {
        private readonly ITenantContext _tenantContext;
        private readonly IConfiguration _config;

        private readonly IConcurrentCache<string, DbContextOptions<GanaderiaDbContext>> _cache;

        public GanaderiaDbContextFactory(
            ITenantContext tenantContext, 
            IConfiguration config,
            IConcurrentCache<string, DbContextOptions<GanaderiaDbContext>> cache
        )
        {
            _tenantContext = tenantContext;
            _config = config;

            _cache = cache;
        }

        private DbContextOptions<GanaderiaDbContext> GetConnectionOptions(string databaseName, bool useAdminLogin = false)
        {
            var key = $"{useAdminLogin}:{databaseName}";

            return _cache.GetOrAdd(key, _ =>
            {
                var connectionName = useAdminLogin
                    ? "AdminTenantTemplate"
                    : "TenantTemplate";

                var template = _config.GetConnectionString(connectionName)!;

                if (string.IsNullOrWhiteSpace(template))
                    throw new UndefinedConfigurationValue(
                        message: $"Missing configuration: ConnectionStrings:{connectionName}"
                    );

                var builder = new SqlConnectionStringBuilder(template)
                {
                    InitialCatalog = databaseName
                };

                return new DbContextOptionsBuilder<GanaderiaDbContext>()
                    .UseSqlServer(builder.ConnectionString)
                    .Options;
            });
        }

        public GanaderiaDbContext CreateDbContextByTenantContext()
        {
            if(_tenantContext.Type != TenantType.Tenant)
            {
                throw new InvalidDbContextAccess();
            }

            var options = GetConnectionOptions(_tenantContext.DatabaseName);

            return new GanaderiaDbContext(options);
        }

        public GanaderiaDbContext CreateDbContextBySpecificDatabaseName(string databaseName, bool useAdminLogin = false)
        {
            if(_tenantContext.Type != TenantType.Admin)
            {
                throw new InvalidDbContextAccess();
            }

            var options = GetConnectionOptions(databaseName, useAdminLogin);

            return new GanaderiaDbContext(options);
        }
    }
}