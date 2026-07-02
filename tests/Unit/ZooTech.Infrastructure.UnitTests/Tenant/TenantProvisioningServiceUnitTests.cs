using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.Infrastructure.Exceptions;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Tenant;
using ZooTech.Tests.Shared.Factories;

namespace ZooTech.Infrastructure.UnitTests.Tenant
{
    public class TenantProvisioningServiceUnitTests
    {
        private static (Mock<ITenantDbContextFactory> DbFactoryMock, IConfiguration Config, Mock<ITenantDatabaseMigrator> Migrator) CreateDependencies()
        {
            var options = new DbContextOptionsBuilder<TenantCatalogDb>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var db = new TenantCatalogDb(options);

            var dbFactoryMock = new Mock<ITenantDbContextFactory>();
            dbFactoryMock.Setup(f => f.CreateDbContext()).Returns(db);

            var configData = new Dictionary<string, string>
            {
                { "ConnectionStrings:TenantTemplate", "Server=.;Database={DATABASE};Trusted_Connection=True;" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData!)
                .Build();

            var migratorMock = new Mock<ITenantDatabaseMigrator>();

            return (dbFactoryMock, configuration, migratorMock);
        }

        [Fact]
        public async Task ProvisionAsync_Should_Executes_Normally_When_Migration_Succeeds()
        {
            // Arrange
            var (dbFactoryMock, config, migratorMock) = CreateDependencies();
            migratorMock.Setup(x => x.MigrateAsync()).Returns(Task.CompletedTask);

            var service = new TenantProvisioningService(dbFactoryMock.Object, config, migratorMock.Object);
            var cmd = TenantTestDataFactory.CreateValidCommand();

            // Act
            await service.ProvisionAsync(cmd);

            // Assert
            migratorMock.Verify(m => m.MigrateAsync(), Times.Once);
        }

        [Fact]
        public async Task ProvisionAsync_Should_Throw_TenantProvisioningException_When_Migration_Fails()
        {
            // Arrange
            var (dbFactoryMock, config, migratorMock) = CreateDependencies();
            migratorMock.Setup(x => x.MigrateAsync()).ThrowsAsync(new Exception("Migration failed"));

            var service = new TenantProvisioningService(dbFactoryMock.Object, config, migratorMock.Object);
            var cmd = TenantTestDataFactory.CreateValidCommand();

            // Act
            var act = async () => await service.ProvisionAsync(cmd);

            // Assert
            await act.Should().ThrowAsync<TenantProvisioningException>();
        }
    }
}
