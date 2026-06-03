using Microsoft.EntityFrameworkCore;
using ZooTech.Domain.Module_Vacuno.Entities;
using ZooTech.Infrastructure.Persistence.Entities;
using GeneratedVacunoAdquisicion = ZooTech.Infrastructure.Persistence.Entities.vacuno_adquisicion;
using GeneratedVacunoFoto = ZooTech.Infrastructure.Persistence.Entities.vacuno_foto;
using GeneratedVacunoUtilizacion = ZooTech.Infrastructure.Persistence.Entities.vacuno_utilizacion_historial;
using GeneratedVacuno = ZooTech.Infrastructure.Persistence.Entities.vacuno;

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
        public DbSet<geo_departamento> GeoDepartamentos => Set<geo_departamento>();
        public DbSet<geo_provincium> GeoProvincias => Set<geo_provincium>();
        public DbSet<geo_distrito> GeoDistritos => Set<geo_distrito>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GanaderiaDbContext).Assembly);
            modelBuilder.Ignore<granja>();
            modelBuilder.Ignore<GeneratedVacuno>();
            modelBuilder.Ignore<GeneratedVacunoAdquisicion>();
            modelBuilder.Ignore<GeneratedVacunoFoto>();
            modelBuilder.Ignore<GeneratedVacunoUtilizacion>();

            modelBuilder.Entity<geo_departamento>(entity =>
            {
                entity.ToTable("geo_departamento");
                entity.HasKey(e => e.codigo);
            });

            modelBuilder.Entity<geo_provincium>(entity =>
            {
                entity.ToTable("geo_provincia");
                entity.HasKey(e => e.codigo);

                entity.HasOne(e => e.departamento_codigoNavigation)
                    .WithMany(e => e.geo_provincia)
                    .HasForeignKey(e => e.departamento_codigo);
            });

            modelBuilder.Entity<geo_distrito>(entity =>
            {
                entity.ToTable("geo_distrito");
                entity.HasKey(e => e.codigo);
                entity.Ignore(e => e.granjas);

                entity.HasOne(e => e.provincia_codigoNavigation)
                    .WithMany(e => e.geo_distritos)
                    .HasForeignKey(e => e.provincia_codigo);
            });
        }
    }
}
