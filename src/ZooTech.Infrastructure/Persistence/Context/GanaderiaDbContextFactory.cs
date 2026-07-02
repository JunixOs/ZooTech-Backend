using Microsoft.Data.SqlClient;
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
        private readonly IConfiguration _config;

        public GanaderiaDbContextFactory(ITenantContext tenantContext, IConfiguration config)
        {
            _tenantContext = tenantContext;
            _config = config;
        }

        public GanaderiaDbContext CreateDbContext()
        {
            var template = _config.GetConnectionString("TenantTemplate");

            var builder = new SqlConnectionStringBuilder(template);

            builder.InitialCatalog = _tenantContext.DatabaseName;

            var conn = builder.ConnectionString;

            var options = new DbContextOptionsBuilder<GanaderiaDbContext>()
                .UseSqlServer(conn)
                .Options;

            return new GanaderiaDbContext(options);
        }
    }
}