using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ZooTech.Infrastructure.Persistence.Modules.Module_Sanidad.Entities;

namespace ZooTech.Infrastructure.Context;

public partial class ZootechContext : DbContext
{
    public ZootechContext(DbContextOptions<ZootechContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CatEstadoRegistro> CatEstadoRegistros { get; set; }

    public virtual DbSet<CatTipoPeso> CatTipoPesos { get; set; }

    public virtual DbSet<Triaje> Triajes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatEstadoRegistro>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("PK__cat_esta__357D4CF8DC626498");
        });

        modelBuilder.Entity<CatTipoPeso>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("PK__cat_tipo__357D4CF8E03DBB3A");
        });

        modelBuilder.Entity<Triaje>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__triaje__3213E83F8AC578E0");

            entity.HasOne(d => d.EstadoRegistroCodeNavigation).WithMany(p => p.Triajes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_triaje_estado");

            entity.HasOne(d => d.TipoPesoCodeNavigation).WithMany(p => p.Triajes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_triaje_tipo_peso");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
