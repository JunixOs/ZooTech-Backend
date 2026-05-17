using Microsoft.EntityFrameworkCore;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Context;

public partial class ZootechContext : DbContext
{
    public ZootechContext(DbContextOptions<ZootechContext> options)
        : base(options)
    {
    }

    public virtual DbSet<cat_estado_registro> CatEstadoRegistros { get; set; }

    public virtual DbSet<cat_tipo_peso> CatTipoPesos { get; set; }
    public virtual DbSet<vacuno> Vacunos { get; set; }

    public virtual DbSet<triaje> Triajes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<cat_estado_registro>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_esta__357D4CF8DC626498");
        });

        modelBuilder.Entity<cat_tipo_peso>(entity =>
        {
            entity.HasKey(e => e.code).HasName("PK__cat_tipo__357D4CF8E03DBB3A");
        });

        modelBuilder.Entity<triaje>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__triaje__3213E83F8AC578E0");

            entity.HasOne(d => d.estado_registro_codeNavigation).WithMany(p => p.triajes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_triaje_estado");

            entity.HasOne(d => d.tipo_peso_codeNavigation).WithMany(p => p.triajes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_triaje_tipo_peso");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
