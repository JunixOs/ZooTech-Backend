using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.Infrastructure.UnitTests.Tenant;

public class TenantDatabaseMigratorTests
{
    [Fact]
    public async Task MigrateAsync_Should_Invoke_Factory_With_ConnectionString()
    {
        // Arrange
        var factoryMock = new Mock<IGanaderiaDbContextFactory>();
        var options = new DbContextOptionsBuilder<GanaderiaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new GanaderiaDbContext(options);

        factoryMock.Setup(f => f.Create(It.IsAny<string>())).Returns(context);

        var migrator = new TenantDatabaseMigrator(factoryMock.Object);
        var connString = "Server=.;Database=TestDb;Trusted_Connection=True;";

        // Act
        var act = async () => await migrator.MigrateAsync(connString);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        factoryMock.Verify(f => f.Create(connString), Times.Once);
    }
}
