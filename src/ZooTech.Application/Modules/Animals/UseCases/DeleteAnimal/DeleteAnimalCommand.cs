namespace ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;

public sealed record DeleteAnimalCommand(
    long Id,
    string? MotivoEliminacion,
    long? EliminadoPor);
