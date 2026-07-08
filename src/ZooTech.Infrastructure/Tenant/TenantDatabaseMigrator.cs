using Microsoft.EntityFrameworkCore;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Tenant
{
    public class TenantDatabaseMigrator : ITenantDatabaseMigrator
    {
        private readonly IGanaderiaDbContextFactory _ganaderiaDbContextFactory;

        public TenantDatabaseMigrator(IGanaderiaDbContextFactory ganaderiaDbContextFactory)
        {
            _ganaderiaDbContextFactory = ganaderiaDbContextFactory;
        }

        public async Task MigrateAsync(string tenantDatabaseName)
        {
            var context = _ganaderiaDbContextFactory.CreateDbContextBySpecificDatabaseName(tenantDatabaseName , true);

            if (context.Database.IsRelational())
            {
                await context.Database.MigrateAsync();
            }
        }
    }
}