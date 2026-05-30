using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZooTech.Domain.Module_Vacuno.Entities;

namespace ZooTech.Infrastructure.Persistence.Modules.Module_Vacuno;

/// <summary>
/// Mapeo Fluent API de Vacuno hacia las tablas de la BD (dbo.vacuno, dbo.vacuno_adquisicion, etc.)
/// </summary>
public class VacunoEntityConfiguration :
    IEntityTypeConfiguration<Vacuno>,
    IEntityTypeConfiguration<VacunoAdquisicion>,
    IEntityTypeConfiguration<VacunoFoto>,
    IEntityTypeConfiguration<VacunoUtilizacionHistorial>
{
    // ── Vacuno ────────────────────────────────────────────────────────
    public void Configure(EntityTypeBuilder<Vacuno> builder)
    {
        builder.ToTable("vacuno");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).HasColumnName("id").UseIdentityColumn();

        builder.Property(v => v.Codigo)
            .HasColumnName("codigo")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(v => v.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(v => v.FechaNacimiento)
            .HasColumnName("fecha_nacimiento")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(v => v.IdRaza).HasColumnName("id_raza");
        builder.Property(v => v.Raza)
            .HasColumnName("raza")
            .HasMaxLength(15)
            .IsRequired();
        builder.Property(v => v.IdColor).HasColumnName("id_color");
        builder.Property(v => v.Color)
            .HasColumnName("color")
            .HasMaxLength(15)
            .IsRequired();
        builder.Property(v => v.IdSexo).HasColumnName("id_sexo");
        builder.Property(v => v.Sexo)
            .HasColumnName("sexo")
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(v => v.CodigoPadre)
            .HasColumnName("codigo_padre")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(v => v.CodigoMadre)
            .HasColumnName("codigo_madre")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(v => v.IdGranja).HasColumnName("id_granja");
        builder.Property(v => v.Granja)
            .HasColumnName("granja")
            .HasMaxLength(15)
            .IsRequired();
        builder.Property(v => v.IdDistrito).HasColumnName("id_distrito");
        builder.Property(v => v.Distrito)
            .HasColumnName("distrito")
            .HasMaxLength(15)
            .IsRequired();
        builder.Property(v => v.IdDepartamento).HasColumnName("id_departamento");
        builder.Property(v => v.Departamento)
            .HasColumnName("departamento")
            .HasMaxLength(15)
            .IsRequired();
        builder.Property(v => v.IdProvincia).HasColumnName("id_provincia");
        builder.Property(v => v.Provincia)
            .HasColumnName("provincia")
            .HasMaxLength(15)
            .IsRequired();
        builder.Property(v => v.IdEstado).HasColumnName("id_estado");

        builder.Property(v => v.CreadoEn)
            .HasColumnName("creado_en")
            .HasColumnType("datetime2");

        builder.Property(v => v.ActualizadoEn)
            .HasColumnName("actualizado_en")
            .HasColumnType("datetime2");

        // Índice único por código (dentro del tenant — EF filtra por schema/tenant)
        builder.HasIndex(v => v.Codigo)
            .IsUnique()
            .HasDatabaseName("IX_vacuno_codigo");

        // Relaciones
        builder.HasOne(v => v.Adquisicion)
            .WithOne(a => a.Vacuno)
            .HasForeignKey<VacunoAdquisicion>(a => a.IdVacuno);

        builder.HasOne(v => v.Foto)
            .WithOne(f => f.Vacuno)
            .HasForeignKey<VacunoFoto>(f => f.IdVacuno);

        builder.HasOne(v => v.Utilizacion)
            .WithOne(u => u.Vacuno)
            .HasForeignKey<VacunoUtilizacionHistorial>(u => u.IdVacuno);
    }

    // ── VacunoAdquisicion ─────────────────────────────────────────────
    public void Configure(EntityTypeBuilder<VacunoAdquisicion> builder)
    {
        builder.ToTable("vacuno_adquisicion");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id").UseIdentityColumn();
        builder.Property(a => a.IdVacuno).HasColumnName("id_vacuno");
        builder.Property(a => a.IdTipoAdquisicion).HasColumnName("id_tipo_adquisicion");
        builder.Property(a => a.PrecioCompra)
            .HasColumnName("precio_compra")
            .HasColumnType("decimal(18,2)")
            .IsRequired(false);
    }

    // ── VacunoFoto ────────────────────────────────────────────────────
    public void Configure(EntityTypeBuilder<VacunoFoto> builder)
    {
        builder.ToTable("vacuno_foto");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).HasColumnName("id").UseIdentityColumn();
        builder.Property(f => f.IdVacuno).HasColumnName("id_vacuno");
        builder.Property(f => f.RutaArchivo)
            .HasColumnName("ruta_archivo")
            .HasMaxLength(500)
            .IsRequired();
    }

    // ── VacunoUtilizacionHistorial ────────────────────────────────────
    public void Configure(EntityTypeBuilder<VacunoUtilizacionHistorial> builder)
    {
        builder.ToTable("vacuno_utilizacion_historial");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id").UseIdentityColumn();
        builder.Property(u => u.IdVacuno).HasColumnName("id_vacuno");
        builder.Property(u => u.IdTipoUtilizacion).HasColumnName("id_tipo_utilizacion");
        builder.Property(u => u.AptoPara)
            .HasColumnName("apto_para")
            .HasMaxLength(30)
            .IsRequired();
        builder.Property(u => u.FechaEspecificacion)
            .HasColumnName("fecha_especificacion")
            .HasColumnType("date");
        builder.Property(u => u.Observaciones)
            .HasColumnName("observaciones")
            .HasMaxLength(150)
            .IsRequired(false);
    }
}
