namespace ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;

public sealed record DeleteAnimalOutput(
    long Id,
    string Codigo,
    string Nombre,
    string MotivoEliminacion,
    long? EliminadoPor,
    DateTime FechaEliminacion,
    bool ResueltoComoDuplicado);
