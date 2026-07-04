namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunosPaginado;

public interface IListarVacunosPaginadoOutputPort
{
    Task Ok(ListarVacunosPaginadoOutput output);
    Task Error(string code, string message);
}
