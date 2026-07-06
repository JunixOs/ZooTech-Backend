namespace ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;

public sealed class DeleteAnimalInteractor : IDeleteAnimalInputPort
{
    private readonly IAnimalRepository animalRepository;
    private readonly DeleteAnimalValidator validator;

    public DeleteAnimalInteractor(
        IAnimalRepository animalRepository,
        DeleteAnimalValidator validator)
    {
        this.animalRepository = animalRepository;
        this.validator = validator;
    }

    public async Task Handle(
        DeleteAnimalCommand command,
        IDeleteAnimalOutputPort outputPort,
        CancellationToken cancellationToken = default)
    {
        try
        {
            validator.Validate(command);

            var animal = await animalRepository.GetByIdAsync(command.Id, cancellationToken);
            if (animal is null)
            {
                throw new AnimalNotFoundException(command.Id);
            }

            var resolution = await ResolveDeleteTargetAsync(animal, cancellationToken);
            var hasDependencies = await animalRepository.HasDependenciesAsync(resolution.Target.Id, cancellationToken);
            if (hasDependencies && !resolution.CanDeleteWithDependencies)
            {
                throw new AnimalHasDependenciesException(resolution.Target.Id);
            }

            var fechaEliminacion = DateTime.UtcNow;
            var motivoEliminacion = command.MotivoEliminacion!.Trim();

            await animalRepository.SoftDeleteAsync(
                resolution.Target.Id,
                motivoEliminacion,
                command.EliminadoPor,
                fechaEliminacion,
                cancellationToken);

            outputPort.PresentSuccess(new DeleteAnimalOutput(
                resolution.Target.Id,
                resolution.Target.Codigo,
                resolution.Target.Nombre,
                motivoEliminacion,
                command.EliminadoPor,
                fechaEliminacion,
                resolution.ResueltoComoDuplicado));
        }
        catch (AnimalDeleteValidationException exception)
        {
            outputPort.PresentValidationError(exception);
        }
        catch (AnimalNotFoundException exception)
        {
            outputPort.PresentNotFound(exception);
        }
        catch (AnimalHasDependenciesException exception)
        {
            outputPort.PresentConflict(exception);
        }
        catch (AnimalDuplicateException exception)
        {
            outputPort.PresentDuplicateConflict(exception);
        }
    }

    private async Task<DeleteAnimalResolution> ResolveDeleteTargetAsync(
        AnimalDeleteCandidate animal,
        CancellationToken cancellationToken)
    {
        var duplicates = await animalRepository.GetActiveDuplicatesAsync(animal, cancellationToken);
        if (duplicates.Count == 0)
        {
            return new DeleteAnimalResolution(animal, false, false);
        }

        var candidates = duplicates.Append(animal).ToArray();
        foreach (var candidate in candidates.OrderBy(candidate => candidate.CreatedAt))
        {
            if (!await animalRepository.HasDependenciesAsync(candidate.Id, cancellationToken))
            {
                return new DeleteAnimalResolution(candidate, false, candidate.Id != animal.Id);
            }
        }

        var latest = candidates
            .OrderByDescending(candidate => candidate.CreatedAt)
            .ThenByDescending(candidate => candidate.Id)
            .FirstOrDefault()
            ?? throw new AnimalDuplicateException(animal.Id);

        return new DeleteAnimalResolution(latest, true, latest.Id != animal.Id);
    }

    private sealed record DeleteAnimalResolution(
        AnimalDeleteCandidate Target,
        bool CanDeleteWithDependencies,
        bool ResueltoComoDuplicado);
}
