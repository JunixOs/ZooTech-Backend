namespace ZooTech.Infrastructure.Persistence;

public interface ITenantDbContextFactory
{
    GanaderiaDbContext CreateDbContext();
}
