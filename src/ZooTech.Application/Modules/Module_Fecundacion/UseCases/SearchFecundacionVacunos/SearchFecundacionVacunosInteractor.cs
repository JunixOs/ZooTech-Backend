using ZooTech.Domain.Module_Fecundacion.Interfaces;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.SearchFecundacionVacunos;

public sealed class SearchFecundacionVacunosInteractor : ISearchFecundacionVacunosInputPort
{
    private readonly IFecundacionRepository _repository;

    public SearchFecundacionVacunosInteractor(IFecundacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<SearchFecundacionVacunoOutput>> HandleAsync(
        SearchFecundacionVacunosQuery query,
        CancellationToken cancellationToken = default)
    {
        var data = await _repository.SearchVacunosAsync(query.Sexo, query.Query, cancellationToken);

        return data
            .Select(item => new SearchFecundacionVacunoOutput(item.Id, item.Codigo, item.Nombre, item.Sexo))
            .ToList();
    }
}
