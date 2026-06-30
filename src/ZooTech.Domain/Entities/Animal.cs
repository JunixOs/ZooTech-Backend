using ZooTech.Domain.Enums;
using ZooTech.Domain.Exceptions;
using ZooTech.Domain.ValueObjects;

namespace ZooTech.Domain.Entities;

public class Animal
{
    public AnimalId Id { get; private set; }
    public string Codigo { get; private set; }
    public string Nombre { get; private set; }
    public DateTime FechaNacimiento { get; private set; }
    public DateTime FechaRegistro { get; private set; }
    public EstadoAnimal Estado { get; private set; }
    public Raza Raza { get; private set; }
    public Procedencia Procedencia { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Animal() { }

    public static Animal Create(
        AnimalId id,
        string codigo,
        string nombre,
        DateTime fechaNacimiento,
        DateTime fechaRegistro,
        Raza raza,
        Procedencia procedencia,
        DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(codigo)) throw new DomainException("El código es obligatorio.");
        if (string.IsNullOrWhiteSpace(nombre)) throw new DomainException("El nombre es obligatorio.");
        if (fechaNacimiento > DateTime.UtcNow) throw new DomainException("La fecha de nacimiento no puede ser futura.");

        return new Animal
        {
            Id = id,
            Codigo = codigo,
            Nombre = nombre,
            FechaNacimiento = fechaNacimiento,
            FechaRegistro = fechaRegistro,
            Estado = EstadoAnimal.VIVO, // Estado inicial
            Raza = raza,
            Procedencia = procedencia,
            CreatedAt = createdAt
        };
    }

    public static Animal Reconstitute(
        AnimalId id,
        string codigo,
        string nombre,
        DateTime fechaNacimiento,
        DateTime fechaRegistro,
        EstadoAnimal estado,
        Raza raza,
        Procedencia procedencia,
        DateTime createdAt)
    {
        return new Animal
        {
            Id = id,
            Codigo = codigo,
            Nombre = nombre,
            FechaNacimiento = fechaNacimiento,
            FechaRegistro = fechaRegistro,
            Estado = estado,
            Raza = raza,
            Procedencia = procedencia,
            CreatedAt = createdAt
        };
    }
}
