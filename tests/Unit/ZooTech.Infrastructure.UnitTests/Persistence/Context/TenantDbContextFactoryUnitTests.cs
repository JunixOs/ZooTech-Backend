using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Infrastructure.Exceptions;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.Infrastructure.UnitTests.Persistence.Context
{
    public class TenantDbContextFactoryUnitTests
    {
        [Fact]
        public async Task CreateDbContext_Should_Throw_When_Database_Unreachable()
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
            var act = async () => await factory.CreateDbContext();

            // Assert
            await act.Should().ThrowAsync<DatabaseConnectionException>();
        }
    }
}