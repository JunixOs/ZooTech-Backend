// Infrastructure/
// Persistence/TenantCatalogDb.cs

// Infrastructure/
// Persistence/TenantCatalogDb.cs
using Microsoft.EntityFrameworkCore;
using ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;
namespace ZooTech.Infrastructure.Persistence.Context;

public class TenantCatalogDb : DbContext
{
    public DbSet<TenantEntity> TenantEntity => Set<TenantEntity>();
    public DbSet<AddressEntity> AddressEntity => Set<AddressEntity>();
    public DbSet<TenantDatabaseConnectionEntity> TenantDatabaseConnectionEntity => Set<TenantDatabaseConnectionEntity>();
    public TenantCatalogDb(DbContextOptions<TenantCatalogDb> options)
    : base(options) { }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TenantEntity>()
                .HasOne(t => t.Address)
                .WithOne(a => a.TenantEntity)
                .HasForeignKey<AddressEntity>(
                    a => a.TenantId);

            modelBuilder.Entity<TenantEntity>()
                .HasOne(t => t.TenantDatabaseConnection)
                .WithOne(c => c.TenantEntity)
                .HasForeignKey<
                    TenantDatabaseConnectionEntity>(
                        c => c.TenantId);

            modelBuilder.Entity<TenantEntity>()
                .HasOne(t => t.TenantBranding)
                .WithOne(b => b.Tenant)
                .HasForeignKey<
                    TenantBrandingEntitiy>(
                        b => b.TenantId);
        }
}
