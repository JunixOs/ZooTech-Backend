using Microsoft.Data.SqlClient;
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

        public TenantCatalogDb CreateDbContext()
        {
            var template = _config.GetConnectionString("TenantTemplate");

            var builder = new SqlConnectionStringBuilder(template);

            builder.InitialCatalog = _tenantContext.DatabaseName;

            var conn = builder.ConnectionString;

            var options = new DbContextOptionsBuilder<TenantCatalogDb>()
                .UseSqlServer(conn)
                .Options;

            return new TenantCatalogDb(options);
        }
    }
}