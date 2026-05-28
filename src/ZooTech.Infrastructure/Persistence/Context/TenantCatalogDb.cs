using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ZooTech.Infrastructure.Persistence.Entities.MainTenantsDb;

namespace ZooTech.Infrastructure.Persistence.Context;

public partial class TenantCatalogDb : DbContext
{
    public TenantCatalogDb(DbContextOptions<TenantCatalogDb> options)
        : base(options)
    {
    }

    public virtual DbSet<address> addresses { get; set; }

    public virtual DbSet<admin_user> admin_users { get; set; }

    public virtual DbSet<refresh_token> refresh_tokens { get; set; }

    public virtual DbSet<tenant> tenants { get; set; }

    public virtual DbSet<tenant_branding> tenant_brandings { get; set; }

    public virtual DbSet<tenant_database_connection> tenant_database_connections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<address>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__addresse__3213E83F42BFDF40");

            entity.Property(e => e.created_at).HasDefaultValueSql("(sysutcdatetime())", "DF_addresses_created_at");
            entity.Property(e => e.updated_at).HasDefaultValueSql("(sysutcdatetime())", "DF_addresses_updated_at");

            entity.HasOne(d => d.tenant).WithOne(p => p.address).HasConstraintName("FK_addresses_tenants");
        });

        modelBuilder.Entity<admin_user>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__admin_us__3213E83F92D9F731");

            entity.Property(e => e.created_at).HasDefaultValueSql("(sysutcdatetime())", "DF_admin_users_created_at");
            entity.Property(e => e.is_active).HasDefaultValue(true, "DF_admin_users_is_active");
            entity.Property(e => e.updated_at).HasDefaultValueSql("(sysutcdatetime())", "DF_admin_users_updated_at");
        });

        modelBuilder.Entity<refresh_token>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__refresh___3213E83FA25580CD");

            entity.Property(e => e.created_at).HasDefaultValueSql("(sysutcdatetime())", "DF_refresh_tokens_created_at");

            entity.HasOne(d => d.admin_user).WithMany(p => p.refresh_tokens).HasConstraintName("FK_refresh_tokens_admin_users");
        });

        modelBuilder.Entity<tenant>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__tenants__3213E83F46AE2181");

            entity.Property(e => e.created_at).HasDefaultValueSql("(sysutcdatetime())", "DF_tenants_created_at");
            entity.Property(e => e.status).HasDefaultValue("TRIAL", "DF_tenants_status");
            entity.Property(e => e.updated_at).HasDefaultValueSql("(sysutcdatetime())", "DF_tenants_updated_at");
        });

        modelBuilder.Entity<tenant_branding>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__tenant_b__3213E83F1EFE520D");

            entity.Property(e => e.created_at).HasDefaultValueSql("(sysutcdatetime())", "DF_tenant_brandings_created_at");
            entity.Property(e => e.updated_at).HasDefaultValueSql("(sysutcdatetime())", "DF_tenant_brandings_updated_at");

            entity.HasOne(d => d.tenant).WithOne(p => p.tenant_branding).HasConstraintName("FK_tenant_brandings_tenants");
        });

        modelBuilder.Entity<tenant_database_connection>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__tenant_d__3213E83F18A66298");

            entity.Property(e => e.created_at).HasDefaultValueSql("(sysutcdatetime())", "DF_tenant_database_connections_created_at");
            entity.Property(e => e.updated_at).HasDefaultValueSql("(sysutcdatetime())", "DF_tenant_database_connections_updated_at");

            entity.HasOne(d => d.tenant).WithOne(p => p.tenant_database_connection).HasConstraintName("FK_tenant_database_connections_tenants");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
