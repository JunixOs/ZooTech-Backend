using ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;

namespace ZooTech.Application.UnitTests.Modules.Animals.UseCases.DeleteAnimal;

public sealed class DeleteAnimalInteractorTests
{
    private static readonly AnimalDeleteCandidate Animal = new(
        10,
        "VAC-001",
        "Luna",
        new DateOnly(2020, 1, 15),
        3,
        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        null);

    [Fact]
    public async Task Handle_ShouldSoftDelete_WhenAnimalHasNoDependencies()
    {
        var repository = new FakeAnimalRepository { Animal = Animal };
        var presenter = new TestDeleteAnimalOutputPort();
        var interactor = CreateInteractor(repository);

        await interactor.Handle(new DeleteAnimalCommand(Animal.Id, "Registro duplicado", 8), presenter);

        Assert.NotNull(presenter.Success);
        Assert.Equal(Animal.Id, repository.SoftDeletedId);
        Assert.Equal("Registro duplicado", repository.MotivoEliminacion);
        Assert.Equal(8, repository.EliminadoPor);
        Assert.NotNull(repository.FechaEliminacion);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenReasonIsEmpty()
    {
        var repository = new FakeAnimalRepository { Animal = Animal };
        var presenter = new TestDeleteAnimalOutputPort();
        var interactor = CreateInteractor(repository);

        await interactor.Handle(new DeleteAnimalCommand(Animal.Id, string.Empty, 8), presenter);

        Assert.Null(presenter.Success);
        Assert.NotNull(presenter.ValidationError);
        Assert.Contains("motivoEliminacion", presenter.ValidationError.Errors.Keys);
        Assert.Null(repository.SoftDeletedId);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenReasonIsWhitespace()
    {
        var repository = new FakeAnimalRepository { Animal = Animal };
        var presenter = new TestDeleteAnimalOutputPort();
        var interactor = CreateInteractor(repository);

        await interactor.Handle(new DeleteAnimalCommand(Animal.Id, "   ", 8), presenter);

        Assert.Null(presenter.Success);
        Assert.NotNull(presenter.ValidationError);
        Assert.Contains("motivoEliminacion", presenter.ValidationError.Errors.Keys);
        Assert.Null(repository.SoftDeletedId);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenReasonExceeds250Characters()
    {
        var repository = new FakeAnimalRepository { Animal = Animal };
        var presenter = new TestDeleteAnimalOutputPort();
        var interactor = CreateInteractor(repository);

        await interactor.Handle(new DeleteAnimalCommand(Animal.Id, new string('A', 251), 8), presenter);

        Assert.Null(presenter.Success);
        Assert.NotNull(presenter.ValidationError);
        Assert.Contains("motivoEliminacion", presenter.ValidationError.Errors.Keys);
        Assert.Null(repository.SoftDeletedId);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenDeletedByIsInvalid()
    {
        var repository = new FakeAnimalRepository { Animal = Animal };
        var presenter = new TestDeleteAnimalOutputPort();
        var interactor = CreateInteractor(repository);

        await interactor.Handle(new DeleteAnimalCommand(Animal.Id, "Baja administrativa", 0), presenter);

        Assert.Null(presenter.Success);
        Assert.NotNull(presenter.ValidationError);
        Assert.Contains("eliminadoPor", presenter.ValidationError.Errors.Keys);
        Assert.Null(repository.SoftDeletedId);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenAnimalDoesNotExist()
    {
        var repository = new FakeAnimalRepository();
        var presenter = new TestDeleteAnimalOutputPort();
        var interactor = CreateInteractor(repository);

        await interactor.Handle(new DeleteAnimalCommand(999, "No corresponde", 8), presenter);

        Assert.Null(presenter.Success);
        Assert.NotNull(presenter.NotFound);
        Assert.Equal(999, presenter.NotFound.AnimalId);
        Assert.Null(repository.SoftDeletedId);
    }

    [Fact]
    public async Task Handle_ShouldReturnConflict_WhenAnimalHasDependencies()
    {
        var repository = new FakeAnimalRepository { Animal = Animal };
        repository.DependencyIds.Add(Animal.Id);
        var presenter = new TestDeleteAnimalOutputPort();
        var interactor = CreateInteractor(repository);

        await interactor.Handle(new DeleteAnimalCommand(Animal.Id, "No corresponde", 8), presenter);

        Assert.Null(presenter.Success);
        Assert.NotNull(presenter.DependencyConflict);
        Assert.Equal(Animal.Id, presenter.DependencyConflict.AnimalId);
        Assert.Null(repository.SoftDeletedId);
    }

    [Fact]
    public async Task Handle_ShouldPersistDeletedAtDeletedByAndReason()
    {
        var repository = new FakeAnimalRepository { Animal = Animal };
        var presenter = new TestDeleteAnimalOutputPort();
        var interactor = CreateInteractor(repository);

        await interactor.Handle(new DeleteAnimalCommand(Animal.Id, "  Baja por duplicidad  ", 22), presenter);

        Assert.NotNull(presenter.Success);
        Assert.Equal(Animal.Id, repository.SoftDeletedId);
        Assert.Equal("Baja por duplicidad", repository.MotivoEliminacion);
        Assert.Equal(22, repository.EliminadoPor);
        Assert.NotNull(repository.FechaEliminacion);
        Assert.Equal(repository.FechaEliminacion, presenter.Success.FechaEliminacion);
    }

    [Fact]
    public async Task Handle_ShouldDeleteDuplicateWithoutDependencies_WhenRequestedAnimalHasDependencies()
    {
        var duplicateWithoutDependencies = Animal with
        {
            Id = 11,
            CreatedAt = Animal.CreatedAt.AddDays(1)
        };

        var repository = new FakeAnimalRepository
        {
            Animal = Animal,
            Duplicates = [duplicateWithoutDependencies]
        };
        repository.DependencyIds.Add(Animal.Id);
        var presenter = new TestDeleteAnimalOutputPort();
        var interactor = CreateInteractor(repository);

        await interactor.Handle(new DeleteAnimalCommand(Animal.Id, "Duplicado", 8), presenter);

        Assert.NotNull(presenter.Success);
        Assert.Equal(duplicateWithoutDependencies.Id, repository.SoftDeletedId);
        Assert.True(presenter.Success.ResueltoComoDuplicado);
    }

    [Fact]
    public async Task Handle_ShouldApplyLatestRecordRule_WhenAllDuplicatesHaveDependencies()
    {
        var latestDuplicate = Animal with
        {
            Id = 11,
            CreatedAt = Animal.CreatedAt.AddDays(1)
        };

        var repository = new FakeAnimalRepository
        {
            Animal = Animal,
            Duplicates = [latestDuplicate]
        };
        repository.DependencyIds.Add(Animal.Id);
        repository.DependencyIds.Add(latestDuplicate.Id);
        var presenter = new TestDeleteAnimalOutputPort();
        var interactor = CreateInteractor(repository);

        await interactor.Handle(new DeleteAnimalCommand(Animal.Id, "Regla ultimo registro", 8), presenter);

        Assert.NotNull(presenter.Success);
        Assert.Equal(latestDuplicate.Id, repository.SoftDeletedId);
        Assert.True(presenter.Success.ResueltoComoDuplicado);
    }

    private static DeleteAnimalInteractor CreateInteractor(FakeAnimalRepository repository)
        => new(repository, new DeleteAnimalValidator());

    private sealed class FakeAnimalRepository : IAnimalRepository
    {
        public AnimalDeleteCandidate? Animal { get; set; }

        public IReadOnlyCollection<AnimalDeleteCandidate> Duplicates { get; set; } = [];

        public HashSet<long> DependencyIds { get; } = [];

        public long? SoftDeletedId { get; private set; }

        public string? MotivoEliminacion { get; private set; }

        public long? EliminadoPor { get; private set; }

        public DateTime? FechaEliminacion { get; private set; }

        public Task<AnimalDeleteCandidate?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
            => Task.FromResult(Animal?.Id == id ? Animal : null);

        public Task<IReadOnlyCollection<AnimalDeleteCandidate>> GetActiveDuplicatesAsync(
            AnimalDeleteCandidate animal,
            CancellationToken cancellationToken = default)
            => Task.FromResult(Duplicates);

        public Task<bool> HasDependenciesAsync(long id, CancellationToken cancellationToken = default)
            => Task.FromResult(DependencyIds.Contains(id));

        public Task SoftDeleteAsync(
            long id,
            string motivoEliminacion,
            long? eliminadoPor,
            DateTime fechaEliminacion,
            CancellationToken cancellationToken = default)
        {
            SoftDeletedId = id;
            MotivoEliminacion = motivoEliminacion;
            EliminadoPor = eliminadoPor;
            FechaEliminacion = fechaEliminacion;
            return Task.CompletedTask;
        }
    }

    private sealed class TestDeleteAnimalOutputPort : IDeleteAnimalOutputPort
    {
        public DeleteAnimalOutput? Success { get; private set; }

        public AnimalNotFoundException? NotFound { get; private set; }

        public AnimalDeleteValidationException? ValidationError { get; private set; }

        public AnimalHasDependenciesException? DependencyConflict { get; private set; }

        public AnimalDuplicateException? DuplicateConflict { get; private set; }

        public void PresentSuccess(DeleteAnimalOutput output)
            => Success = output;

        public void PresentNotFound(AnimalNotFoundException exception)
            => NotFound = exception;

        public void PresentValidationError(AnimalDeleteValidationException exception)
            => ValidationError = exception;

        public void PresentConflict(AnimalHasDependenciesException exception)
            => DependencyConflict = exception;

        public void PresentDuplicateConflict(AnimalDuplicateException exception)
            => DuplicateConflict = exception;
    }
}
