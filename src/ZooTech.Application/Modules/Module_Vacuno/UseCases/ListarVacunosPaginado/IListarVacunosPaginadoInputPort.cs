namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunosPaginado;

public interface IListarVacunosPaginadoInputPort
{
    Task Handle(ListarVacunosPaginadoCommand command);
}
