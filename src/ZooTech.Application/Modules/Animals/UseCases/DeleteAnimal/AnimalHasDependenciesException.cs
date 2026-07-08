namespace ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;

public sealed class AnimalHasDependenciesException : Exception
{
    public AnimalHasDependenciesException(long animalId)
        : base($"El vacuno con ID {animalId} tiene registros relacionados y no puede eliminarse.")
    {
        AnimalId = animalId;
    }

    public long AnimalId { get; }
}
