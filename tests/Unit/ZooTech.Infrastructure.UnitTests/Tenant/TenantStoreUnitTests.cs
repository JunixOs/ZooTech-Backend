using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ZooTech.Application.Common.Gateway.Tenant;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;
using ZooTech.Infrastructure.Tenant;

namespace ZooTech.Infrastructure.UnitTests.Tenant
{
    public class TenantStoreUnitTests
    {
        private TenantCatalogDb CreateDbContext()
        {
            var options =
                new DbContextOptionsBuilder<TenantCatalogDb>()
                    .UseInMemoryDatabase(
                        Guid.NewGuid().ToString())
                    .Options;

            return new TenantCatalogDb(options);
        }
        [Fact]
        public async Task Should_Return_Null_When_Tenant_Not_Found()
        {
            // Arrange
        
            var db = CreateDbContext();

            var cache = new MemoryCache(new MemoryCacheOptions());

            var tenantStore = new TenantStore(db , cache);

            // Act

            var result = await tenantStore.GetBySubDomainAsync("tenant1");
        
            // Assert

            Assert.Null(result);
        }

        [Fact]
        public async Task Should_Return_Null_When_Tenant_Is_Inactive()
        {
            // Arrange
        
            var db = CreateDbContext();

            db.tenants.Add(
                new tenant
                {
                    id = 1,
                    code = "TENANT_1",
                    subdomain = "tenant1",
                    display_name = "Granja Tenant 1",
                    legal_name = "Granja Tenant 1 S.A.C",
                    email = "tenant1@gmail.com",
                    phone = "950678900",
                    status = "INACTIVE",

                    addresses = new List<address>
                    {
                        new address
                        {
                            id = 10,
                            country = "Perú",
                            state = "Huanuco",
                            province = "Huanuco",
                            city = "Huanuco",
                            address_line_1 = "Av. Abtao 1001",
                            created_at = DateTime.Now
                        }
                    },
                    tenant_database_connection =
                        new tenant_database_connection
                        {
                            id = 10,
                            is_active = true,
                            database_name =
                                "ZooTech_tenant1_Db",
                            created_at = DateTime.Now
                        },
                    created_at = DateTime.Now
                }
            );

            await db.SaveChangesAsync();

            var cache = new MemoryCache(new MemoryCacheOptions());

            var store = new TenantStore(db , cache);

            // Act
        
            var result = await store.GetBySubDomainAsync("tenant1");
            
            // Assert

            Assert.Null(result);
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
                Status = "ACTIVE",
                Email = "tenant1@gmail.com"
            };

            cache.Set(
                "tenant:tenant1",
                tenant,
                TimeSpan.FromMinutes(5)
            );

            var store = new TenantStore(db , cache);
        
            // Act

            var result = await store.GetBySubDomainAsync("tenant1");
        
            // Assert
            Assert.Equal(
                tenant.SubDomain,
                result!.SubDomain
            );;
        }

        [Fact]
        public async Task Should_Save_Tenant_In_Cache()
        {
            // Arrange
            var db = CreateDbContext();

            db.tenants.Add(
                new tenant
                {
                    id = 1,
                    code = "TENANT_1",
                    subdomain = "tenant1",
                    display_name = "Granja Tenant 1",
                    legal_name = "Granja Tenant 1 S.A.C",
                    email = "tenant1@gmail.com",
                    phone = "950678900",
                    status = "ACTIVE",

                    addresses = new List<address>
                    {
                        new address
                        {
                            id = 10,
                            country = "Perú",
                            state = "Huanuco",
                            province = "Huanuco",
                            city = "Huanuco",
                            address_line_1 = "Av. Abtao 1001",
                            created_at = DateTime.Now
                        }
                    },
                    tenant_database_connection =
                        new tenant_database_connection
                        {
                            id = 10,
                            is_active = true,
                            database_name =
                                "ZooTech_tenant1_Db",
                            created_at = DateTime.Now
                        },
                    created_at = DateTime.Now
                }
            );

            await db.SaveChangesAsync();
        
            var cache = new MemoryCache(new MemoryCacheOptions());

            var store = new TenantStore(db , cache);

            // Act
            await store.GetBySubDomainAsync("tenant1");
        
            // Assert
            var exists = cache.TryGetValue("tenant:tenant1" , out TenantInfo? tenantCached);

            Assert.True(exists);
            Assert.NotNull(tenantCached);
        }
    }
}