using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using ZooTech.Application.Common.Exceptions;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.Domain.Admin.Enums;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Tenant;
using ZooTech.Tests.Shared.Factories;

namespace ZooTech.Infrastructure.UnitTests.Tenant
{
    public class TenantProvisioningServiceUnitTests
    {
        private static (TenantCatalogDb Db, IConfiguration Config, Mock<ITenantDatabaseMigrator> Migrator) CreateDependencies()
        {
            var options = new DbContextOptionsBuilder<TenantCatalogDb>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var db = new TenantCatalogDb(options);

            var configData = new Dictionary<string, string>
            {
                { "ConnectionStrings:TenantTemplate", "Server=.;Database={DATABASE};Trusted_Connection=True;" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData!)
                .Build();

            var migratorMock = new Mock<ITenantDatabaseMigrator>();

            return (db, configuration, migratorMock);
        }

        [Fact]
        public async Task ProvisionAsync_Should_Return_True_When_Migration_Succeeds()
        {
            // Arrange
            var (db, config, migratorMock) = CreateDependencies();
            migratorMock.Setup(x => x.MigrateAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            var service = new TenantProvisioningService(db, config, migratorMock.Object);
            var cmd = TenantTestDataFactory.CreateValidCommand();

            // Act
            var result = await service.ProvisionAsync(cmd);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ProvisionAsync_Should_Throw_TenantProvisioningException_When_Migration_Fails()
        {
            // Arrange
            var (db, config, migratorMock) = CreateDependencies();
            migratorMock.Setup(x => x.MigrateAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Migration failed"));

            var service = new TenantProvisioningService(db, config, migratorMock.Object);
            var cmd = TenantTestDataFactory.CreateValidCommand();

            // Act
            var act = async () => await service.ProvisionAsync(cmd);

            // Assert
            await act.Should().ThrowAsync<TenantProvisioningException>();
        }
    }
}
