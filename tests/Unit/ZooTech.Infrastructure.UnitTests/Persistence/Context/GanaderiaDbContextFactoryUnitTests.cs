using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using ZooTech.Application.Common.Gateway.Context;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.Infrastructure.UnitTests.Persistence.Context;

public class GanaderiaDbContextFactoryUnitTests
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

        var tenantContext = new Mock<ITenantContext>();

        var inMemorySettings = new Dictionary<string , string>
        {
            {
                "ConnectionStrings:TenantTemplate",
                "Server=.;Database=TestDb;Trusted_Connection=True;"
            }
        };

        tenantContext.Setup(tc => tc.SetTenant(
            100,
            "tenant-01",
            "Legal",
            "Display",
            "tenant",
            "tenant-01",
            "TestDb"
        ));

        factoryMock.Setup(f => f.CreateDbContext()).Returns(context);

        var migrator = new TenantDatabaseMigrator(factoryMock.Object);
        
        // Act
        var act = async () => await migrator.MigrateAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        factoryMock.Verify(f => f.CreateDbContext(), Times.Once);
    }
}
