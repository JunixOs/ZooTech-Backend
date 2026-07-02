using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Context;

namespace ZooTech.Infrastructure.Persistence.Context
{
    public class GanaderiaDbContextFactory
        : IGanaderiaDbContextFactory
    {
        private readonly ITenantContext _tenantContext;
        public GanaderiaDbContextFactory(
            ITenantContext tenantContext,
            IConfiguration configuration
        )
        {
            _tenantContext = tenantContext;

            
        }

        public GanaderiaDbContext Create()
        {
            var options = new DbContextOptionsBuilder<GanaderiaDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new GanaderiaDbContext(options);
        }
    }
}