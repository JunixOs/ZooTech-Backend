using Microsoft.EntityFrameworkCore;
using ZooTech.Infrastructure.Persistence.ReadModels;

namespace ZooTech.Infrastructure.Persistence.Context;

public partial class GanaderiaDbContext
{
    public virtual DbSet<ReporteVacunoListadoRow> ReporteVacunoListadoRows { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReporteVacunoListadoRow>(entity =>
        {
            entity.HasNoKey();
            entity.ToView(null);
        });
    }
}
