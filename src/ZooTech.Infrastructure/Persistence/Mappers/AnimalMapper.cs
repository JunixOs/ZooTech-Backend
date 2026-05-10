using ZooTech.Domain.Modules.Module_Animals.Entities;
using ZooTech.Domain.Modules.Module_Animals.Enums;
using ZooTech.Domain.Modules.Module_Animals.ValueObjects;
using ZooTech.Infrastructure.Persistence.Entities;

namespace ZooTech.Infrastructure.Persistence.Mappers;

public static class AnimalMapper
{
    public static AnimalEntity ToEntity(Animal animal)
    {
        return new AnimalEntity
        {
            Id = animal.Id.Value,
            Codigo = animal.Codigo,
            Nombre = animal.Nombre,
            FechaNacimiento = animal.FechaNacimiento,
            FechaRegistro = animal.FechaRegistro,
            Estado = animal.Estado.ToString(),
            RazaCode = animal.Raza.Code,
            RazaNombre = animal.Raza.Nombre,
            ProcedenciaGranja = animal.Procedencia.Granja,
            ProcedenciaDistrito = animal.Procedencia.Distrito,
            ProcedenciaProvincia = animal.Procedencia.Provincia,
            ProcedenciaDepartamento = animal.Procedencia.Departamento,
            CreatedAt = animal.CreatedAt
        };
    }

    public static Animal ToDomain(AnimalEntity entity)
    {
        return Animal.Reconstitute(
            AnimalId.Of(entity.Id),
            entity.Codigo,
            entity.Nombre,
            entity.FechaNacimiento,
            entity.FechaRegistro,
            Enum.Parse<EstadoAnimal>(entity.Estado),
            new Raza(entity.RazaCode, entity.RazaNombre),
            new Procedencia(entity.ProcedenciaGranja, entity.ProcedenciaDistrito, entity.ProcedenciaProvincia, entity.ProcedenciaDepartamento),
            entity.CreatedAt
        );
    }
}
