using FluentAssertions;
using ZooTech.Domain.Modules.Module_Animals.Entities;
using ZooTech.Domain.Modules.Module_Animals.Enums;
using ZooTech.Domain.Modules.Module_Animals.ValueObjects;
using ZooTech.Infrastructure.Persistence.Entities;
using ZooTech.Infrastructure.Persistence.Mappers;

namespace ZooTech.Infrastructure.UnitTests.Persistence.Mappers;

public class AnimalMapperTests
{
    [Fact]
    public void ToEntity_MapsAllDomainFieldsCorrectly()
    {
        // Arrange
        var animal = Animal.Reconstitute(
            AnimalId.Of(99),
            "VAC-99",
            "Muu",
            new DateTime(2021, 5, 10),
            new DateTime(2021, 5, 12),
            EstadoAnimal.MUERTO,
            new Raza("HOL", "Holstein"),
            new Procedencia("Granja Sur", "Distrito 1", "Provincia 2", "Dep 3"),
            new DateTime(2021, 5, 12)
        );

        // Act
        var entity = AnimalMapper.ToEntity(animal);

        // Assert
        entity.Id.Should().Be(99);
        entity.Codigo.Should().Be("VAC-99");
        entity.Nombre.Should().Be("Muu");
        entity.FechaNacimiento.Should().Be(new DateTime(2021, 5, 10));
        entity.FechaRegistro.Should().Be(new DateTime(2021, 5, 12));
        entity.Estado.Should().Be("MUERTO");
        entity.RazaCode.Should().Be("HOL");
        entity.RazaNombre.Should().Be("Holstein");
        entity.ProcedenciaGranja.Should().Be("Granja Sur");
        entity.ProcedenciaDistrito.Should().Be("Distrito 1");
        entity.ProcedenciaProvincia.Should().Be("Provincia 2");
        entity.ProcedenciaDepartamento.Should().Be("Dep 3");
        entity.CreatedAt.Should().Be(new DateTime(2021, 5, 12));
    }

    [Fact]
    public void ToDomain_MapsAllEntityFieldsCorrectly()
    {
        // Arrange
        var entity = new AnimalEntity
        {
            Id = 42,
            Codigo = "VAC-42",
            Nombre = "Pepe",
            FechaNacimiento = new DateTime(2022, 1, 1),
            FechaRegistro = new DateTime(2022, 1, 2),
            Estado = "VIVO",
            RazaCode = "JER",
            RazaNombre = "Jersey",
            ProcedenciaGranja = "G1",
            ProcedenciaDistrito = "D1",
            ProcedenciaProvincia = "P1",
            ProcedenciaDepartamento = "Dep1",
            CreatedAt = new DateTime(2022, 1, 2)
        };

        // Act
        var animal = AnimalMapper.ToDomain(entity);

        // Assert
        animal.Id.Value.Should().Be(42);
        animal.Codigo.Should().Be("VAC-42");
        animal.Nombre.Should().Be("Pepe");
        animal.FechaNacimiento.Should().Be(new DateTime(2022, 1, 1));
        animal.FechaRegistro.Should().Be(new DateTime(2022, 1, 2));
        animal.Estado.Should().Be(EstadoAnimal.VIVO);
        animal.Raza.Code.Should().Be("JER");
        animal.Raza.Nombre.Should().Be("Jersey");
        animal.Procedencia.Granja.Should().Be("G1");
        animal.Procedencia.Distrito.Should().Be("D1");
        animal.Procedencia.Provincia.Should().Be("P1");
        animal.Procedencia.Departamento.Should().Be("Dep1");
        animal.CreatedAt.Should().Be(new DateTime(2022, 1, 2));
    }
}
