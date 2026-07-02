using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.Infrastructure.UnitTests.Persistence.Context
{
    public class TenantDbContextFactoryUnitTests
    {
        [Fact]
        public void CreateDbContext_Should_Create_Context_With_Tenant_Database()
        {
            // Arrange
            var tenantContextMock = new Mock<ITenantContext>();

            tenantContextMock
                .Setup(t => t.DatabaseName)
                .Returns("Ganaderia_Test");

            var inMemorySettings = new Dictionary<string , string>
            {
                {
                    "ConnectionStrings:TenantTemplate",
                    "Server=.;TrustServerCertificate=True;User Id=userfalse;Password=123456;MultipleActiveResultSets=true"
                }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            var factory = new TenantDbContextFactory(
                tenantContextMock.Object,
                configuration
            );

            // Act
            var context = factory.CreateDbContext();

            // Assert
            var connectionString = context.Database.GetConnectionString();
            var builder = new SqlConnectionStringBuilder(connectionString);

            Assert.NotNull(context);
            Assert.Contains("Ganaderia_Test" , builder.InitialCatalog);
        }
    }
}