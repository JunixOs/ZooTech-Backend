using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Infrastructure.Exceptions;
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

        public async Task<TenantCatalogDb> CreateDbContext()
        {
            var template = _config.GetConnectionString("TenantTemplate");

            if (string.IsNullOrWhiteSpace(template))
                throw new UndefinedConfigurationValue();

            var builder = new SqlConnectionStringBuilder(template);

            if (string.IsNullOrWhiteSpace(_tenantContext.DatabaseName))
                throw new EmptyTenantContextValues();


            builder.InitialCatalog = _tenantContext.DatabaseName;

            var conn = builder.ConnectionString;

            var options = new DbContextOptionsBuilder<TenantCatalogDb>()
                .UseSqlServer(conn)
                .Options;


            var tenantDbContext = new TenantCatalogDb(options);

            if (!await tenantDbContext.Database.CanConnectAsync())
                throw new DatabaseConnectionException(_tenantContext.DatabaseName);

            return tenantDbContext;
        }
    }
}