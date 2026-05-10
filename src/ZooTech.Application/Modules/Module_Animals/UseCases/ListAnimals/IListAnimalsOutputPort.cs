namespace ZooTech.Application.Modules.Module_Animals.UseCases.ListAnimals;

public interface IListAnimalsOutputPort
{
    Task Ok(ListAnimalsOutput output);
    Task Error(string code, string message);
}
