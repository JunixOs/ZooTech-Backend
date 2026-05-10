namespace ZooTech.Application.Modules.Module_Animals.UseCases.ListAnimals;

public interface IListAnimalsInputPort
{
    Task Handle(ListAnimalsCommand command);
}
