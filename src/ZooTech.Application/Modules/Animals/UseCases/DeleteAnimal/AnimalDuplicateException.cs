namespace ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;

public sealed class AnimalDuplicateException : Exception
{
    public AnimalDuplicateException(long animalId)
        : base($"El vacuno con ID {animalId} tiene duplicados, pero no se pudo resolver un registro eliminable.")
    {
        AnimalId = animalId;
    }

    public long AnimalId { get; }
}
