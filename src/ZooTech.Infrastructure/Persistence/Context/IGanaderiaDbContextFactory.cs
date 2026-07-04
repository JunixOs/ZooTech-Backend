namespace ZooTech.Infrastructure.Persistence.Context
{
    public interface IGanaderiaDbContextFactory
    {
        Task<GanaderiaDbContext> CreateDbContextByTenantContext();
        Task<GanaderiaDbContext> CreateDbContextBySpecificDatabaseName(string databaseName);
    }
}