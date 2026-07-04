namespace ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;

public interface IAnimalRepository
{
    Task<AnimalDeleteCandidate?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AnimalDeleteCandidate>> GetActiveDuplicatesAsync(
        AnimalDeleteCandidate animal,
        CancellationToken cancellationToken = default);

    Task<bool> HasDependenciesAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task SoftDeleteAsync(
        long id,
        string motivoEliminacion,
        long? eliminadoPor,
        DateTime fechaEliminacion,
        CancellationToken cancellationToken = default);
}
