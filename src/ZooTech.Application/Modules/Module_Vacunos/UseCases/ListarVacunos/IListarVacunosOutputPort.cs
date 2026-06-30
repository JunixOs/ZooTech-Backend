namespace ZooTech.Application.Modules.Module_Vacunos.UseCases.ListarVacunos;

public interface IListarVacunosOutputPort
{
    Task Ok(ListarVacunosOutput output);
    Task Error(string code, string message);
}
