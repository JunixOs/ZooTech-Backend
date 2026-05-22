using FluentAssertions;
using ZooTech.Domain.Entities;
using ZooTech.Domain.Enums;
using ZooTech.Domain.ValueObjects;
using ZooTech.Infrastructure.Persistence.Entities;
using ZooTech.Infrastructure.Persistence.Mappers;

namespace ZooTech.Infrastructure.UnitTests.Persistence.Mappers;

public class VacunoMapperTests
{
    [Fact]
    public void ToDomain_MapsAllFieldsCorrectly()
    {
        // Arrange
        var entity = new vacuno
        {
            id = 99,
            codigo = "VAC-99",
            nombre = "Muu",
            fecha_nacimiento = new DateOnly(2021, 5, 10),
            fecha_registro = new DateOnly(2021, 5, 12),
            raza_code = "HOL",
            color_code = "NEGRO",
            sexo_code = "H",
            tipo_adquisicion_code = "COMPRA",
            granja_id = 1,
            created_at = new DateTime(2021, 5, 12),
            updated_at = new DateTime(2021, 5, 12),
            raza_codeNavigation = new cat_raza { code = "HOL", nombre = "Holstein", activo = true },
            granja = new granja
            {
                id = 1,
                nombre = "Granja Sur",
                distrito_codigo = "010101",
                activo = true,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow,
                distrito_codigoNavigation = new geo_distrito
                {
                    codigo = "010101",
                    nombre = "Distrito 1",
                    provincia_codigo = "0101",
                    provincia_codigoNavigation = new geo_provincium
                    {
                        codigo = "0101",
                        nombre = "Provincia 2",
                        departamento_codigo = "01",
                        departamento_codigoNavigation = new geo_departamento
                        {
                            codigo = "01",
                            nombre = "Dep 3"
                        }
                    }
                }
            }
        };

        // Act
        var animal = VacunoMapper.ToDomain(entity, "MUERTO");

        // Assert
        animal.Id.Value.Should().Be(99);
        animal.Codigo.Should().Be("VAC-99");
        animal.Nombre.Should().Be("Muu");
        animal.FechaNacimiento.Should().Be(new DateTime(2021, 5, 10));
        animal.FechaRegistro.Should().Be(new DateTime(2021, 5, 12));
        animal.Estado.Should().Be(EstadoAnimal.MUERTO);
        animal.Raza.Code.Should().Be("HOL");
        animal.Raza.Nombre.Should().Be("Holstein");
        animal.Procedencia.Granja.Should().Be("Granja Sur");
        animal.Procedencia.Distrito.Should().Be("Distrito 1");
        animal.Procedencia.Provincia.Should().Be("Provincia 2");
        animal.Procedencia.Departamento.Should().Be("Dep 3");
        animal.CreatedAt.Should().Be(new DateTime(2021, 5, 12));
    }

    [Fact]
    public void ToDomain_WhenNoEstado_DefaultsToVivo()
    {
        // Arrange
        var entity = new vacuno
        {
            id = 42,
            codigo = "VAC-42",
            nombre = "Pepe",
            fecha_nacimiento = new DateOnly(2022, 1, 1),
            fecha_registro = new DateOnly(2022, 1, 2),
            raza_code = "JER",
            color_code = "MARRON",
            sexo_code = "M",
            tipo_adquisicion_code = "NACIMIENTO",
            granja_id = 1,
            created_at = new DateTime(2022, 1, 2),
            updated_at = new DateTime(2022, 1, 2),
            raza_codeNavigation = new cat_raza { code = "JER", nombre = "Jersey", activo = true },
            granja = new granja
            {
                id = 1,
                nombre = "G1",
                distrito_codigo = "010101",
                activo = true,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow,
                distrito_codigoNavigation = new geo_distrito
                {
                    codigo = "010101",
                    nombre = "D1",
                    provincia_codigo = "0101",
                    provincia_codigoNavigation = new geo_provincium
                    {
                        codigo = "0101",
                        nombre = "P1",
                        departamento_codigo = "01",
                        departamento_codigoNavigation = new geo_departamento
                        {
                            codigo = "01",
                            nombre = "Dep1"
                        }
                    }
                }
            }
        };

        // Act
        var animal = VacunoMapper.ToDomain(entity, null);

        // Assert
        animal.Id.Value.Should().Be(42);
        animal.Estado.Should().Be(EstadoAnimal.VIVO);
        animal.Raza.Code.Should().Be("JER");
        animal.Raza.Nombre.Should().Be("Jersey");
    }

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
        var entity = VacunoMapper.ToEntity(animal, "NEGRO", "H", "COMPRA", 1);

        // Assert
        entity.id.Should().Be(99);
        entity.codigo.Should().Be("VAC-99");
        entity.nombre.Should().Be("Muu");
        entity.fecha_nacimiento.Should().Be(new DateOnly(2021, 5, 10));
        entity.fecha_registro.Should().Be(new DateOnly(2021, 5, 12));
        entity.raza_code.Should().Be("HOL");
        entity.color_code.Should().Be("NEGRO");
        entity.sexo_code.Should().Be("H");
        entity.tipo_adquisicion_code.Should().Be("COMPRA");
        entity.granja_id.Should().Be(1);
        entity.created_at.Should().Be(new DateTime(2021, 5, 12));
    }
}
