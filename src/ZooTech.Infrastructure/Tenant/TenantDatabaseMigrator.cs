using Microsoft.EntityFrameworkCore;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Tenant
{
    public class TenantDatabaseMigrator : ITenantDatabaseMigrator
    {
        private readonly GanaderiaDbContextFactory _ganaderiaDbContextFactory;

        public TenantDatabaseMigrator(GanaderiaDbContextFactory ganaderiaDbContextFactory)
        {
            _ganaderiaDbContextFactory = ganaderiaDbContextFactory;
        }

        public async Task MigrateAsync(string connectionString)
        {
            await using var context = _ganaderiaDbContextFactory.Create(connectionString);

            await context.Database.MigrateAsync();
        }
    }
}