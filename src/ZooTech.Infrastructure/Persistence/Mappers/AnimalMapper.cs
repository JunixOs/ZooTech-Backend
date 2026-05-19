using ZooTech.Domain.Entities;
using ZooTech.Domain.Enums;
using ZooTech.Domain.ValueObjects;
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
