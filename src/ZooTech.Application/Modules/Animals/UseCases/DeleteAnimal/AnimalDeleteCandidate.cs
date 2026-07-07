namespace ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;

public sealed record AnimalDeleteCandidate(
    long Id,
    string Codigo,
    string Nombre,
    DateOnly FechaNacimiento,
    long GranjaId,
    DateTime CreatedAt,
    DateTime? DeletedAt);
