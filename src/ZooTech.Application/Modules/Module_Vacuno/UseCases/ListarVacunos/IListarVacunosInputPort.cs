namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public interface IListarVacunosInputPort
{
    Task<ListarVacunosOutput> HandleAsync(ListarVacunosCommand command, CancellationToken cancellationToken = default);
}
