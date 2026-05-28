using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ZooTech.Infrastructure.Persistence.Context
{
    public class GanaderiaDbContextFactory
        : IGanaderiaDbContextFactory
    {
        public GanaderiaDbContext Create(string connectionString)
        {
            var options = new DbContextOptionsBuilder<GanaderiaDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new GanaderiaDbContext(options);
        }
    }
}