namespace ZooTech.Application.Modules.Animals.UseCases.DeleteAnimal;

public interface IDeleteAnimalInputPort
{
    Task Handle(
        DeleteAnimalCommand command,
        IDeleteAnimalOutputPort outputPort,
        CancellationToken cancellationToken = default);
}
