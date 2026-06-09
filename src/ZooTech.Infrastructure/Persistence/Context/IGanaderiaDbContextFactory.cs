namespace ZooTech.Infrastructure.Persistence.Context
{
    public interface IGanaderiaDbContextFactory
    {
        GanaderiaDbContext Create(string connectionString);
    }
}