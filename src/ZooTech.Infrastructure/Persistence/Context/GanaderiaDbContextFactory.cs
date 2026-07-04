using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Domain.Shared.Enums;
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

        private DbContextOptions<GanaderiaDbContext> GetConnectionOptions(string databaseName)
        {
            var template = _config.GetConnectionString("TenantTemplate");

            if (string.IsNullOrWhiteSpace(template))
                throw new UndefinedConfigurationValue(
                    message: "Missing configuration: ConnectionStrings:TenantTemplate"
                );

            var builder = new SqlConnectionStringBuilder(template);

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new EmptyTenantContextValues();

            builder.InitialCatalog = databaseName;

            var conn = builder.ConnectionString;

            return new DbContextOptionsBuilder<GanaderiaDbContext>()
                .UseSqlServer(conn)
                .Options;
        }

        public async Task<GanaderiaDbContext> CreateDbContextByTenantContext()
        {
            if(_tenantContext.Type != TenantType.Tenant)
            {
                throw new InvalidDbContextAccess();
            }

            var options = GetConnectionOptions(_tenantContext.DatabaseName);

            var ganaderiaDbContext = new GanaderiaDbContext(options);

            try
            {
                if (!await ganaderiaDbContext.Database.CanConnectAsync())
                    throw new DatabaseConnectionException(_tenantContext.DatabaseName);
            }
            catch (SqlException)
            {
                throw new DatabaseConnectionException(_tenantContext.DatabaseName);
            }

            return ganaderiaDbContext;
        }

        public async Task<GanaderiaDbContext> CreateDbContextBySpecificDatabaseName(string databaseName)
        {
            if(_tenantContext.Type != TenantType.Admin)
            {
                throw new InvalidDbContextAccess();
            }

            var options = GetConnectionOptions(databaseName);

            var ganaderiaDbContext = new GanaderiaDbContext(options);

            try
            {
                if (!await ganaderiaDbContext.Database.CanConnectAsync())
                    throw new DatabaseConnectionException(databaseName);
            }
            catch (SqlException)
            {
                throw new DatabaseConnectionException(databaseName);
            }

            return ganaderiaDbContext;
        }
    }
}