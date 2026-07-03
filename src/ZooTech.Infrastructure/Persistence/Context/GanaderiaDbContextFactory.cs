using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Infrastructure.Exceptions;

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

        public async Task<GanaderiaDbContext> CreateDbContext()
        {
            var template = _config.GetConnectionString("TenantTemplate");

            if (string.IsNullOrWhiteSpace(template))
                throw new UndefinedConfigurationValue();

            var builder = new SqlConnectionStringBuilder(template);

            if (string.IsNullOrWhiteSpace(_tenantContext.DatabaseName))
                throw new EmptyTenantContextValues();

            builder.InitialCatalog = _tenantContext.DatabaseName;

            var conn = builder.ConnectionString;

            var options = new DbContextOptionsBuilder<GanaderiaDbContext>()
                .UseSqlServer(conn)
                .Options;

            var ganaderiaDbContext = new GanaderiaDbContext(options);

            if (!await ganaderiaDbContext.Database.CanConnectAsync())
                throw new DatabaseConnectionException(_tenantContext.DatabaseName);

            return ganaderiaDbContext;
        }
    }
}