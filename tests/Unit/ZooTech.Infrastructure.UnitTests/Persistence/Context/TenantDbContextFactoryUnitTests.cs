using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Domain.Shared.Enums;
using ZooTech.Infrastructure.Caching.ConcurrentCache;
using ZooTech.Infrastructure.Exceptions;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.Infrastructure.UnitTests.Persistence.Context
{
    public class TenantDbContextFactoryUnitTests
    {
        [Fact]
        public void CreateDbContext_Should_Throw_When_Database_Unreachable()
        {
            // Arrange
            var tenantContextMock = new Mock<ITenantContext>();

            tenantContextMock
                .Setup(t => t.DatabaseName)
                .Returns("Ganaderia_Test");

            tenantContextMock
                .Setup(t => t.Type)
                .Returns(TenantType.Admin);

            var inMemorySettings = new Dictionary<string , string>
            {
                {
                    "ConnectionStrings:TenantTemplate",
                    "Server=.;TrustServerCertificate=True;User Id=userfalse;Password=123456;MultipleActiveResultSets=true"
                },
                {
                    "MultiTenant:AdminDatabaseName",
                    "ZooTech_Admin_Test"
                }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            var cacheMock = new Mock<IConcurrentCache<string, DbContextOptions<TenantCatalogDb>>>();
            cacheMock
                .Setup(c => c.GetOrAdd(It.IsAny<string>(), It.IsAny<Func<string, DbContextOptions<TenantCatalogDb>>>()))
                .Returns((string key, Func<string, DbContextOptions<TenantCatalogDb>> factory) => factory(key));

            var factory = new TenantDbContextFactory(
                tenantContextMock.Object,
                configuration,
                cacheMock.Object
            );

            // Act
            var act = () => factory.CreateDbContextByTenantContext();

            // Assert
            act.Should().Throw<DatabaseConnectionException>();
        }
    }
}