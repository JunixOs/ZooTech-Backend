namespace ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;

public sealed class AnimalNotFoundException : Exception
{
    public AnimalNotFoundException(long animalId)
        : base($"El vacuno con ID {animalId} no existe o ya fue eliminado.")
    {
        AnimalId = animalId;
    }

    public long AnimalId { get; }
}
