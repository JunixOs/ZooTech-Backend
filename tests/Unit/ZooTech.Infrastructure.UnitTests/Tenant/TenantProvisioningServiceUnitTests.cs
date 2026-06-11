using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using ZooTech.Application.Modules.Module_Tenancing.UseCases.CreateTenant;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.Infrastructure.UnitTests.Tenant
{
    public class TenantProvisioningServiceUnitTests
    {
        private CreateTenantCommand CreateFakeCommand()
        {
            return new CreateTenantCommand
            {
                Code = "ganaderia-central",
                SubDomain = "ganaderia-central",
                DisplayName = "Ganadería Central",
                LegalName = "Ganadería Central SAC",

                Email = "admin@ganaderia.com",
                Phone = "+51999999999",

                TimeZone = "America/Lima",

                Status = "TRIAL",

                Metadata = "{}",

                CreatedAt = DateTime.Now,

                TenantAddress = new TenantAddress
                {
                    Country = "Perú",
                    State = "Lima",
                    Province = "Lima",
                    City = "Lima",

                    AddressLine_1 = "Av. Principal 123",
                    AddressLine_2 = "Oficina 501",

                    Metadata = "{}",

                    CreatedAt = DateTime.Now
                },

                TenantBranding = new TenantBranding
                {
                    PrimaryColor = "#2563EB",
                    SecondaryColor = "#1E293B",

                    LogoUrl = "https://cdn.zoo-tech.com/logo.png",

                    Metadata = "{}",

                    CreatedAt = DateTime.Now
                },

                TenantDatabaseConnection = new TenantDatabaseConnection
                {
                    IsActive = true
                }
            };
        }

        [Fact]
        public async Task ProvisionAsync_Should_Return_True_When_Migration_Succeeds()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TenantCatalogDb>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var tenantCatalogDb = new TenantCatalogDb(options);

            var configData = new Dictionary<string, string>
            {
                {
                    "ConnectionStrings:TenantTemplate",
                    "Server=.;Database={DATABASE};Trusted_Connection=True;"
                }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData!)
                .Build();

            var tenantDbMigratorMock = new Mock<ITenantDatabaseMigrator>();

            tenantDbMigratorMock
                .Setup(x => x.MigrateAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var factoryMock = new Mock<IGanaderiaDbContextFactory>();

            var service = new TenantProvisioningService(
                tenantCatalogDb,
                configuration,
                tenantDbMigratorMock.Object
            );

            var cmd = CreateFakeCommand();

            // Act
            var result = await service.ProvisionAsync(cmd);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ProvisionAsync_Should_Return_False_When_Migration_Fails()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TenantCatalogDb>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var tenantCatalogDb = new TenantCatalogDb(options);

            var configData = new Dictionary<string, string>
            {
                {
                    "ConnectionStrings:TenantTemplate",
                    "Server=.;Database={DATABASE};Trusted_Connection=True;"
                }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData!)
                .Build();

            var tenantDbMigratorMock = new Mock<ITenantDatabaseMigrator>();

            tenantDbMigratorMock
                .Setup(x => x.MigrateAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Migration failed"));

            var service = new TenantProvisioningService(
                tenantCatalogDb,
                configuration,
                tenantDbMigratorMock.Object
            );

            var cmd = CreateFakeCommand();

            // Act
            var result = await service.ProvisionAsync(cmd);

            // Assert
            Assert.False(result);
        }
    }
}