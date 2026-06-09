using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Domain.Enums;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;
using ZooTech.Infrastructure.Tenant;
using ZooTech.Tests.Shared.Factories;

namespace ZooTech.Infrastructure.UnitTests.Tenant
{
    public class TenantStoreUnitTests
    {
        private static TenantCatalogDb CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<TenantCatalogDb>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TenantCatalogDb(options);
        }

        [Fact]
        public async Task Should_Return_Null_When_Tenant_Not_Found()
        {
            // Arrange
            var db = CreateDbContext();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var tenantStore = new TenantStore(db, cache);

            // Act
            var result = await tenantStore.GetBySubDomainAsync("tenant1");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Should_Return_Null_When_Tenant_Is_Inactive()
        {
            // Arrange
            var db = CreateDbContext();
            db.tenants.Add(TenantTestDataFactory.CreateTenantEntity(status: TenantStatus.INACTIVE.ToString()));
            await db.SaveChangesAsync();

            var cache = new MemoryCache(new MemoryCacheOptions());
            var store = new TenantStore(db, cache);

            // Act
            var result = await store.GetBySubDomainAsync("tenant1");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Should_Return_Tenant_From_Cache()
        {
            // Arrange
            var db = CreateDbContext();
            var cache = new MemoryCache(new MemoryCacheOptions());

            var tenant = new TenantInfo
            {
                Id = 1,
                SubDomain = "tenant1",
                Code = "TENANT_1",
                DatabaseName = "ZooTech_tenant1_Db",
                Status = TenantStatus.ACTIVE,
                Email = "tenant1@gmail.com"
            };

            cache.Set("tenant:tenant1", tenant, TimeSpan.FromMinutes(5));

            var store = new TenantStore(db, cache);

            // Act
            var result = await store.GetBySubDomainAsync("tenant1");

            // Assert
            result.Should().NotBeNull();
            result!.SubDomain.Should().Be(tenant.SubDomain);
        }

        [Fact]
        public async Task Should_Save_Tenant_In_Cache()
        {
            // Arrange
            var db = CreateDbContext();
            db.tenants.Add(TenantTestDataFactory.CreateTenantEntity());
            await db.SaveChangesAsync();

            var cache = new MemoryCache(new MemoryCacheOptions());
            var store = new TenantStore(db, cache);

            // Act
            await store.GetBySubDomainAsync("tenant1");

            // Assert
            var exists = cache.TryGetValue("tenant:tenant1", out TenantInfo? tenantCached);
            exists.Should().BeTrue();
            tenantCached.Should().NotBeNull();
        }
    }
}
