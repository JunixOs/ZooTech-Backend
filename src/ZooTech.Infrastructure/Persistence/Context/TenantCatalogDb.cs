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

    public virtual DbSet<feature> features { get; set; }

    public virtual DbSet<refresh_token> refresh_tokens { get; set; }

    public virtual DbSet<rule_definition> rule_definitions { get; set; }

    public virtual DbSet<setting_definition> setting_definitions { get; set; }

    public virtual DbSet<tenant> tenants { get; set; }

    public virtual DbSet<tenant_branding> tenant_brandings { get; set; }

    public virtual DbSet<tenant_business_rule> tenant_business_rules { get; set; }

    public virtual DbSet<tenant_database_connection> tenant_database_connections { get; set; }

    public virtual DbSet<tenant_feature> tenant_features { get; set; }

    public virtual DbSet<tenant_setting> tenant_settings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<address>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__addresse__3213E83FA9ED44E4");

            entity.Property(e => e.created_at).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.updated_at).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.tenant).WithMany(p => p.addresses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_addresses_tenants");
        });

        modelBuilder.Entity<admin_user>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__admin_us__3213E83F1F07B287");

            entity.Property(e => e.created_at).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.updated_at).HasDefaultValueSql("(sysdatetimeoffset())");
        });

        modelBuilder.Entity<feature>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__features__3213E83FCFCB01E1");

            entity.Property(e => e.created_at).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.updated_at).HasDefaultValueSql("(sysdatetimeoffset())");
        });

        modelBuilder.Entity<refresh_token>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__refresh___3213E83FB0E1F619");

            entity.Property(e => e.created_at).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.admin_user).WithMany(p => p.refresh_tokens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_refresh_tokens_admin_users");
        });

        modelBuilder.Entity<rule_definition>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__rule_def__3213E83F312ED06F");

            entity.Property(e => e.created_at).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.updated_at).HasDefaultValueSql("(sysdatetimeoffset())");
        });

        modelBuilder.Entity<setting_definition>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__setting___3213E83FF8C698B8");

            entity.Property(e => e.created_at).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.is_required).HasDefaultValue(false);
            entity.Property(e => e.is_sensitive).HasDefaultValue(false);
            entity.Property(e => e.updated_at).HasDefaultValueSql("(sysdatetimeoffset())");
        });

        modelBuilder.Entity<tenant>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__tenants__3213E83F90873C7A");

            entity.Property(e => e.created_at).HasDefaultValueSql("(sysdatetimeoffset())", "DF_tenants_created_at");
            entity.Property(e => e.status).HasDefaultValue("TRIAL", "DF_tenants_status");
            entity.Property(e => e.updated_at).HasDefaultValueSql("(sysdatetimeoffset())", "DF_tenants_updated_at");
        });

        modelBuilder.Entity<tenant_branding>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__tenant_b__3213E83F73C9C569");

            entity.Property(e => e.created_at).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.updated_at).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.tenant).WithOne(p => p.tenant_branding)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tenant_branding_tenants");
        });

        modelBuilder.Entity<tenant_business_rule>(entity =>
        {
            entity.HasOne(d => d.rule_definition).WithMany(p => p.tenant_business_rules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tenant_business_rules_rules");

            entity.HasOne(d => d.tenant).WithMany(p => p.tenant_business_rules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tenant_business_rules_tenants");
        });

        modelBuilder.Entity<tenant_database_connection>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__tenant_d__3213E83F5018054E");

            entity.Property(e => e.created_at).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.updated_at).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.tenant).WithOne(p => p.tenant_database_connection)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tenant_database_connections_tenants");
        });

        modelBuilder.Entity<tenant_feature>(entity =>
        {
            entity.Property(e => e.is_enabled).HasDefaultValue(true);

            entity.HasOne(d => d.feature).WithMany(p => p.tenant_features)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tenant_features_features");

            entity.HasOne(d => d.tenant).WithMany(p => p.tenant_features)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tenant_features_tenants");
        });

        modelBuilder.Entity<tenant_setting>(entity =>
        {
            entity.HasOne(d => d.setting_definition).WithMany(p => p.tenant_settings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tenant_settings_definitions");

            entity.HasOne(d => d.tenant).WithMany(p => p.tenant_settings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tenant_settings_tenants");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
