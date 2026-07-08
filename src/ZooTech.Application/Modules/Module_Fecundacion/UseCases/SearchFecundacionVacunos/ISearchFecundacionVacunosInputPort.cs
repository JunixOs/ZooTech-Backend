namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.SearchFecundacionVacunos;

public interface ISearchFecundacionVacunosInputPort
{
    Task<IReadOnlyList<SearchFecundacionVacunoOutput>> HandleAsync(
        SearchFecundacionVacunosQuery query,
        CancellationToken cancellationToken = default);
}
