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

    public virtual DbSet<business_setting> business_settings { get; set; }

    public virtual DbSet<business_setting_parameter> business_setting_parameters { get; set; }

    public virtual DbSet<business_setting_parameter_value> business_setting_parameter_values { get; set; }

    public virtual DbSet<feature> features { get; set; }

    public virtual DbSet<refresh_token> refresh_tokens { get; set; }

    public virtual DbSet<rule_definition> rule_definitions { get; set; }

    public virtual DbSet<tenant> tenants { get; set; }

    public virtual DbSet<tenant_branding> tenant_brandings { get; set; }

    public virtual DbSet<tenant_business_rule> tenant_business_rules { get; set; }

    public virtual DbSet<tenant_database_connection> tenant_database_connections { get; set; }

    public virtual DbSet<tenant_feature> tenant_features { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<address>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__addresse__3213E83F410EB871");

            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.tenant).WithOne(p => p.address)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_addresses_tenants");
        });

        modelBuilder.Entity<admin_user>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__admin_us__3213E83FC486451E");

            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.is_active).HasDefaultValue(true);
        });

        modelBuilder.Entity<business_setting>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__business__3213E83F14AAF967");

            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.is_active).HasDefaultValue(true);
        });

        modelBuilder.Entity<business_setting_parameter>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__business__3213E83FB7178823");

            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.is_active).HasDefaultValue(true);

            entity.HasOne(d => d.business_setting).WithMany(p => p.business_setting_parameters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_business_setting_parameters_business_settings");
        });

        modelBuilder.Entity<business_setting_parameter_value>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__business__3213E83F9656E3AA");

            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.is_active).HasDefaultValue(true);

            entity.HasOne(d => d.business_setting_parameter).WithMany(p => p.business_setting_parameter_values)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_business_setting_parameter_values_parameters");
        });

        modelBuilder.Entity<feature>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__features__3213E83F36F48103");

            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.is_active).HasDefaultValue(true);
        });

        modelBuilder.Entity<refresh_token>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__refresh___3213E83FF24B3943");

            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.admin_user).WithMany(p => p.refresh_tokens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_refresh_tokens_admin_users");
        });

        modelBuilder.Entity<rule_definition>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__rule_def__3213E83F46A7485F");

            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<tenant>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__tenants__3213E83F7B29AD44");

            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.status).HasDefaultValue("TRIAL", "DF_tenants_status");
        });

        modelBuilder.Entity<tenant_branding>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__tenant_b__3213E83F48B5F337");

            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.tenant).WithOne(p => p.tenant_branding)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tenant_branding_tenants");
        });

        modelBuilder.Entity<tenant_business_rule>(entity =>
        {
            entity.Property(e => e.is_active).HasDefaultValue(true);

            entity.HasOne(d => d.rule_definition).WithMany(p => p.tenant_business_rules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tenant_business_rules_rules");

            entity.HasOne(d => d.tenant).WithMany(p => p.tenant_business_rules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tenant_business_rules_tenants");
        });

        modelBuilder.Entity<tenant_database_connection>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__tenant_d__3213E83F6A1F3347");

            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.is_active).HasDefaultValue(true);

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
