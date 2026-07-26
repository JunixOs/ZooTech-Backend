using ZooTech.Application.Common.Behaviors;

namespace ZooTech.Application.Modules.Module_Fecundacion.UseCases.SearchFecundacionVacunos;

public interface ISearchFecundacionVacunosInputPort
    : IRequestHandler<SearchFecundacionVacunosQuery , IReadOnlyList<SearchFecundacionVacunoOutput>>
{
}
