namespace ZooTech.Application.Modules.Module_Vacuno.UseCases.ListarVacunos;

public interface IListarVacunosInputPort
{
    Task<ListarVacunosOutput> HandleAsync(
        ListarVacunosQuery? query = null,
        CancellationToken cancellationToken = default);
}
