namespace ZooTech.Application.Modules.Module_Vacunos.UseCases.ListarVacunos;

public interface IListarVacunosInputPort
{
    Task Handle(ListarVacunosCommand command);
}
