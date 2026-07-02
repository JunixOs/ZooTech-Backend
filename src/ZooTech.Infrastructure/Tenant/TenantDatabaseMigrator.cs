using Microsoft.EntityFrameworkCore;
using ZooTech.Infrastructure.Persistence.Context;

namespace ZooTech.Infrastructure.Tenant
{
    public class TenantDatabaseMigrator : ITenantDatabaseMigrator
    {
        private readonly GanaderiaDbContext _ganaderiaDbContext;

        public TenantDatabaseMigrator(IGanaderiaDbContextFactory ganaderiaDbContextFactory)
        {
            _ganaderiaDbContext = ganaderiaDbContextFactory.CreateDbContext();
        }

        public async Task MigrateAsync()
        {
            await _ganaderiaDbContext.Database.MigrateAsync();
        }
    }
}