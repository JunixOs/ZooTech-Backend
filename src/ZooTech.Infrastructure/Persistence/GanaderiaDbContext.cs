using Microsoft.EntityFrameworkCore;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence;

public class GanaderiaDbContext : DbContext
{
    public DbSet<AnimalEntity> Animals => Set<AnimalEntity>();

    public GanaderiaDbContext(DbContextOptions<GanaderiaDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GanaderiaDbContext).Assembly);
    }
}