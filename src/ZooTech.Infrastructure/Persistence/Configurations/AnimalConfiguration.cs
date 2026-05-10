using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Configurations;

public class AnimalConfiguration : IEntityTypeConfiguration<AnimalEntity>
{
    public void Configure(EntityTypeBuilder<AnimalEntity> builder)
    {
        builder.ToTable("Animals");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Codigo).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Estado).IsRequired().HasMaxLength(20);
        builder.Property(x => x.RazaCode).IsRequired().HasMaxLength(20);
        builder.Property(x => x.RazaNombre).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ProcedenciaGranja).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ProcedenciaDistrito).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ProcedenciaProvincia).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ProcedenciaDepartamento).IsRequired().HasMaxLength(100);

        builder.HasIndex(x => x.Codigo).IsUnique();
        builder.HasIndex(x => x.Estado);
    }
}
