namespace ZooTech.Infrastructure.Persistence.Context
{
    public interface IGanaderiaDbContextFactory
    {
        GanaderiaDbContext CreateDbContextByTenantContext();
        GanaderiaDbContext CreateDbContextBySpecificDatabaseName(string databaseName , bool useAdminLogin = false);
    }
}