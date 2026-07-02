namespace ZooTech.Infrastructure.Persistence.Context
{
    public interface IGanaderiaDbContextFactory
    {
        GanaderiaDbContext CreateDbContext();
    }
}