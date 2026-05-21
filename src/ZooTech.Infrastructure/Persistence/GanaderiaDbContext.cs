using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Vacuno.Entities;

namespace ZooTech.Infrastructure.Persistence
{
    public class GanaderiaDbContext : DbContext
    {
        public GanaderiaDbContext(DbContextOptions<GanaderiaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vacuno> Vacunos => Set<Vacuno>();
        public DbSet<VacunoAdquisicion> VacunosAdquisicion => Set<VacunoAdquisicion>();
        public DbSet<VacunoFoto> VacunosFoto => Set<VacunoFoto>();
        public DbSet<VacunoUtilizacionHistorial> VacunosUtilizacion => Set<VacunoUtilizacionHistorial>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GanaderiaDbContext).Assembly);
        }
    }
}