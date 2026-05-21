using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ZooTech.Infrastructure.Persistence.Context
{
    public class GanaderiaDbContextFactory
        : IDesignTimeDbContextFactory<GanaderiaDbContext>
    {
        public GanaderiaDbContext CreateDbContext(
            string[] args)
        {
            var optionsBuilder =
                new DbContextOptionsBuilder<GanaderiaDbContext>();

            optionsBuilder.UseSqlServer(
                "Server=69.164.246.85,1433;TrustServerCertificate=True;User Id=yonel.ordonez;Password=ZooTech@2026#06;MultipleActiveResultSets=true");

            return new GanaderiaDbContext(
                optionsBuilder.Options);
        }
    }
}