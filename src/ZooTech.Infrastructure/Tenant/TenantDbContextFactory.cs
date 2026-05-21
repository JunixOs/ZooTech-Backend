using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Context;

using ZooTech.Infrastructure.Persistence.Context;


namespace ZooTech.Infrastructure.Tenant
{
    public class TenantDbContextFactory : ITenantDbContextFactory
    {
        private readonly ITenantContext _tenantContext;
        private readonly IConfiguration _config;

        public TenantDbContextFactory(ITenantContext tenantContext, IConfiguration config)
        {
            _tenantContext = tenantContext;
            _config = config;
        }

        public GanaderiaDbContext CreateDbContext()
        {
            var template = _config.GetConnectionString("TenantTemplate");
            var conn = template.Replace("{DATABASE}", _tenantContext.DatabaseName);

            var options = new DbContextOptionsBuilder<GanaderiaDbContext>()
                .UseSqlServer(conn)
                .Options;

            return new GanaderiaDbContext(options);
        }
    }
}