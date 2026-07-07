// Infrastructure/
// Persistence/TenantCatalogDb.cs

// Infrastructure/
// Persistence/TenantCatalogDb.cs
using Microsoft.EntityFrameworkCore;
namespace ZooTech.Infrastructure.Persistence.Context;

public class TenantCatalogDb : DbContext
{
    public DbSet<TenantEntity> Tenants => Set<TenantEntity>();
    public TenantCatalogDb(DbContextOptions<TenantCatalogDb> options)
    : base(options) { }
}
// Entidad ORM
public class TenantEntity
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = default!;
    public string DatabaseName { get; set; } = default!;
    public bool IsActive { get; set; }
}