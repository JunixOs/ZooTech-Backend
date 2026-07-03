namespace ZooTech.Infrastructure.Persistence.Context
{
    public interface IGanaderiaDbContextFactory
    {
        Task<GanaderiaDbContext> CreateDbContext();
    }
}