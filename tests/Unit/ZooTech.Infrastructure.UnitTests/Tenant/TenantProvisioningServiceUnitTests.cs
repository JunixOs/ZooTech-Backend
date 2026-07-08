using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Infrastructure.Exceptions;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Tenant;
using ZooTech.Tests.Shared.Factories;

namespace ZooTech.Infrastructure.UnitTests.Tenant
{
    public class TenantProvisioningServiceUnitTests
    {
        private static (Mock<ITenantDbContextFactory> DbFactoryMock, Mock<ITenantDatabaseMigrator> MigratorMock, Mock<ITenantDatabaseCreator> CreatorMock) CreateDependencies()
        {
            var options = new DbContextOptionsBuilder<TenantCatalogDb>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var db = new TenantCatalogDb(options);

            var dbFactoryMock = new Mock<ITenantDbContextFactory>();
            dbFactoryMock.Setup(f => f.CreateDbContextByTenantContext()).Returns(db);

            var migratorMock = new Mock<ITenantDatabaseMigrator>();
            var creatorMock = new Mock<ITenantDatabaseCreator>();

            return (dbFactoryMock, migratorMock, creatorMock);
        }

        [Fact]
        public async Task ProvisionAsync_Should_Executes_Normally_When_Migration_Succeeds()
        {
            // Arrange
            var (dbFactoryMock, migratorMock, creatorMock) = CreateDependencies();
            migratorMock.Setup(x => x.MigrateAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            var service = new TenantProvisioningService(dbFactoryMock.Object, migratorMock.Object, creatorMock.Object);
            var cmd = TenantTestDataFactory.CreateValidCommand();

            // Act
            await service.ProvisionAsync(cmd);

            // Assert
            migratorMock.Verify(m => m.MigrateAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ProvisionAsync_Should_Throw_TenantProvisioningException_When_Migration_Fails()
        {
            // Arrange
            var (dbFactoryMock, migratorMock, creatorMock) = CreateDependencies();
            migratorMock.Setup(x => x.MigrateAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Migration failed"));

            var service = new TenantProvisioningService(dbFactoryMock.Object, migratorMock.Object, creatorMock.Object);
            var cmd = TenantTestDataFactory.CreateValidCommand();

            // Act
            var act = async () => await service.ProvisionAsync(cmd);

            // Assert
            await act.Should().ThrowAsync<TenantProvisioningException>();
        }
    }
}
